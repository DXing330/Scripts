using System.IO;
using System.Linq;
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
    public List<int> ridingMoveCosts;
    public List<int> swimmingMoveCosts;
    public List<int> scoutingMoveCosts;
    // Occupied tiles adjust move cost.
    public List<int> occupiedTiles;
    private int fullSize;
    private int bigInt = 999999;
    public List<int> adjacentTiles;
    public List<int> tempAdjTiles;
    public List<int> adjTracker;
    public List<int> distances;
    // Shortest path tree.
    public List<int> checkedTiles;
    // Reachable tiles.
    public List<int> reachableTiles;
    public List<int> attackableTiles;

    public void SetTerrainInfo(List<int> newTerrain, int size, List<int> newOccupied)
    {
        UpdateOccupiedTiles(newOccupied);
        terrainInfo = newTerrain;
        fullSize = size;
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
        moveCostList.Clear();
        flyingMoveCosts.Clear();
        ridingMoveCosts.Clear();
        swimmingMoveCosts.Clear();
        scoutingMoveCosts.Clear();
        for (int i = 0; i < fullSize * fullSize; i++)
        {
            moveCostList.Add(terrainTile.ReturnMoveCost(terrainInfo[i], newOccupied[i]));
            flyingMoveCosts.Add(terrainTile.ReturnFlyingMoveCost(terrainInfo[i], newOccupied[i]));
            ridingMoveCosts.Add(terrainTile.ReturnRidingMoveCost(terrainInfo[i], newOccupied[i]));
            swimmingMoveCosts.Add(terrainTile.ReturnSwimmingMoveCost(terrainInfo[i], newOccupied[i]));
            scoutingMoveCosts.Add(terrainTile.ReturnScoutingMoveCost(terrainInfo[i], newOccupied[i]));
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
        RecurviseAdjacency(closestTile);
        for (int i = 0; i < adjacentTiles.Count; i++)
        {
            // If the cost to move to the path from this tile is less than what we've already recorded;
            // Based on movement type check a different list.
            int moveCost = 0;
            switch (type)
            {
                case -1:
                    moveCost = 1;
                    break;
                case 0:
                    moveCost = moveCostList[adjacentTiles[i]];
                    break;
                case 1:
                    moveCost = flyingMoveCosts[adjacentTiles[i]];
                    break;
                case 2:
                    moveCost = ridingMoveCosts[adjacentTiles[i]];
                    break;
                case 3:
                    moveCost = swimmingMoveCosts[adjacentTiles[i]];
                    break;
                case 4:
                    moveCost = scoutingMoveCosts[adjacentTiles[i]];
                    break;
            }
            if (distances[closestTile]+moveCost < distances[adjacentTiles[i]])
            {
                // Then update the distance and the previous tile.
                distances[adjacentTiles[i]] = distances[closestTile]+moveCost;
                savedPathList[adjacentTiles[i]] = closestTile;
                heap.AddNodeWeight(adjacentTiles[i], distances[adjacentTiles[i]]);
            }
        }
    }

    // O(1);
    private List<int> AdjacentFromIndex(int location)
    {
        tempAdjTiles.Clear();
        if (location%fullSize > 0)
        {
            tempAdjTiles.Add(location-1);
        }
        if (location%fullSize < fullSize - 1)
        {
            tempAdjTiles.Add(location+1);
        }
        if (location < (fullSize - 1) * fullSize)
        {
            tempAdjTiles.Add(location+fullSize);
        }
        if (location > fullSize - 1)
        {
            tempAdjTiles.Add(location-fullSize);
        }
        return tempAdjTiles;
    }

    // Hard code test, come back to this later.
    private List<int> AdjacentWithinRange(int location, int range)
    {
        tempAdjTiles.Clear();
        if (location%fullSize > range - 1)
        {
            tempAdjTiles.Add(location-range);
        }
        if (location%fullSize < fullSize - range - 1)
        {
            tempAdjTiles.Add(location+range);
        }
        if (location < (fullSize - range - 1) * fullSize)
        {
            tempAdjTiles.Add(location+(fullSize*range));
        }
        if (location > fullSize - range - 1)
        {
            tempAdjTiles.Add(location-(fullSize*range));
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

    private List<int> FindTilesInRange(int start, int range, int moveType = -1)
    {
        reachableTiles.Clear();
        ResetDistances(start);
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

    public List<int> FindTilesInMoveRange(TacticActor actor, bool current = false)
    {
        reachableTiles.Clear();
        int startIndex = actor.locationIndex;
        int range = actor.ReturnMaxPossibleDistance(current);
        int moveType = actor.movementType;
        if (range <= 0)
        {
            return reachableTiles;
        }
        return FindTilesInRange(startIndex, range, moveType);
    }

    public List<int> FindTilesInAttackRange(TacticActor currentActor, bool currentTurn = true)
    {
        int startIndex = currentActor.locationIndex;
        int moveRange = currentActor.MaxMovementWhileAttacking(currentTurn);
        int moveType = currentActor.movementType;
        int attackRange = currentActor.currentAttackRange;
        attackableTiles.Clear();
        reachableTiles.Clear();
        if (attackRange <= 0)
        {
            return reachableTiles;
        }
        ResetDistances(startIndex);
        // Check what tiles you can move to.
        int distance = 0;
        while (distance <= moveRange && reachableTiles.Count < fullSize * fullSize)
        {
            distance = heap.PeekWeight();
            if (distance > moveRange)
            {
                break;
            }
            CheckClosestTile(false, moveType);
        }
        // Check what tiles you can attack based on the tiles you can move to.
        // O(n).
        for (int i = 0; i < reachableTiles.Count; i++)
        {
            RecurviseAdjacency(reachableTiles[i], attackRange);
            attackableTiles.AddRange(adjacentTiles.Except(attackableTiles));
        }
        attackableTiles.Remove(startIndex);
        return attackableTiles;
    }

    public List<int> FindTilesInSkillRange(TacticActor skillUser, int skillRange)
    {
        reachableTiles.Clear();
        int startIndex = skillUser.locationIndex;
        int moveType = -1;
        if (skillRange <= 0)
        {
            return reachableTiles;
        }
        return FindTilesInRange(startIndex, skillRange, moveType);
    }

    public List<int> FindTilesInSkillSpan(int center, int span)
    {
        reachableTiles.Clear();
        reachableTiles.Add(center);
        int moveType = -1;
        if (span <= 0)
        {
            return reachableTiles;
        }
        return FindTilesInRange(center, span, moveType);
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

    public TacticActor FindNearestEnemy(TacticActor currentActor, List<TacticActor> allActors)
    {
        int location = currentActor.locationIndex;
        int distance = bigInt;
        int targetIndex = 0;
        for (int i = 0; i < allActors.Count; i++)
        {
            if (allActors[i].team != currentActor.team)
            {
                int distanceToEnemy = CalculateDistance(location, allActors[i].locationIndex);
                if (distanceToEnemy < distance)
                {
                    distance = distanceToEnemy;
                    targetIndex = i;
                }
            }
        }
        return allActors[targetIndex];
    }

    public TacticActor FindClosestEnemyInAttackRange(TacticActor currentActor, List<TacticActor> allActors)
    {
        int targetIndex = -1;
        int targetDistance = bigInt;
        int currentDistance = bigInt;
        int location = currentActor.locationIndex;
        FindTilesInAttackRange(currentActor);
        for (int i = 0; i < allActors.Count; i++)
        {
            if (allActors[i].team == currentActor.team)
            {
                continue;
            }
            if (attackableTiles.Contains(allActors[i].locationIndex))
            {
                currentDistance = CalculateDistance(location, allActors[i].locationIndex);
                if (currentDistance < targetDistance)
                {
                    targetIndex = i;
                    targetDistance = currentDistance;
                }
            }
        }
        if (targetIndex >= 0)
        {
            return allActors[targetIndex];
        }
        return null;
    }
}
