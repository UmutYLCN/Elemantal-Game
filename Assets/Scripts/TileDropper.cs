using UnityEngine;

public class TileDropper
{
    private GameObject[] elementPrefabs;
    private Transform parentTransform;

    public TileDropper(GameObject[] elementPrefabs, Transform parentTransform)
    {
        this.elementPrefabs = elementPrefabs;
        this.parentTransform = parentTransform;
    }

    public void DropTilesAndRefill(Tile[,] tiles, int width, int height, float tileSize, System.Func<int, int, float, Vector2> getWorldPosition, System.Action<Tile, int, int> onTileCreated)
    {
        // Taşları düşür
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (tiles[x, y] == null)
                {
                    for (int k = y + 1; k < height; k++)
                    {
                        if (tiles[x, k] != null)
                        {
                            tiles[x, y] = tiles[x, k];
                            tiles[x, y].SetPosition(x, y);
                            tiles[x, y].transform.position = getWorldPosition(x, y, tileSize);
                            tiles[x, k] = null;
                            break;
                        }
                    }
                }
            }
        }
        // Boş kalan yerlere yeni taş üret
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (tiles[x, y] == null)
                {
                    int randomIndex = Random.Range(0, elementPrefabs.Length);
                    GameObject element = Object.Instantiate(elementPrefabs[randomIndex], getWorldPosition(x, y, tileSize), Quaternion.identity, parentTransform);

                    // Prefab'ın ismine göre tag ata (FireGem -> Fire, WaterGem -> Water, vb.)
                    string prefabName = elementPrefabs[randomIndex].name;
                    string tagName = prefabName.Replace("Gem", "");
                    if (IsValidTag(tagName))
                        element.tag = tagName;

                    Tile tile = element.GetComponent<Tile>();
                    if (tile == null) tile = element.AddComponent<Tile>();
                    tile.SetPosition(x, y);
                    tiles[x, y] = tile;
                    onTileCreated?.Invoke(tile, x, y);
                }
            }
        }
    }

    private bool IsValidTag(string tag)
    {
        try { GameObject.FindWithTag(tag); return true; } catch { return false; }
    }
} 