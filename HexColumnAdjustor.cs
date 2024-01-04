using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexColumnAdjustor : MonoBehaviour
{
    public List<HexTerrainTile> column;
    public bool flatTop = true;
    public bool upper = true;
    private float xPivot = 0.5f;

    void Start()
    {
        InitializeTiles();
    }

    // Leave space for one extra tile, since there will be an offset.
    // So pretend that there is one extra tile.
    // Offset by half of a tile height either up or down.
    protected virtual void InitializeTiles()
    {
        int size = column.Count;
        float yPivot = 1f;
        if (upper)
        {
            yPivot = 1f - (1f/(2*size)) + 1f/(4*size);
        }
        else
        {
            yPivot = 1f - (1f/(2*size)) - 1f/(4*size);
        }
        for (int i = 0; i < size; i++)
        {
            column[i].UpdatePivot(xPivot, yPivot);
            yPivot -= 1f/size;
        }
    }
}
