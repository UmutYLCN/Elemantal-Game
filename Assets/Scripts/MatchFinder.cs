using System.Collections.Generic;

public class MatchFinder
{
    public static List<Tile> FindMatches(Tile[,] tiles, int width, int height)
    {
        List<Tile> matchedTiles = new List<Tile>();

        // Yatay kontrol
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width - 2; x++)
            {
                Tile t1 = tiles[x, y];
                Tile t2 = tiles[x + 1, y];
                Tile t3 = tiles[x + 2, y];
                if (t1 != null && t2 != null && t3 != null &&
                    t1.CompareTag(t2.tag) && t1.CompareTag(t3.tag))
                {
                    if (!matchedTiles.Contains(t1)) matchedTiles.Add(t1);
                    if (!matchedTiles.Contains(t2)) matchedTiles.Add(t2);
                    if (!matchedTiles.Contains(t3)) matchedTiles.Add(t3);
                }
            }
        }

        // Dikey kontrol
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height - 2; y++)
            {
                Tile t1 = tiles[x, y];
                Tile t2 = tiles[x, y + 1];
                Tile t3 = tiles[x, y + 2];
                if (t1 != null && t2 != null && t3 != null &&
                    t1.CompareTag(t2.tag) && t1.CompareTag(t3.tag))
                {
                    if (!matchedTiles.Contains(t1)) matchedTiles.Add(t1);
                    if (!matchedTiles.Contains(t2)) matchedTiles.Add(t2);
                    if (!matchedTiles.Contains(t3)) matchedTiles.Add(t3);
                }
            }
        }
        return matchedTiles;
    }
} 