using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPathfinder : MonoBehaviour
{
    public int totalRows;
    public int totalColumns;
    public List<string> allTiles;
    public List<int> currentTiles;

    public void SetAllTiles(List<string> newTiles)
    {
        allTiles = new List<string>(newTiles);
    }

    public void SetTotalRowsColumns(int rows, int columns)
    {
        totalRows = rows;
        totalColumns = columns;
    }

    protected int GetRow(int locationIndex)
    {
        int row = 0;
        while (locationIndex >= totalColumns)
        {
            locationIndex -= totalColumns;
            row++;
        }
        return row;
    }

    protected int GetColumn(int locationIndex)
    {
        return locationIndex%totalColumns;
    }

    protected int GetHexQ(int location)
    {
        return GetColumn(location);
    }

    protected int GetHexR(int location)
    {
        return GetRow(location) - (GetColumn(location) - (GetColumn(location)%2)) / 2;
    }

    protected int GetHexS(int location)
    {
        return -GetHexQ(location)-GetHexR(location);
    }

    public virtual int CalculateDistance(int pointOne, int pointTwo)
    {
        return (Mathf.Abs(GetHexQ(pointOne)-GetHexQ(pointTwo))+Mathf.Abs(GetHexR(pointOne)-GetHexR(pointTwo))+Mathf.Abs(GetHexS(pointOne)-GetHexS(pointTwo)))/2;
    }

    public virtual bool DirectionCheck(int location, int direction)
    {
        switch (direction)
        {
            // Up.
            case 0:
                return (GetRow(location) > 0);
            // UpRight.
            case 1:
                if (GetColumn(location) == totalColumns - 1){return false;}
                if (GetRow(location) == 0 && GetColumn(location)%2 == 0){return false;}
                return true;
            // DownRight.
            case 2:
                if (GetColumn(location) == totalColumns - 1){return false;}
                if (GetRow(location) == totalRows - 1 && GetColumn(location)%2 == 1){return false;}
                return true;
            // Down.
            case 3:
                return (GetRow(location) < totalRows - 1);
            // DownLeft.
            case 4:
                if (GetColumn(location) == 0){return false;}
                if (GetRow(location) == totalRows - 1 && GetColumn(location)%2 == 1){return false;}
                return true;
            // UpLeft.
            case 5:
                if (GetColumn(location) == 0){return false;}
                if (GetRow(location) == 0 && GetColumn(location)%2 == 0){return false;}
                return true;
        }
        return false;
    }

    public virtual int GetDestination(int location, int direction)
    {
        if (!DirectionCheck(location, direction)){return location;}
        switch (direction)
        {
            // Up.
            case 0:
                return location - totalColumns;
            // UpRight.
            case 1:
                if (GetColumn(location)%2 == 1)
                {
                    return location + 1;
                }
                return (location - totalColumns + 1);
            // DownRight.
            case 2:
                if (GetColumn(location)%2 == 0)
                {
                    return location + 1;
                }
                return (location + totalColumns + 1);
            // Down.
            case 3:
                return location + totalColumns;
            // DownLeft.
            case 4:
                if (GetColumn(location)%2 == 0)
                {
                    return location - 1;
                }
                return (location + totalColumns - 1);
            // UpLeft.
            case 5:
                if (GetColumn(location)%2 == 1)
                {
                    return location - 1;
                }
                return (location - totalColumns - 1);
        }
        return location;
    }
}
