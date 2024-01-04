using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMapAdjustor : MonoBehaviour
{
    public List<HexTerrainTile> tiles;
    public int gridSize = 9;
    public bool flatTop = true;

    void Start()
    {
        InitializeTiles();
    }

    protected virtual void InitializeTiles()
    {
        int tileIndex = 0;
        float scale = 1f/(gridSize);
        float xPivot = 0f;
        float yCenter = 1f - (1f/(2*gridSize));
        float yPivot = 1f;
        for (int i = 0; i < gridSize; i++)
        {
            xPivot = 0f;
            for (int j = 0; j < gridSize; j++)
            {
                if (j%2 == 0)
                {
                    yPivot = yCenter + 1f/(4*gridSize);
                }
                else
                {
                    yPivot = yCenter - 1f/(4*gridSize);
                }
                tiles[tileIndex].UpdatePivot(xPivot, yPivot);
                tiles[tileIndex].SetTileText("("+GetHexQ(tileIndex)+","+GetHexR(tileIndex)+","+GetHexS(tileIndex)+")");
                tileIndex++;
                xPivot += 1f/(gridSize - 1);
            }
            yCenter -= 1f/(gridSize);
        }
    }

    private int GetRow(int location)
    {
        int row = 0;
        while (location >= gridSize)
        {
            location -= gridSize;
            row++;
        }
        return row;
    }

    private int GetColumn(int location)
    {
        return location%gridSize;
    }

    private int GetHexQ(int location)
    {
        return GetColumn(location);
    }

    private int GetHexR(int location)
    {
        return GetRow(location) - (GetColumn(location) - (GetColumn(location)%2)) / 2;
    }

    private int GetHexS(int location)
    {
        return -GetHexQ(location)-GetHexR(location);
    }

}
