using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldPathfinder : MonoBehaviour
{
    // Raw terrain types.
    private List<string> terrainInfo;
    // Stores the move cost for each tile.
    private int fullSize;

    public void SetTerrainInfo(List<string> newTerrain, int size)
    {
        terrainInfo = newTerrain;
        fullSize = size;
    }

    // Basic idea, check if there is a mountain between you and the point.
    public bool Reachable(int startIndex, int destIndex)
    {
        if (startIndex == destIndex)
        {
            return true;
        }
        // Just need two corners, upper left and bottom right.
        int rowDist = RowDistance(startIndex, destIndex);
        int columnDist = ColumnDistance(startIndex, destIndex);
        int upperRow = Mathf.Min(GetRow(startIndex), GetRow(destIndex));
        int rightColumn = Mathf.Max(GetColumn(startIndex), GetColumn(destIndex));
        int bottomRow = upperRow + rowDist;
        int leftColumn = rightColumn - columnDist;
        // Iterate through all points in the box.
        int upperCorner = (upperRow * fullSize) + leftColumn;
        for (int i = 0; i < rowDist + 1; i++)
        {
            for (int j = 0; j < columnDist; j++)
            {
                if (terrainInfo[upperCorner] == "2")
                {
                    return false;
                }
                upperCorner++;
            }
            upperCorner -= columnDist;
            upperCorner += fullSize;
        }
        return true;
    }

    private int CalculateDistance(int pointOne, int pointTwo)
    {
        int rowOne = GetRow(pointOne);
        int columnOne = GetColumn(pointOne);
        int rowTwo = GetRow(pointTwo);
        int columnTwo = GetColumn(pointTwo);
        return Mathf.Abs(rowOne-rowTwo)+Mathf.Abs(columnOne-columnTwo);
    }

    private int RowDistance(int pointOne, int pointTwo)
    {
        int rowOne = GetRow(pointOne);
        int rowTwo = GetRow(pointTwo);
        return Mathf.Abs(rowOne-rowTwo);
    }

    private int ColumnDistance(int pointOne, int pointTwo)
    {
        int columnOne = GetColumn(pointOne);
        int columnTwo = GetColumn(pointTwo);
        return Mathf.Abs(columnOne-columnTwo);
    }

    private int GetRow(int location)
    {
        int row = 0;
        while (location >= fullSize)
        {
            location -= fullSize;
            row++;
        }
        return row;
    }

    private int GetColumn(int location)
    {
        return location%fullSize;
    }
}

