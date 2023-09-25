using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainPathfinder : MonoBehaviour
{
    public Utility heap;
    public TerrainTile terrainTile;
    // Raw terrain types.
    private List<int> terrainInfo;
    // Stores the previous tile on the optimal path for each tile.
    public List<int> savedPathList;
    // The actual path to the tile.
    public List<int> actualPath;
    // Stores the move cost for each tile.
    public List<int> moveCostList;
    public List<int> flyingMoveCosts;
    // Occupied tiles adjust move cost.
    public List<int> occupiedTiles;
    private int fullSize;
    private int bigInt = 999999;
    public List<int> adjacentTiles;
    public List<int> distances;
    // Shortest path tree.
    public List<int> checkedTiles;
    // Reachable tiles.
    public List<int> reachableTiles;

    public void SetTerrainInfo(List<int> newTerrain, int size, List<int> newOccupied)
    {
        UpdateOccupiedTiles(newOccupied);
        terrainInfo = newTerrain;
        fullSize = size;
        moveCostList.Clear();
        for (int i = 0; i < fullSize * fullSize; i++)
        {
            moveCostList.Add(terrainTile.ReturnMoveCost(terrainInfo[i], newOccupied[i]));
            flyingMoveCosts.Add(terrainTile.ReturnFlyingMoveCost(terrainInfo[i], newOccupied[i]));
        }
    }

    private void ResetHeap()
    {
        heap.Reset();
        heap.InitialCapacity(fullSize * fullSize);
    }

    private void ResetDistances(int startIndex)
    {
        ResetHeap();
        distances.Clear();
        for (int i = 0; i < fullSize * fullSize; i++)
        {
            // At the start no idea what tile leads to what.
            savedPathList.Add(-1);
            if (i == startIndex)
            {
                // Starting tile is always distance zero.
                distances.Add(0);
                heap.AddNodeWeight(startIndex, 0);
                continue;
            }
            // Other tiles are considered far away.
            distances.Add(bigInt);
        }
    }

    public void UpdateOccupiedTiles(List<int> newOccupied)
    {
        occupiedTiles = newOccupied;
        moveCostList.Clear();
        for (int i = 0; i < fullSize * fullSize; i++)
        {
            moveCostList.Add(terrainTile.ReturnMoveCost(terrainInfo[i], newOccupied[i]));
            flyingMoveCosts.Add(terrainTile.ReturnFlyingMoveCost(terrainInfo[i], newOccupied[i]));
        }
    }

    // Returns a list of tiles to pass through, not including the start or end points, so you will end up adjacent to the destination.
    public List<int> FindPathIndex(int startIndex, int destIndex)
    {
        checkedTiles.Clear();
        savedPathList.Clear();
        // Initialize distances and previous tiles.
        ResetDistances(startIndex);
        //ResetHeap();
        // Each loop checks one tile.
        for (int i = 0; i < bigInt; i++)
        {
            CheckClosestTile();
            if (checkedTiles.Contains(destIndex))
            {
                break;
            }
        }
        // Get the actual path to the tile.
        actualPath.Clear();
        int pathIndex = destIndex;
        while (pathIndex != startIndex)
        {
            actualPath.Add(pathIndex);
            // Go backwards through the path until you reach the start.
            //Debug.Log(pathIndex);
            pathIndex = savedPathList[pathIndex];
        }
        return actualPath;
    }

    private void CheckClosestTile(bool path = true, int type = 0)
    {
        // Find the closest tile.
        // This part is where the heap is used making it O(nlgn) instead of O(n^2).
        int closestTile = heap.Pull();
        if (path)
        {
            checkedTiles.Add(closestTile);
        }
        else
        {
            reachableTiles.Add(closestTile);
        }
        AdjacentFromIndex(closestTile);
        for (int i = 0; i < adjacentTiles.Count; i++)
        {
            // If the cost to move to the path from this tile is less than what we've already recorded;
            // Based on movement type check a different list.
            int moveCost = 0;
            switch (type)
            {
                case 0:
                    moveCost = moveCostList[adjacentTiles[i]];
                    break;
                case 1:
                    moveCost = flyingMoveCosts[adjacentTiles[i]];
                    break;
            }
            if (distances[closestTile]+moveCost < distances[adjacentTiles[i]])
            {
                // Then update the distance and the previous tile.
                distances[adjacentTiles[i]] = distances[closestTile]+moveCost;
                //Debug.Log(closestTile);
                //Debug.Log(adjacentTiles[i]);
                savedPathList[adjacentTiles[i]] = closestTile;
                heap.AddNodeWeight(adjacentTiles[i], distances[adjacentTiles[i]]);
            }
        }
    }

    public void AdjacentFromIndex(int location)
    {
        adjacentTiles.Clear();
        if (location%fullSize > 0)
        {
            adjacentTiles.Add(location-1);
        }
        if (location%fullSize < fullSize - 1)
        {
            adjacentTiles.Add(location+1);
        }
        if (location < (fullSize - 1) * fullSize)
        {
            adjacentTiles.Add(location+fullSize);
        }
        if (location > fullSize - 1)
        {
            adjacentTiles.Add(location-fullSize);
        }
    }

    public bool DirectionCheck(int location, int direction)
    {
        switch (direction)
        {
            // Up.
            case 0:
                return (location > fullSize - 1);
            // Right.
            case 1:
                return (location%fullSize < fullSize - 1);
            // Down.
            case 2:
                return (location < (fullSize - 1) * fullSize);
            // Left.
            case 3:
                return (location%fullSize > 0);
        }
        return false;   
    }

    public int GetDestination(int location, int direction)
    {
        switch (direction)
        {
            // Up.
            case 0:
                return (location-fullSize);
            // Right.
            case 1:
                return (location+1);
            // Down.
            case 2:
                return (location+fullSize);
            // Left.
            case 3:
                return (location-1);
        }
        return location;
    }

    public List<int> FindTilesInRange(int startIndex, int range, int moveType = 0)
    {
        reachableTiles.Clear();
        if (range <= 0)
        {
            return reachableTiles;
        }
        ResetDistances(startIndex);
        int distance = 0;
        while (distance <= range && reachableTiles.Count < fullSize * fullSize)
        {
            distance = heap.PeekWeight();
            if (distance > range)
            {
                break;
            }
            CheckClosestTile(false, moveType);
        }
        // Don't include your own tile in the reachable tiles.
        reachableTiles.RemoveAt(0);
        return reachableTiles;
    }

    public int CalculateDistance(int pointOne, int pointTwo)
    {
        int rowOne = GetRow(pointOne);
        int columnOne = GetColumn(pointOne);
        int rowTwo = GetRow(pointTwo);
        int columnTwo = GetColumn(pointTwo);
        return Mathf.Abs(rowOne-rowTwo)+Mathf.Abs(columnOne-columnTwo);
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
