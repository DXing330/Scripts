using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public bool hex = true;
    public bool square = true;
    protected int startIndex;
    public int gridSize = 9;
    // Used for square maps.
    protected int fullSize;
    // Used for rectangular maps.
    public int totalRows;
    public int totalColumns;
    public int cornerColumn;
    public int cornerRow;
    public List<Sprite> tileSprites;
    public List<string> allTiles;
    public List<int> currentTiles;
    public List<string> tempTiles;
    public List<TerrainTile> terrainTiles;
    public TerrainPathfinder pathfinder;

    protected virtual void Start()
    {
    }

    public void SetTotalRowsColumns(int rows = -1, int columns = -1)
    {
        if (rows < 0 || columns < 0)
        {
            totalRows = fullSize;
            totalColumns = fullSize;
            return;
        }
        totalRows = rows;
        totalColumns = columns;
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
        if (hex)
        {
            int startRow = GetRow(start);
            int startColumn = GetColumn(start);
            cornerRow = startRow - (gridSize/2);
            cornerColumn = startColumn - (gridSize/2);
        }
        if (!hex)
        {
            cornerRow = -(gridSize/2);
            cornerColumn = -(gridSize/2);
            while (start >= totalColumns)
            {
                start -= totalColumns;
                cornerRow++;
            }
            cornerColumn += start;
        }
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
            if (tileType == 7)
            {
                terrainTiles[imageIndex].UpdateColor(0);
                terrainTiles[imageIndex].UpdateTileImage(tileSprites[0]);
                terrainTiles[imageIndex].UpdateLocationImage(tileSprites[tileType]);
                return;
            }
            terrainTiles[imageIndex].UpdateColor(tileType);
            terrainTiles[imageIndex].UpdateTileImage(tileSprites[tileType]);
        }
    }

    public virtual void ClickOnTile(int tileNumber)
    {
        Debug.Log(tileNumber);
        for (int i = 0; i < terrainTiles.Count; i++)
        {
            terrainTiles[i].SetTileText(pathfinder.CalculateDistance(tileNumber, i).ToString());
        }
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

    public virtual void MoveMap(int direction)
    {
        int previousIndex = startIndex;
        int previousColumn = GetColumn(previousIndex);
        int previousRow = GetRow(previousIndex);
        switch (direction)
        {
            case -1:
                UpdateCenterTile();
                break;
            case 0:
                if (previousRow <= 0){break;}
                startIndex-=totalColumns;
                break;
            case 1:
                if (previousColumn >= totalColumns - 2){break;}
                startIndex += 2;
                break;
            case 2:
                if (previousColumn >= totalColumns - 2){break;}
                startIndex += 2;
                break;
            case 3:
                if (previousRow>=totalRows-1)
                {
                    break;
                }
                startIndex+=totalColumns;
                break;
            case 4:
                if (previousColumn <= 1){break;}
                startIndex -= 2;
                break;
            case 5:
                if (previousColumn <= 1){break;}
                startIndex -= 2;
                break;
        }
        DetermineCornerRowColumn();
        DetermineCurrentTiles();
    }

    protected virtual void UpdateCenterTile(int tileNumber = -1)
    {
        if (tileNumber < 0)
        {
            if (totalRows%2 == 1)
            {
                startIndex = (totalRows*totalColumns/2);
            }
            else
            {
                startIndex = (totalColumns/2)+(totalRows*totalColumns/2);
            }
            // Main idea, go to the middle row, middle column.
        }
        else{startIndex = tileNumber;}
        DetermineCornerRowColumn();
        DetermineCurrentTiles();
    }

    protected virtual void AddRow()
    {
        // Just add a new row at the bottom.
        for (int i = 0; i < totalColumns; i++)
        {
            allTiles.Add("0");
        }
        totalRows++;
    }

    protected virtual void RemoveRow()
    {
        if (totalRows <= gridSize){return;}
        // Remove the last row.
        for (int i = 0; i < totalColumns; i++)
        {
            allTiles.RemoveAt(allTiles.Count - 1);
        }
        totalRows--;
    }

    protected virtual void AddColumns()
    {
        // Add two columns at once to ensure balance.
        // Adding two doesn't make it balanced, try adding four at a time.
        int index = 0;
        for (int i = 0; i < totalRows; i++)
        {
            for (int j = 0; j < totalColumns+4; j++)
            {
                if (j >= totalColumns)
                {
                    tempTiles.Add("0");
                    continue;
                }
                tempTiles.Add(allTiles[index]);
                index++;
            }
        }
        totalColumns += 4;
        allTiles = new List<string>(tempTiles);
    }

    protected virtual void RemoveColumns()
    {
        if (totalColumns <= gridSize){return;}
        int index = 0;
        for (int i = 0; i < totalRows; i++)
        {
            for (int j = 0; j < totalColumns; j++)
            {
                if (j >= totalColumns - 4)
                {
                    index++;
                    continue;
                }
                tempTiles.Add(allTiles[index]);
                index++;
            }
        }
        totalColumns -= 4;
        allTiles = new List<string>(tempTiles);
    }

    public virtual void AdjustRows(bool increase = true)
    {
        tempTiles.Clear();
        if (increase)
        {
            AddRow();
        }
        else
        {
            RemoveRow();
        }
        UpdateCenterTile();
    }

    public virtual void AdjustColumns(bool increase = true)
    {
        tempTiles.Clear();
        if (increase)
        {
            AddColumns();
        }
        else
        {
            RemoveColumns();
        }
        UpdateCenterTile();
    }
}
