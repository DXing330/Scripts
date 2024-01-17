using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPathfinder : MonoBehaviour
{
    // Pathfinder stuff.
    protected int bigInt = 999999;
    public Utility heap;
    protected void ResetHeap()
    {
        heap.ResetHeap();
        heap.InitialCapacity(totalRows * totalColumns);
    }
    public List<int> distances;
    // Stores the previous tile on the optimal path for each tile.
    public List<int> savedPathList;
    public List<int> checkedTiles;
    public List<int> adjTracker;
    public List<int> tempAdjTiles;
    public List<int> adjacentTiles;
    protected List<int> AdjacentFromIndex(int location)
    {
        tempAdjTiles.Clear();
        int currentRow = GetRow(location);
        int currentColumn = GetColumn(location);
        // The top and bottom row have some special cases.
        if (currentRow < totalRows - 1 && currentRow > 0)
        {
            tempAdjTiles.Add(location+totalColumns);
            tempAdjTiles.Add(location-totalColumns);
            if (currentColumn < totalColumns - 1 && currentColumn > 0)
            {
                tempAdjTiles.Add(location+1);
                tempAdjTiles.Add(location-1);
                if (currentColumn%2 == 1)
                {
                    tempAdjTiles.Add(location+totalColumns+1);
                    tempAdjTiles.Add(location+totalColumns-1);
                }
                else if (currentColumn%2 == 0)
                {
                    tempAdjTiles.Add(location-totalColumns+1);
                    tempAdjTiles.Add(location-totalColumns-1);
                }
            }
            else if (currentColumn == 0)
            {
                tempAdjTiles.Add(location+1);
                tempAdjTiles.Add(location-totalColumns+1);
            }
            else if (currentColumn == totalColumns - 1)
            {
                tempAdjTiles.Add(location-1);
                tempAdjTiles.Add(location-totalColumns-1);
            }
        }
        if (currentRow == 0)
        {
            tempAdjTiles.Add(location+totalColumns);
            // Corner cases.
            if (currentColumn == 0)
            {
                tempAdjTiles.Add(location + 1);
            }
            else if (currentColumn == totalColumns - 1)
            {
                tempAdjTiles.Add(location - 1);
            }
            else
            {
                tempAdjTiles.Add(location + 1);
                tempAdjTiles.Add(location - 1);
                if (currentColumn%2 == 1)
                {
                    tempAdjTiles.Add(location + totalColumns + 1);
                    tempAdjTiles.Add(location + totalColumns - 1);
                }
            }
        }
        if (currentRow == totalRows - 1)
        {
            tempAdjTiles.Add(location-totalColumns);
            // Corner cases.
            if (currentColumn == 0)
            {
                tempAdjTiles.Add(location + 1);
                tempAdjTiles.Add(location - totalColumns + 1);
            }
            else if (currentColumn == totalColumns - 1)
            {
                tempAdjTiles.Add(location - 1);
                tempAdjTiles.Add(location - totalColumns - 1);
            }
            else
            {
                tempAdjTiles.Add(location + 1);
                tempAdjTiles.Add(location - 1);
                if (currentColumn%2 == 0)
                {
                    tempAdjTiles.Add(location - totalColumns + 1);
                    tempAdjTiles.Add(location - totalColumns - 1);
                }
            }
        }
        return tempAdjTiles;
    }
    public void RecurviseAdjacency(int location, int range = 1)
    {
        adjacentTiles.Clear();
        if (range <= 0)
        {
            return;
        }
        adjacentTiles = new List<int>(AdjacentFromIndex(location));
        if (range == 1)
        {
            return;
        }
        adjTracker = new List<int>(adjacentTiles);
        for (int i = 0; i < range - 1; i++)
        {
            if (i > 0)
            {
                // Only add newly added tiles.
                adjTracker = new List<int>(adjacentTiles.Except(adjTracker));
            }
            for (int j = 0; j < adjTracker.Count; j++)
            {
                AdjacentFromIndex(adjTracker[j]);
                adjacentTiles.AddRange(tempAdjTiles.Except(adjacentTiles));
            }
        }
    }
    // Can be easily adjusted to return the path to a destination.
    public bool DestReachable(int start, int dest, List<int> newTiles)
    {
        List<int> path = new List<int>();
        ResetHeap();
        checkedTiles.Clear();
        savedPathList.Clear();
        distances.Clear();
        for (int i = 0; i < allTiles.Count; i++)
        {
            savedPathList.Add(-1);
            if (i == start)
            {
                distances.Add(0);
                heap.AddNodeWeight(start, 0);
                continue;
            }
            distances.Add(bigInt);
        }
        for (int i = 0; i < allTiles.Count; i++)
        {
            CheckTile();
            if (checkedTiles.Contains(dest))
            {
                break;
            }
        }
        if (!checkedTiles.Contains(dest)){return false;}
        // Check if there distance is valid.
        int totalDistance = 0;
        int pathIndex = dest;
        while (pathIndex != start)
        {
            path.Add(pathIndex);
            pathIndex = savedPathList[pathIndex];
            totalDistance += distances[pathIndex];
        }
        if (totalDistance >= bigInt){return false;}
        return true;
    }
    protected virtual void CheckTile()
    {
        int moveCost = 1;
        int closestTile = heap.Pull();
        checkedTiles.Add(closestTile);
        if (closestTile < 0){return;}
        RecurviseAdjacency(closestTile);
        for (int i = 0; i < adjacentTiles.Count; i++)
        {

            moveCost = 1;
            if (allTiles[adjacentTiles[i]] == "2")
            {
                moveCost = bigInt;
            }
            if (distances[closestTile]+moveCost < distances[adjacentTiles[i]])
            {
                distances[adjacentTiles[i]] = distances[closestTile]+moveCost;
                savedPathList[adjacentTiles[i]] = closestTile;
                heap.AddNodeWeight(adjacentTiles[i], distances[adjacentTiles[i]]);
            }
        }
    }
    // Map info stuff.
    public int totalRows;
    public int totalColumns;
    public List<string> allTiles;

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
