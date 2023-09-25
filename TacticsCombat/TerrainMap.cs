using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerrainMap : MonoBehaviour
{
    private int turnIndex = 0;
    private int startIndex = 0;
    private int cornerRow;
    private int cornerColumn;
    private int gridSize = 7;
    public int fullSize = 7;
    private List<int> terrainInfo;
    public List<int> allUnoccupied;
    private List<int> occupiedTiles;
    private List<int> highlightedTiles;
    public List<int> targetableTiles;
    private int currentTarget = 0;
    public List<int> currentTiles;
    public List<TerrainTile> terrainTiles;
    public TerrainMaker terrainMaker;
    public TerrainPathfinder pathFinder;
    public List<TacticActor> actors;
    public ActorManager actorManager;
    public MoveManager moveManager;

    void Start()
    {
        GenerateMap(0, fullSize);
        UpdateCenterTile((fullSize * 2) + 2);
        UpdateMap();
        //actorManager.GenerateActor(0, 0, 0);
        GenerateActor(0, 0, 0);
        GenerateActor((fullSize * 2) + 2, 0, 1);
        GenerateActor((fullSize * fullSize) - 2, 0, 1);
        pathFinder.SetTerrainInfo(terrainInfo, fullSize, occupiedTiles);
        NextTurn();
    }

    public int ActorCurrentMovement()
    {
        return actors[turnIndex].movement;
    }

    public void ActorsTurn()
    {
        if (actors[turnIndex] == null)
        {
            actors.RemoveAt(turnIndex);
            NextTurn();
        }
        moveManager.UpdateMoveMenu();
        pathFinder.UpdateOccupiedTiles(occupiedTiles);
        UpdateCenterTile(actors[turnIndex].locationIndex);
        UpdateMap();
        actors[turnIndex].StartTurn();
        if (actors[turnIndex].team > 0)
        {
            NPCActorsTurn();
        }
    }

    public void ActorStartMoving()
    {
        GetReachableTiles();
    }

    public void ActorStopMoving()
    {
        UpdateCenterTile(actors[turnIndex].locationIndex);
        UpdateMap();
    }

    public void ActorStartAttacking()
    {
        GetTargetableTiles(actors[turnIndex].attackRange);
        if (targetableTiles.Count > 0)
        {
            currentTarget = 0;
            SeeTarget();
        }
    }

    public void SwitchTarget(bool right = true)
    {
        if (targetableTiles.Count <= 0)
        {
            return;
        }
        if (right)
        {
            if (targetableTiles.Count - 1 > currentTarget)
            {
                currentTarget++;
            }
            else
            {
                currentTarget = 0;
            }
        }
        else
        {
            if (currentTarget > 0)
            {
                currentTarget--;
            }
            else
            {
                currentTarget = targetableTiles.Count - 1;
            }
        }
        SeeTarget();
    }

    public TacticActor ReturnCurrentTarget()
    {
        if (targetableTiles.Count <= 0)
        {
            return null;
        }
        int targetLocation = targetableTiles[currentTarget];
        for (int i = 0; i < actors.Count; i++)
        {
            if (actors[i].locationIndex == targetLocation)
            {
                return actors[i];
            }
        }
        return null;
    }

    public void ActorStopAttacking()
    {
        UpdateCenterTile(actors[turnIndex].locationIndex);
        UpdateMap();
    }

    public void CurrentActorAttack()
    {
        if (targetableTiles.Count <= 0)
        {
            return;
        }
        actors[turnIndex].Attack(ReturnCurrentTarget());
    }

    public void MoveActor(int direction)
    {
        if (actors[turnIndex].team == 0)
        {
            moveManager.MoveInDirection(actors[turnIndex], direction);
            UpdateOnActorTurn();
            GetReachableTiles();
        }
    }

    private void NPCActorsTurn()
    {
        actors[turnIndex].NPCStartTurn();
        NextTurn();
    }

    public void UpdateOnActorTurn()
    {
        UpdateOccupiedTiles();
        pathFinder.UpdateOccupiedTiles(occupiedTiles);
        UpdateCenterTile(actors[turnIndex].locationIndex);
        UpdateMap();
    }

    public void NextTurn()
    {
        turnIndex++;
        if (turnIndex >= actors.Count)
        {
            turnIndex = 0;
        }
        ActorsTurn();
    }

    public int ReturnMoveCost(int index)
    {
        return pathFinder.terrainTile.ReturnMoveCost(terrainInfo[index], occupiedTiles[index]);   
    }

    public int RandomDestination(int currentLocation)
    {
        int randomLocation = Random.Range(0, terrainInfo.Count);
        if (randomLocation != currentLocation && occupiedTiles[randomLocation] == 0)
        {
            return randomLocation;
        }
        return RandomDestination(currentLocation);
    }

    public bool CheckAdjacency(int location, int target)
    {
        if (location == target)
        {
            return true;
        }
        pathFinder.AdjacentFromIndex(location);
        if (pathFinder.adjacentTiles.Contains(target))
        {
            return true;
        }
        return false;
    }

    public void AddActor(TacticActor newActor)
    {
        actors.Add(newActor);
        UpdateOccupiedTiles();
        UpdateMap();
    }

    public void RemoveActor(TacticActor deadActor)
    {
        if (actors.Contains(deadActor))
        {
            actors.Remove(deadActor);
            UpdateMap();
        }
    }

    public void GenerateActor(int location, int type, int team)
    {
        actorManager.GenerateActor(location, type, team);
    }

    public void GenerateMap(int type, int size = 6)
    {
        terrainInfo = terrainMaker.GenerateTerrain(type, size);
        fullSize = size;
        allUnoccupied.Clear();
        for (int i = 0; i < fullSize * fullSize; i++)
        {
            allUnoccupied.Add(0);
        }
    }

    private void UpdateCenterTile(int index)
    {
        startIndex = index;
        DetermineCornerRowColumn();
        DetermineCurrentTiles();
    }

    private void DetermineCornerRowColumn()
    {
        int start = startIndex;
        cornerRow = -(gridSize/2);
        cornerColumn = -(gridSize/2);
        while (start >= fullSize)
        {
            start -= fullSize;
            cornerRow++;
        }
        cornerColumn += start;
    }

    private void DetermineCurrentTiles()
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

    private void AddCurrentTile(int row, int column)
    {
        if (row < 0 || column < 0 || column >= fullSize || row >= fullSize)
        {
            currentTiles.Add(-1);
            return;
        }
        currentTiles.Add((row*fullSize)+column);
    }

    private void UpdateOccupiedTiles()
    {
        // This doesn't work we need to make a copy of it.
        occupiedTiles = new List<int>(allUnoccupied);
        for (int i = 0; i < actors.Count; i++)
        {
            occupiedTiles[actors[i].locationIndex] = 1;
        }
    }

    private void UpdateMap()
    {
        UpdateOccupiedTiles();
        // O(n)
        for (int i = 0; i < terrainTiles.Count; i++)
        {
            terrainTiles[i].ResetImage();
            terrainTiles[i].ResetHighlight();
            UpdateTile(i, currentTiles[i]);
        }
        // O(n^2)
        for (int i = 0; i < actors.Count; i++)
        {
            if (currentTiles.Contains(actors[i].locationIndex))
            {
                UpdateActor(currentTiles.IndexOf(actors[i].locationIndex), i);
            }
        }
    }

    private void UpdateActor(int imageIndex, int actorIndex)
    {
        terrainTiles[imageIndex].UpdateImage(actors[actorIndex].image.sprite);
    }

    private void UpdateTile(int imageIndex, int tileIndex)
    {
        // Undefined tiles are black.
        if (tileIndex < 0 || tileIndex >= (fullSize * fullSize))
        {
            terrainTiles[imageIndex].UpdateColor(-1);
        }
        else
        {
            terrainTiles[imageIndex].UpdateColor(terrainInfo[tileIndex]);
        }
    }

    private void GetReachableTiles()
    {
        int start = actors[turnIndex].locationIndex;
        int movement = actors[turnIndex].movement;
        highlightedTiles = pathFinder.FindTilesInRange(start, movement);
        HighlightTiles();
    }

    // O(n^3)?
    private void HighlightTiles()
    {
        for (int i = 0; i < highlightedTiles.Count; i++)
        {
            if (currentTiles.Contains(highlightedTiles[i]))
            {
                HighlightTile(currentTiles.IndexOf(highlightedTiles[i]));
            }
        }
    }

    private void HighlightTile(int imageIndex, bool blue = true)
    {
        terrainTiles[imageIndex].Highlight(blue);
    }

    private void GetTargetableTiles(int targetRange)
    {
        currentTarget = 0;
        UpdateOccupiedTiles();
        targetableTiles.Clear();
        int start = actors[turnIndex].locationIndex;
        highlightedTiles = pathFinder.FindTilesInRange(start, targetRange, 1);
        // Check if the tiles in attack range have targets.
        for (int i = 0; i < highlightedTiles.Count; i++)
        {
            if (occupiedTiles[highlightedTiles[i]] > 0)
            {
                targetableTiles.Add(highlightedTiles[i]);
            }
        }
    }

    private void SeeTarget()
    {
        UpdateCenterTile(targetableTiles[currentTarget]);
        UpdateMap();
        HighlightTile(gridSize*gridSize/2, false);
        // Update some info about the target.
    }
}
