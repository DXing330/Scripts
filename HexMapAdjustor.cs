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
                tiles[tileIndex].SetTileText("("+i+","+j+")");
                tileIndex++;
                xPivot += 1f/(gridSize - 1);
            }
            yCenter -= 1f/(gridSize);
        }
    }


}
