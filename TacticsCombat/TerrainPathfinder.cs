using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainPathfinder : BasicPathfinder
{
    public bool hex = false;
    public bool square = true;
    public TerrainTile terrainTile;
    // Raw terrain types.
    public List<int> terrainInfo;
    // The actual path to the tile.
    public List<int> actualPath;
    // Occupied tiles adjust move cost.
    public List<int> occupiedTiles;
    // Fullsize is used for square maps where #rows = #columns
    private int fullSize;
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

    protected void ResetDistances(int startIndex, int startDirection = -1)
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
                heap.AddNodeWeight(startIndex, 0, startDirection);
                continue;
            }
            // Other tiles are considered far away.
            distances.Add(bigInt);
        }
    }

    public void UpdateOccupiedTiles(List<int> newOccupied)
    {
        occupiedTiles = newOccupied;
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
    public List<int> FindPathIndex(TacticActor actor, int destIndex)
    {
        int startIndex = actor.locationIndex;
        int moveType = actor.movementType;
        checkedTiles.Clear();
        savedPathList.Clear();
        // Initialize distances and previous tiles.
        ResetDistances(startIndex, actor.currentDirection);
        // Each loop checks one tile.
        // O(n^2)
        for (int i = 0; i < bigInt; i++)
        {
            ActorChecksClosestTile(actor, destIndex);
            // O(n)
            if (checkedTiles.Contains(destIndex))
            {
                break;
            }
        }
        // Get the actual path to the tile.
        actualPath.Clear();
        if (savedPathList.Count <= 0){return actualPath;}
        int pathIndex = destIndex;
        int maxMovement = actor.MaxMovePerTurn();
        for (int i = 0; i < maxMovement * maxMovement; i++)
        {
            actualPath.Add(pathIndex);
            pathIndex = savedPathList[pathIndex];
            if (pathIndex == startIndex || pathIndex < 0){break;}
        }
        // Can't reach destination.
        if (pathIndex != startIndex){actualPath.Clear();}
        return actualPath;
    }

    protected int AdjustMoveCostOnDirectionChange(int moveCost, int directionChange = -1)
    {
        if (directionChange >= 0)
        {
            moveCost = moveCost - 1 + directionChange;
            if (moveCost < 1){moveCost = 1;}
        }
        return moveCost;
    }

    public int AdjustActorMoveCostOnDirection(TacticActor actor, int moveCost, int destination)
    {
        if (actor.movementType == 1)
        {
            return moveCost;
        }
        int currentDirection = actor.currentDirection;
        int newDirection = DirectionBetweenLocations(actor.locationIndex, destination);
        int directionChange = ReturnDirectionChange(currentDirection, newDirection);
        return AdjustMoveCostOnDirectionChange(moveCost, directionChange);
    }

    private int ReturnDirectionChange(int directionOne, int directionTwo)
    {
        if (directionOne < 0 || directionTwo < 0){return -1;}
        int change = Mathf.Abs(directionOne - directionTwo);
        if (change == 4){return 2;}
        if (change == 5){return 1;}
        return change;
    }

    protected void CheckClosestTile()
    {
        // Find the closest tile.
        // This part is where the heap is used making it O(nlgn) instead of O(n^2).
        int closestTile = heap.Pull();
        reachableTiles.Add(closestTile);
        RecursiveAdjacency(closestTile);
        for (int i = 0; i < adjacentTiles.Count; i++)
        {
            int moveCost = 1;
            if (distances[closestTile]+moveCost < distances[adjacentTiles[i]])
            {
                // Then update the distance and the previous tile.
                distances[adjacentTiles[i]] = distances[closestTile]+moveCost;
                savedPathList[adjacentTiles[i]] = closestTile;
                heap.AddNodeWeight(adjacentTiles[i], distances[adjacentTiles[i]]);
            }
        }
    }

    protected bool ActorChecksClosestTile(TacticActor actor, int destination = -1, bool path = true)
    {
        int newDirection = -1;
        int directionChange = -1;
        int moveCost = 1;
        int closestDirection = heap.PeekDirection();
        int closestTile = heap.Pull();
        if (closestTile < 0){return false;}
        if (path){checkedTiles.Add(closestTile);}
        else{reachableTiles.Add(closestTile);}
        RecursiveAdjacency(closestTile);
        // Check if all adjacent tiles are occupied.
        int adjacentUnpassable = 0;
        // O(n)
        for (int i = 0; i < adjacentTiles.Count; i++)
        {
            // Need an extra check for occupied tiles.
            if (occupiedTiles[adjacentTiles[i]] > 0 && adjacentTiles[i] != destination)
            {
                moveCost = bigInt;
                adjacentUnpassable++;
                if (adjacentUnpassable >= adjacentTiles.Count - 1){return false;}
            }
            else
            {
                // O(1)
                newDirection = DirectionBetweenLocations(closestTile, adjacentTiles[i]);
                directionChange = ReturnDirectionChange(closestDirection, newDirection);
                moveCost = actor.ReturnMoveCostForTile(terrainInfo[adjacentTiles[i]]);
                if (actor.movementType != 1)
                {
                    moveCost = AdjustMoveCostOnDirectionChange(moveCost, directionChange);
                }
            }
            // This is never true if all adjacent tiles are occupied/unavailable.
            if (distances[closestTile]+moveCost < distances[adjacentTiles[i]])
            {
                // Then update the distance and the previous tile.
                distances[adjacentTiles[i]] = distances[closestTile]+moveCost;
                savedPathList[adjacentTiles[i]] = closestTile;
                heap.AddNodeWeight(adjacentTiles[i], distances[adjacentTiles[i]], newDirection);
            }
        }
        return true;
    }

    public int DirectionBetweenLocations(int startLocation, int nextLocation)
    {
        // Check if they're directly adjacent from each other first.
        for (int i = 0; i < 6; i++)
        {
            if (GetDestination(startLocation, i) == nextLocation)
            {
                return i;
            }
        }
        // Then just check the direction manually.
        int startRow = GetRow(startLocation);
        int nextRow = GetRow(nextLocation);
        int startCol = GetColumn(startLocation);
        int nextCol = GetColumn(nextLocation);
        // First check if they're in a column.
        if (startCol == nextCol)
        {
            // Then either face up.
            if (startRow < nextRow){return 3;}
            // Or down.
            return 0;
        }
        // Otherwise check if left or right.
        else if (startCol < nextCol)
        {
            if (startRow < nextRow){return 2;}
            return 1;
        }
        else
        {
            if (startRow < nextRow){return 4;}
            return 5;
        }
    }

    public override bool DirectionCheck(int location, int direction)
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

    public override int GetDestination(int location, int direction)
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

    private List<int> FindTilesInRange(int start, int range, int moveType = -1, int startDirection = -1)
    {
        reachableTiles.Clear();
        ResetDistances(start, startDirection);
        int distance = 0;
        while (distance <= range && reachableTiles.Count < totalColumns * totalRows)
        {
            distance = heap.PeekWeight();
            if (distance > range)
            {
                break;
            }
            CheckClosestTile();
        }
        // Don't include your own tile in the reachable tiles.
        reachableTiles.RemoveAt(0);
        return reachableTiles;
    }

    public List<int> FindTilesInMoveRange(TacticActor actor, bool current = false)
    {
        reachableTiles.Clear();
        int range = actor.ReturnMaxPossibleDistance(current);
        if (range <= 0){return reachableTiles;}
        int startIndex = actor.locationIndex;
        int moveType = actor.movementType;
        ResetDistances(startIndex, actor.currentDirection);
        int distance = 0;
        for (int i = 0; i < totalColumns * totalRows; i++)
        {
            distance = heap.PeekWeight();
            if (distance > range){break;}
            if(!ActorChecksClosestTile(actor, -1, false)){break;}
        }
        // Don't include your own tile in the reachable tiles.
        reachableTiles.RemoveAt(0);
        return reachableTiles;
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
        ResetDistances(startIndex, currentActor.currentDirection);
        // Check what tiles you can move to.
        int distance = 0;
        for (int i = 0; i < totalRows * totalColumns; i++)
        {
            distance = heap.PeekWeight();
            if (distance > moveRange)
            {
                break;
            }
            if (!ActorChecksClosestTile(currentActor, -1, false)){break;}
        }
        // Check what tiles you can attack based on the tiles you can move to.
        // O(n).
        for (int i = 0; i < reachableTiles.Count; i++)
        {
            RecursiveAdjacency(reachableTiles[i], attackRange);
            attackableTiles.AddRange(adjacentTiles.Except(attackableTiles));
        }
        attackableTiles.Remove(startIndex);
        return attackableTiles;
    }

    // Skill can ignore direction generally speaking.
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
        int start = actor.locationIndex;
        for (int i = 0; i < 6; i++)
        {
            GetTilesInLineRange(start, range, i);
        }
        return reachableTiles;
    }

    public List<int> FindTilesInConeRange(int start, int range, int direction)
    {
        reachableTiles.Clear();
        if (direction < 0){return reachableTiles;}
        int dir1 = direction;
        int dir2 = (direction+1)%6;
        int dir3 = (direction+5)%6;
        // Start by adding lines in the facing directions.
        GetTilesInLineRange(start, range, dir1);
        GetTilesInLineRange(start, range, dir2);
        GetTilesInLineRange(start, range, dir3);
        // Then add the appropriate things inbetween.
        if (range ==2)
        {
            int middleTile = GetDestination(start,direction);
            if (DirectionCheck(middleTile, dir3))
            {
                reachableTiles.Add(GetDestination(middleTile, dir3));
            }
            if (DirectionCheck(middleTile, dir2))
            {
                reachableTiles.Add(GetDestination(middleTile, dir2));
            }
        }
        return reachableTiles;
        
    }

    protected void GetTilesInLineRange(int start, int range, int direction)
    {
        for (int i = 0; i < range; i++)
        {
            if (DirectionCheck(start, direction))
            {
                start = GetDestination(start, direction);
                reachableTiles.Add(start);
            }
        }
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

    public override int CalculateDistance(int pointOne, int pointTwo)
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
        int addedDist = 0;
        int directionChange = -1;
        List<int> directionsBetweenLocation = new List<int>();
        directionsBetweenLocation.Add(actor.currentDirection);
        FindPathIndex(actor, destination);
        for (int i = 0; i < actualPath.Count; i++)
        {
            // Add direction change to the calculations.
            if (i == 0)
            {
                directionsBetweenLocation.Add(DirectionBetweenLocations(actor.locationIndex, actualPath[i]));
            }
            else
            {
                directionsBetweenLocation.Add(DirectionBetweenLocations(actualPath[i-1], actualPath[i]));
            }
            directionChange = ReturnDirectionChange(directionsBetweenLocation[i], directionsBetweenLocation[i+1]);
            addedDist = actor.ReturnMoveCostForTile(terrainInfo[actualPath[i]]);
            if (actor.movementType != 1)
            {
                addedDist = AdjustMoveCostOnDirectionChange(addedDist, directionChange);
            }
            distance += addedDist;
        }
        return distance;
    }

    public int CalculateDirectionToLocation(TacticActor actor, int destination)
    {
        FindPathIndex(actor, destination);
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
