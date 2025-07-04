using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Header("Grid Size")]
    public int width = 8;
    public int height = 8;

    [Header("Element Prefabs")]
    public GameObject[] elementPrefabs;

    [Header("Tile Settings")]
    public float tileSize = 1.0f;

    [Header("Efektler ve Skor")]
    public GameObject destroyEffectPrefab;
    public ScoreManager scoreManager;

    private Tile[,] tiles;
    private TileDestroyer tileDestroyer;
    private TileDropper tileDropper;

    void Start()
    {
        CalculateTileSizeToFitScreen();
        tileDestroyer = new TileDestroyer(scoreManager, destroyEffectPrefab);
        tileDropper = new TileDropper(elementPrefabs, this.transform);
        GenerateBoard();
        StartCoroutine(CheckAndHandleMatches());
    }

    void CalculateTileSizeToFitScreen()
    {
        Camera cam = Camera.main;
        float screenRatio = (float)Screen.width / (float)Screen.height;
        float camHeight = cam.orthographicSize * 2;
        float camWidth = camHeight * screenRatio;

        float tileSizeX = camWidth / width;
        float tileSizeY = camHeight / height;
        tileSize = Mathf.Min(tileSizeX, tileSizeY) * 0.9f; // %90’ını kullan, kenarlarda boşluk kalsın
    }

    void GenerateBoard()
    {
        tiles = new Tile[width, height];
        float xOffset = -(width - 1) * tileSize / 2f;
        float yOffset = -(height - 1) * tileSize / 2f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject element = null;
                Tile tile = null;
                int tryCount = 0;
                do
                {
                    if (element != null) Object.DestroyImmediate(element);
                    Vector2 spawnPos = new Vector2(x * tileSize + xOffset, y * tileSize + yOffset);
                    int randomIndex = Random.Range(0, elementPrefabs.Length);
                    element = Instantiate(elementPrefabs[randomIndex], spawnPos, Quaternion.identity, this.transform);
                    element.name = $"Element_{x}_{y}";

                    // Prefab'ın ismine göre tag ata (FireGem -> Fire, WaterGem -> Water, vb.)
                    string prefabName = elementPrefabs[randomIndex].name;
                    string tagName = prefabName.Replace("Gem", "");
                    if (IsValidTag(tagName))
                        element.tag = tagName;

                    var spriteRenderer = element.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null && spriteRenderer.sprite != null)
                    {
                        float spriteWidth = spriteRenderer.sprite.rect.width / spriteRenderer.sprite.pixelsPerUnit;
                        float spriteHeight = spriteRenderer.sprite.rect.height / spriteRenderer.sprite.pixelsPerUnit;
                        float scale = tileSize / Mathf.Max(spriteWidth, spriteHeight);
                        element.transform.localScale = Vector3.one * scale;
                    }

                    tile = element.GetComponent<Tile>();
                    if (tile == null)
                        tile = element.AddComponent<Tile>();
                    tile.SetPosition(x, y);
                    tiles[x, y] = tile;
                    tryCount++;
                }
                while (HasMatchOnCreate(x, y) && tryCount < 10);
            }
        }
    }

    // Yeni taş oluşturulurken anında eşleşme var mı kontrolü
    private bool HasMatchOnCreate(int x, int y)
    {
        Tile tile = tiles[x, y];
        if (tile == null) return false;
        // Yatay kontrol
        if (x >= 2)
        {
            if (tiles[x - 1, y] != null && tiles[x - 2, y] != null)
            {
                if (tiles[x - 1, y].tag == tile.tag && tiles[x - 2, y].tag == tile.tag)
                    return true;
            }
        }
        // Dikey kontrol
        if (y >= 2)
        {
            if (tiles[x, y - 1] != null && tiles[x, y - 2] != null)
            {
                if (tiles[x, y - 1].tag == tile.tag && tiles[x, y - 2].tag == tile.tag)
                    return true;
            }
        }
        return false;
    }

    // Tag'ın Unity'de tanımlı olup olmadığını kontrol et
    private bool IsValidTag(string tag)
    {
        try { GameObject.FindWithTag(tag); return true; } catch { return false; }
    }

    public void SwapTiles(Tile a, Tile b)
    {
        int ax = a.x, ay = a.y;
        int bx = b.x, by = b.y;
        tiles[ax, ay] = b;
        tiles[bx, by] = a;
        a.SetPosition(bx, by);
        b.SetPosition(ax, ay);
        Vector3 apos = a.transform.position;
        a.transform.position = b.transform.position;
        b.transform.position = apos;
        StartCoroutine(CheckAndHandleMatches());
    }

    IEnumerator CheckAndHandleMatches()
    {
        yield return new WaitForSeconds(0.2f); // Swap/animasyon için bekle
        List<Tile> matches = MatchFinder.FindMatches(tiles, width, height);
        if (matches.Count > 0)
        {
            yield return StartCoroutine(tileDestroyer.DestroyMatchesCoroutine(matches, tiles));
            yield return new WaitForSeconds(0.2f); // Efekt için bekle
            tileDropper.DropTilesAndRefill(tiles, width, height, tileSize, GetWorldPosition, OnTileCreated);
            yield return new WaitForSeconds(0.2f); // Düşme animasyonu için bekle
            yield return StartCoroutine(CheckAndHandleMatches()); // Zincirleme eşleşme
        }
    }

    public Vector2 GetWorldPosition(int x, int y, float tileSize)
    {
        float xOffset = -(width - 1) * tileSize / 2f;
        float yOffset = -(height - 1) * tileSize / 2f;
        return new Vector2(x * tileSize + xOffset, y * tileSize + yOffset);
    }

    private void OnTileCreated(Tile tile, int x, int y)
    {
        // Gerekirse yeni taş oluşturulurken ek işlemler yapılabilir (örn. animasyon başlatma)
    }
} 