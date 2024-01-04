using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public bool square = true;
    protected int startIndex;
    public int gridSize = 7;
    // Used for square maps.
    protected int fullSize;
    // Used for rectangular maps.
    protected int totalRows;
    protected int totalColumns;
    protected int cornerColumn;
    protected int cornerRow;
    public List<Sprite> tileSprites;
    public List<string> allTiles;
    public List<int> currentTiles;
    public List<TerrainTile> terrainTiles;

    protected virtual void Start()
    {
        //InitializeTiles();
        SetTotalRowsColumns(fullSize, fullSize);
    }

    public void SetTotalRowsColumns(int rows, int columns)
    {
        if (rows == columns){square = true;}
        totalRows = rows;
        totalColumns = columns;
        fullSize  = rows;
    }

    protected virtual void InitializeTiles()
    {
        int tileIndex = 0;
        float scale = 1f/gridSize;
        float xPivot = 0f;
        float yPivot = 1f;
        for (int i = 0; i < gridSize; i++)
        {
            xPivot = 0f;
            for (int j = 0; j < gridSize; j++)
            {
                terrainTiles[tileIndex].UpdatePivot(xPivot, yPivot);
                terrainTiles[tileIndex].UpdateSize(scale);
                tileIndex++;
                xPivot += 1f/(gridSize - 1);
            }
            yPivot -= 1f/(gridSize - 1);
        }
        //need to put the rest of the tiles somewhere invisible
    }

    protected virtual void DetermineCurrentTiles()
    {
        currentTiles.Clear();
        int cColumn = 0;
        int cRow = 0;
        // This doesn't depend on fullSize.
        for (int i = 0; i < gridSize * gridSize; i++)
        {
            AddCurrentTile(cRow + cornerRow, cColumn + cornerColumn);
            cColumn++;
            if (cColumn >= gridSize)
            {
                cColumn -= gridSize;
                cRow++;
            }
        }
    }

    protected virtual void AddCurrentTile(int row, int column)
    {
        if (row < 0 || column < 0 || column >= totalColumns || row >= totalRows)
        {
            currentTiles.Add(-1);
            return;
        }
        currentTiles.Add((row*totalColumns)+column);
    }

    protected virtual void DetermineCornerRowColumn()
    {
        int start = startIndex;
        cornerRow = -(gridSize/2);
        cornerColumn = -(gridSize/2);
        while (start >= totalColumns)
        {
            start -= totalColumns;
            cornerRow++;
        }
        cornerColumn += start;
    }

    protected virtual void UpdateTile(int imageIndex, int tileIndex)
    {
        // Undefined tiles are black.
        if (tileIndex < 0 || tileIndex >= (totalRows * totalColumns))
        {
            terrainTiles[imageIndex].UpdateColor(-1);
        }
        else
        {
            int tileType = int.Parse(allTiles[tileIndex]);
            terrainTiles[imageIndex].UpdateColor(tileType);
            terrainTiles[imageIndex].UpdateTileImage(tileSprites[tileType]);
        }
    }

    public virtual void ClickOnTile(int tileNumber)
    {
        Debug.Log(tileNumber);
    }

    protected int GetRow(int location)
    {
        int row = 0;
        while (location >= totalColumns)
        {
            location -= totalColumns;
            row++;
        }
        return row;
    }

    protected int GetColumn(int location)
    {
        return location%totalColumns;
    }
}
