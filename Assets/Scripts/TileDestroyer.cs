using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class TileDestroyer
{
    private ScoreManager scoreManager;
    private GameObject destroyEffectPrefab;
    private float effectDuration = 0.5f;

    public TileDestroyer(ScoreManager scoreManager, GameObject destroyEffectPrefab = null)
    {
        this.scoreManager = scoreManager;
        this.destroyEffectPrefab = destroyEffectPrefab;
    }

    public IEnumerator DestroyMatchesCoroutine(List<Tile> matches, Tile[,] tiles)
    {
        foreach (Tile tile in matches)
        {
            if (tile == null) continue;
            yield return DestroyTileWithEffect(tile, tiles);
        }
    }

    private IEnumerator DestroyTileWithEffect(Tile tile, Tile[,] tiles)
    {
        if (tile == null) yield break;
        float elapsed = 0f;
        Vector3 originalScale = tile.transform.localScale;
        while (elapsed < effectDuration)
        {
            if (tile == null) yield break;
            tile.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, elapsed / effectDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (tile != null) tile.transform.localScale = Vector3.zero;

        if (destroyEffectPrefab != null && tile != null)
        {
            GameObject effect = Object.Instantiate(destroyEffectPrefab, tile.transform.position, Quaternion.identity);
            Object.Destroy(effect, 1f);
        }
        if (tile != null)
        {
            tiles[tile.x, tile.y] = null;
            Object.Destroy(tile.gameObject);
            if (scoreManager != null)
                scoreManager.AddScore(10);
        }
    }
} 