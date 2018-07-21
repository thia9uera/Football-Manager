using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardView : MonoBehaviour
{
    [SerializeField]
    private BoardTileView tileTemplate;

    public int HorizontalTiles = 7;
    public int VerticalTiles = 10;
    public int TileSize = 64;

    public void Start()
    {
       PopulateBoard();
    }

    public void PopulateBoard()
    {
        for (int v = 0; v < VerticalTiles; v++)
        {
            for (int h = 0; h < HorizontalTiles; h++ )
            {
                BoardTileView tile = Instantiate(tileTemplate, transform);
                tile.transform.localPosition = new Vector2(h * TileSize,-( v * TileSize));
                if ((h - v) % 2 == 0) tile.ChangeColor();
            }
        }
    }
}
