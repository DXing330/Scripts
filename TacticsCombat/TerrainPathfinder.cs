using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainPathfinder : MonoBehaviour
{
    public bool hex = false;
    public bool square = true;
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
    // Fullsize is used for square maps where #rows = #columns
    private int fullSize;
    private int totalRows;
    private int totalColumns;
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
    public List<int> possibleTiles;

    public void SetTerrainInfo(List<int> newTerrain, int rows, int columns, List<int> newOccupied)
    {
        UpdateOccupiedTiles(newOccupied);
        terrainInfo = newTerrain;
        totalRows = rows;
        totalColumns = columns;
        if (totalRows == totalColumns)
        {
            square = true;
            fullSize = totalRows;
        }
    }

    private void ResetHeap()
    {
        heap.ResetHeap();
        heap.InitialCapacity(totalRows * totalColumns);
    }

    private void ResetDistances(int startIndex)
    {
        ResetHeap();
        distances.Clear();
        for (int i = 0; i < totalRows * totalColumns; i++)
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
        for (int i = 0; i < totalRows * totalColumns; i++)
        {
            moveCostList.Add(terrainTile.ReturnMoveCost(terrainInfo[i], newOccupied[i]));
            flyingMoveCosts.Add(terrainTile.ReturnFlyingMoveCost(terrainInfo[i], newOccupied[i]));
            ridingMoveCosts.Add(terrainTile.ReturnRidingMoveCost(terrainInfo[i], newOccupied[i]));
            swimmingMoveCosts.Add(terrainTile.ReturnSwimmingMoveCost(terrainInfo[i], newOccupied[i]));
            scoutingMoveCosts.Add(terrainTile.ReturnScoutingMoveCost(terrainInfo[i], newOccupied[i]));
        }
    }

    public bool CheckTileMoveable(TacticActor actor, int location)
    {
        // Can't move into people.
        if (occupiedTiles[location] > 0)
        {
            return false;
        }
        // Can't move into boulders.
        if (terrainInfo[location] == 6 && actor.movementType != 1)
        {
            return false;
        }
        return true;
    }

    public int CheckCurrentLocationType(TacticActor actor)
    {
        return (terrainInfo[actor.locationIndex]);
    }
    
    // Returns a list of tiles to pass through, not including the start or end points, so you will end up adjacent to the destination.
    public List<int> FindPathIndex(int startIndex, int destIndex, int moveType)
    {
        checkedTiles.Clear();
        savedPathList.Clear();
        // Initialize distances and previous tiles.
        ResetDistances(startIndex);
        //ResetHeap();
        // Each loop checks one tile.
        for (int i = 0; i < bigInt; i++)
        {
            CheckClosestTile(true, moveType);
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

    private int DetermineMovementCost(int tile, int moveType)
    {
        int moveCost = 0;
        switch (moveType)
        {
            case -1:
                moveCost = 1;
                break;
            case 0:
                moveCost = moveCostList[tile];
                break;
            case 1:
                moveCost = flyingMoveCosts[tile];
                break;
            case 2:
                moveCost = ridingMoveCosts[tile];
                break;
            case 3:
                moveCost = swimmingMoveCosts[tile];
                break;
            case 4:
                moveCost = scoutingMoveCosts[tile];
                break;
        }
        return moveCost;
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
            int moveCost = DetermineMovementCost(adjacentTiles[i], type);
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
        if (!hex)
        {
            if (location%totalColumns > 0)
            {
                tempAdjTiles.Add(location-1);
            }
            if (location%totalColumns < totalColumns - 1)
            {
                tempAdjTiles.Add(location+1);
            }
            if (location < (totalRows - 1) * totalColumns)
            {
                tempAdjTiles.Add(location+totalColumns);
            }
            if (location > totalColumns - 1)
            {
                tempAdjTiles.Add(location-totalColumns);
            }
        }
        if (hex)
        {
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

    public int DirectionBetweenLocations(int startLocation, int nextLocation)
    {
        for (int i = 0; i < 6; i++)
        {
            if (GetDestination(startLocation, i) == nextLocation)
            {
                return i;
            }
        }
        return -1;
    }

    public bool DirectionCheck(int location, int direction)
    {
        if (hex)
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
        }
        else
        {
            switch (direction)
            {
                // Up.
                case 0:
                    return (location > totalColumns - 1);
                // Right.
                case 1:
                    return (location%totalColumns < totalColumns - 1);
                // Down.
                case 2:
                    return (location < (totalRows - 1) * totalColumns);
                // Left.
                case 3:
                    return (location%totalColumns > 0);
            }
        }
        return false;   
    }

    public bool FaceOffCheck(int directionOne, int directionTwo)
    {
        if (hex)
        {
            if ((directionOne+3)%6 == directionTwo){return true;}
            else if ((directionOne+2)%6 == directionTwo){return true;}
            else if ((directionOne+4)%6 == directionTwo){return true;}
            return false;
        }
        if (!hex)
        {
            if ((directionOne+2)%4 == directionTwo){return true;}
            return false;
        }
        // For ranged attacks direction facing doesn't matter.
        // Since ranged doesn't have enough nerfs yet lel.
        return true;
    }

    public int SameLine(int locOne, int locTwo)
    {
        if (!hex)
        {
            // 0 = same row, 1 = same column, -1 = neither
            int rowOne = GetRow(locOne);
            int rowTwo = GetRow(locTwo);
            if (rowOne == rowTwo)
            {
                return 0;
            }
            int colOne = GetColumn(locOne);
            int colTwo = GetColumn(locTwo);
            if (colOne == colTwo)
            {
                return 1;
            }
        }
        if (hex)
        {
            // 1 = same column, 2 = same diagonal, -1 = neither
            int QOne = GetHexQ(locOne);
            int QTwo = GetHexQ(locTwo);
            if (QOne == QTwo){return 1;}
            int ROne = GetHexR(locOne);
            int RTwo = GetHexR(locTwo);
            if (ROne == RTwo){return 2;}
            int SOne = GetHexS(locOne);
            int STwo = GetHexS(locTwo);
            if (SOne == STwo){return 3;}
        }
        return -1;
    }

    public bool Left(int locOne, int locTwo)
    {
        return (GetColumn(locOne) < GetColumn(locTwo));
    }

    public bool Up(int locOne, int locTwo)
    {
        return (GetRow(locOne) < GetRow(locTwo));
    }

    // Same R axis, check if one is higher than the other.
    public bool UpR(int locOne, int locTwo)
    {
        return (GetHexQ(locOne) < GetHexQ(locTwo));
    }

    // Same S acis, check is one is higher than the other.
    public bool UpS(int locOne, int locTwo)
    {
        return (GetHexQ(locOne) > GetHexQ(locTwo));
    }

    public int GetDestination(int location, int direction)
    {
        if (!hex)
        {
            switch (direction)
            {
                // Up.
                case 0:
                    return (location-totalColumns);
                // Right.
                case 1:
                    return (location+1);
                // Down.
                case 2:
                    return (location+totalColumns);
                // Left.
                case 3:
                    return (location-1);
            }
        }
        if (hex)
        {
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
        }
        return location;
    }

    private List<int> FindTilesInRange(int start, int range, int moveType = -1)
    {
        reachableTiles.Clear();
        ResetDistances(start);
        int distance = 0;
        while (distance <= range && reachableTiles.Count < totalColumns * totalRows)
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
        while (distance <= moveRange && reachableTiles.Count < totalColumns * totalRows)
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

    public List<int> FindTilesInLineRange(TacticActor actor, int range)
    {
        reachableTiles.Clear();
        for (int i = 0; i < 6; i++)
        {
            // Start wherever the actor is.
            int start = actor.locationIndex;
            for (int k = 0; k < range; k++)
            {
                // Check if you can add the tile in the direction.
                if (DirectionCheck(start, i))
                {
                    // If you can then add it and check if you can keep adding.
                    start = GetDestination(start, i);
                    reachableTiles.Add(start);
                }
            }
        }
        return reachableTiles;
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
        if (!hex)
        {
            int rowOne = GetRow(pointOne);
            int columnOne = GetColumn(pointOne);
            int rowTwo = GetRow(pointTwo);
            int columnTwo = GetColumn(pointTwo);
            return Mathf.Abs(rowOne-rowTwo)+Mathf.Abs(columnOne-columnTwo);
        }
        else
        {
            return (Mathf.Abs(GetHexQ(pointOne)-GetHexQ(pointTwo))+Mathf.Abs(GetHexR(pointOne)-GetHexR(pointTwo))+Mathf.Abs(GetHexS(pointOne)-GetHexS(pointTwo)))/2;
        }
    }

    public int CalculateDistanceToLocation(TacticActor actor, int destination)
    {
        int distance = 0;
        FindPathIndex(actor.locationIndex, destination, actor.movementType);
        for (int i = 0; i < actualPath.Count; i++)
        {
            int addedDist = DetermineMovementCost(actualPath[i], actor.movementType);
            distance += addedDist;
        }
        return distance;
    }

    public int CalculateDirectionToLocation(TacticActor actor, int destination)
    {
        FindPathIndex(actor.locationIndex, destination, actor.movementType);
        if (actualPath.Count == 1)
        {
            return DirectionBetweenLocations(actor.locationIndex, destination);
        }
        else
        {
            // Actual path starts at the destination and ends at the start.
            // So to find the direction to get to the final destination you consider the tile you were at before the final destination, in this case the second tile.
            return DirectionBetweenLocations(actualPath[1], actualPath[0]);
        }
    }

    private int GetRow(int location)
    {
        int row = 0;
        while (location >= totalColumns)
        {
            location -= totalColumns;
            row++;
        }
        return row;
    }

    private int GetColumn(int location)
    {
        if (totalColumns == 0){return location%fullSize;}
        return location%totalColumns;
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

    public TacticActor FindNearestEnemy(TacticActor currentActor, List<TacticActor> allActors)
    {
        possibleTiles.Clear();
        int location = currentActor.locationIndex;
        int distance = bigInt;
        for (int i = 0; i < allActors.Count; i++)
        {
            if (allActors[i].team != currentActor.team)
            {
                int distanceToEnemy = CalculateDistance(location, allActors[i].locationIndex);
                if (distanceToEnemy < distance)
                {
                    possibleTiles.Clear();
                    possibleTiles.Add(i);
                    distance = distanceToEnemy;
                }
                else if (distanceToEnemy == distance)
                {
                    possibleTiles.Add(i);
                }
            }
        }
        if (possibleTiles.Count <= 0){return null;}
        int rng = Random.Range(0, possibleTiles.Count);
        return allActors[possibleTiles[rng]];
    }

    public TacticActor FindClosestEnemyInAttackRange(TacticActor currentActor, List<TacticActor> allActors)
    {
        possibleTiles.Clear();
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
                    possibleTiles.Clear();
                    possibleTiles.Add(i);
                    targetDistance = currentDistance;
                }
                else if (currentDistance == targetDistance)
                {
                    possibleTiles.Add(i);
                }
            }
        }
        if (possibleTiles.Count > 0)
        {
            int rng = Random.Range(0, possibleTiles.Count);
            return allActors[possibleTiles[rng]];
        }
        return null;
    }

    public int FindFurthestTileFromTarget(TacticActor currentActor, TacticActor target)
    {
        // Keep track of options.
        possibleTiles.Clear();
        // Get the reachable tiles.
        int awayFrom = target.locationIndex;
        int destination = currentActor.locationIndex;
        int distance = CalculateDistance(awayFrom, destination);
        int tempDist = 0;
        FindTilesInMoveRange(currentActor, true);
        for (int i = 0; i < reachableTiles.Count; i++)
        {
            tempDist = CalculateDistance(awayFrom, reachableTiles[i]);
            // Run as far as possible.
            if (tempDist > distance)
            {
                distance = tempDist;
                possibleTiles.Clear();
                possibleTiles.Add(reachableTiles[i]);
            }
            if (tempDist == distance)
            {
                possibleTiles.Add(reachableTiles[i]);
            }
        }
        if (possibleTiles.Count > 0)
        {
            int rng = Random.Range(0, possibleTiles.Count);
            return possibleTiles[rng];
        }
        return destination;
    }
}
