using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerClickHandler
{
    public int x;
    public int y;
    private SpriteRenderer spriteRenderer;
    private static Tile selectedTile = null;
    private static BoardManager boardManager;
    private Color originalColor;
    public Color selectedColor = Color.yellow;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        if (boardManager == null)
            boardManager = FindObjectOfType<BoardManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (selectedTile == null)
        {
            Select();
        }
        else if (selectedTile == this)
        {
            Deselect();
        }
        else
        {
            if (IsAdjacent(selectedTile))
            {
                boardManager.SwapTiles(this, selectedTile);
            }
            selectedTile.Deselect();
            Deselect();
        }
    }

    public void Select()
    {
        selectedTile = this;
        spriteRenderer.color = selectedColor;
    }

    public void Deselect()
    {
        spriteRenderer.color = originalColor;
        if (selectedTile == this)
            selectedTile = null;
    }

    public bool IsAdjacent(Tile other)
    {
        int dx = Mathf.Abs(this.x - other.x);
        int dy = Mathf.Abs(this.y - other.y);
        return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
    }

    public void SetPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
} 