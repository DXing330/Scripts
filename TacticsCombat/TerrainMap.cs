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
    public List<int> terrainInfo;
    public List<int> terrainEffects;
    public List<int> terrainEffectDurations;
    public List<int> allUnoccupied;
    public List<int> occupiedTiles;
    public List<int> highlightedTiles;
    public List<int> targetableTiles;
    private int currentTarget = 0;
    private int skillCenter;
    private int skillSpan;
    public List<int> currentTiles;
    public List<TerrainTile> terrainTiles;
    public TacticTileList allTiles;
    public TerrainMaker terrainMaker;
    public TerrainPathfinder pathFinder;
    public List<TacticActor> actors;
    public ActorManager actorManager;
    public MoveManager moveManager;
    public SkillEffectManager skillManager;
    public TacticActorInfo actorInfo;

    void Start()
    {
        Application.targetFrameRate = 60;
        //InitializeTiles();
        GenerateMap(0, fullSize);
        UpdateCenterTile((fullSize * 2) + 2);
        UpdateMap();
        //actorManager.GenerateActor(0, 0, 0);
        GenerateActor(0, "Player", 0);
        GenerateActor((fullSize * 2) + 2, "Wolf", 1);
        GenerateActor((fullSize * fullSize) - 2, "Bear", 1);
        pathFinder.SetTerrainInfo(terrainInfo, fullSize, occupiedTiles);
        NextTurn();
    }

    /*private void InitializeTiles()
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
    }*/

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
        actorInfo.UpdateInfo(actors[turnIndex]);
    }

    public void ActorStartMoving()
    {
        GetReachableTiles();
    }

    public void ActorStopMoving()
    {
        UpdateCenterTile(actors[turnIndex].locationIndex);
        UpdateMap();
        actorInfo.UpdateInfo(actors[turnIndex]);
    }

    public void ActorStartAttacking()
    {
        GetTargetableTiles(actors[turnIndex].attackRange);
        HighlightTiles(false);
        if (targetableTiles.Count > 0)
        {
            currentTarget = 0;
            SeeTarget();
        }
    }

    public void ActorStartUsingSkills()
    {
        // Reuse it for skills.
        currentTarget = 0;
    }

    public void SelectSkill()
    {
        if (!actors[turnIndex].CheckSkillActivatable(actors[turnIndex].activeSkills[currentTarget]))
        {
            return;
        }
        skillCenter = actors[turnIndex].locationIndex;
        skillSpan = actors[turnIndex].activeSkills[currentTarget].span;
        SeeSkillRange();
        SeeSkillSpan();
    }

    public void ActivateSkill()
    {
        ActorStopMoving();
        actors[turnIndex].ActivateSkill(currentTarget);
        int tileNumber = 0;
        TacticActor target = null;
        for (int i = 0; i < targetableTiles.Count; i++)
        {
            tileNumber = targetableTiles[i];
            if (occupiedTiles[tileNumber] > 0)
            {
                target = ReturnActorOnTile(tileNumber);
                skillManager.ApplySkillEffect(target, actors[turnIndex].activeSkills[currentTarget], actors[turnIndex]);
            }
        }
    }

    public void SwitchSkill(bool right = true)
    {
        int skillsAmount = actors[turnIndex].activeSkills.Count;
        if (right)
        {
            if (currentTarget + 1 < skillsAmount)
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
                currentTarget = skillsAmount - 1;
            }
        }
    }

    public TacticActiveSkill ReturnCurrentSkill()
    {
        if (actors[turnIndex].activeSkills.Count <= 0)
        {
            return null;
        }
        return (actors[turnIndex].activeSkills[currentTarget]);
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
        return ReturnActorOnTile(targetLocation);
    }

    public TacticActor ReturnActorOnTile(int tileNumber)
    {
        if (tileNumber < 0)
        {
            return null;
        }
        return actors[occupiedTiles[tileNumber]-1];
    }

    public void ActorStopAttacking()
    {
        UpdateCenterTile(actors[turnIndex].locationIndex);
        UpdateMap();
    }

    public void CurrentActorAttack()
    {
        if (targetableTiles.Count <= 0 || !actors[turnIndex].CheckActions())
        {
            return;
        }
        actors[turnIndex].actionsLeft--;
        actorManager.BattleBetweenActors(actors[turnIndex], ReturnCurrentTarget());
        actorInfo.UpdateInfo(actors[turnIndex]);
    }

    public void MoveActor(int direction)
    {
        if (actors[turnIndex].team == 0)
        {
            moveManager.MoveInDirection(actors[turnIndex], direction);
            UpdateOnActorTurn();
            GetReachableTiles();
            actorInfo.UpdateInfo(actors[turnIndex]);
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
        int winners = actorManager.WinningTeam();
        if (winners >= 0)
        {
            if (winners == 0)
            {
                GameManager.instance.GainResource(2, 1);
            }
            else
            {
                // Players lose.
            }
            actorManager.ReturnToHub();
        }
        if (turnIndex >= actors.Count)
        {
            turnIndex = 0;
        }
        ActorsTurn();
        actorInfo.UpdateInfo(actors[turnIndex]);
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
            actorManager.SubtractTeamCount(deadActor.team);
            actors.Remove(deadActor);
            UpdateMap();
            GetTargetableTiles(actors[turnIndex].attackRange);
        }
    }

    public void GenerateActor(int location, string type, int team)
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
        occupiedTiles = new List<int>(allUnoccupied);
        for (int i = 0; i < actors.Count; i++)
        {
            occupiedTiles[actors[i].locationIndex] = i+1;
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
            terrainTiles[i].ResetAOEHighlight();
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
        terrainTiles[imageIndex].UpdateImage(actors[actorIndex].spriteRenderer.sprite);
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
    // Don't need to call it again if the center tile is moved around.
    private void HighlightTiles(bool cyan = true)
    {
        int index = -1;
        for (int i = 0; i < highlightedTiles.Count; i++)
        {
            index = currentTiles.IndexOf(highlightedTiles[i]);
            if (index >= 0)
            {
                HighlightTile(index, cyan);
            }
        }
    }

    private void HighlightTile(int imageIndex, bool blue = true)
    {
        terrainTiles[imageIndex].Highlight(blue);
    }

    private void HighlightSkillAOE()
    {
        for (int i = 0; i < targetableTiles.Count; i++)
        {
            AOEHighlightTile(currentTiles.IndexOf(targetableTiles[i]));
        }
    }

    private void AOEHighlightTile(int imageIndex, bool red = true)
    {
        terrainTiles[imageIndex].AoeHighlight(red);
    }

    private void GetTargetableTiles(int targetRange)
    {
        currentTarget = 0;
        UpdateOccupiedTiles();
        targetableTiles.Clear();
        int start = actors[turnIndex].locationIndex;
        // Need a new list so that they don't both point to the same thing and automatically update with each other.
        highlightedTiles = new List<int>(pathFinder.FindTilesInRange(start, targetRange, 1));
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
        HighlightTiles(false);
        // Update some info about the target.
    }

    // Just do this once at the beginning of skill movement to get the highlighted range of the skill.
    private void SeeSkillRange()
    {
        int start = actors[turnIndex].locationIndex;
        int skillRange = actors[turnIndex].activeSkills[currentTarget].range;
        highlightedTiles = new List<int>(pathFinder.FindTilesInRange(start, skillRange, 1));
        highlightedTiles.Add(start);
    }

    // Move the skill around.
    public void MoveSkill(int direction)
    {
        // Check the distance between the actor and the skill and make sure its within range.
        // Move it around.
        int nextTile = skillCenter;
        switch (direction)
        {
            case 0:
                nextTile -= fullSize;
                break;
            case 1:
                nextTile++;
                break;
            case 2:
                nextTile += fullSize;
                break;
            case 3:
                nextTile--;
                break;
        }
        if (highlightedTiles.Contains(nextTile))
        {
            skillCenter = nextTile;
            SeeSkillSpan();
        }
    }

    private void SeeSkillSpan()
    {
        UpdateCenterTile(skillCenter);
        UpdateMap();
        HighlightTiles();
        // Need to keep track of the skill's center location.
        // Highlight the center location and tiles around it within the span.
        targetableTiles = new List<int>(pathFinder.FindTilesInRange(skillCenter, skillSpan, 1));
        targetableTiles.Add(skillCenter);
        HighlightSkillAOE();
    }
}
