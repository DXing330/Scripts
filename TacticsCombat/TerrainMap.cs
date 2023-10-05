using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerrainMap : MonoBehaviour
{
    public List<Sprite> tileSprites;
    public bool battleStarted = false;
    private int turnIndex = 0;
    private int startIndex = 0;
    private int fixedCenter;
    private bool freeView = false;
    private int cornerRow;
    private int cornerColumn;
    private int gridSize = 7;
    public int fullSize = 9;
    public int baseTerrain = 0;
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

    public void StartBattle()
    {
        battleStarted = true;
    }

    void Start()
    {
        Application.targetFrameRate = 30;
        GenerateMap(baseTerrain, fullSize);
        UpdateCenterTile((fullSize * fullSize)/2);
        UpdateMap();
        pathFinder.SetTerrainInfo(terrainInfo, fullSize, occupiedTiles);
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

    public void NextTurn()
    {
        if (!battleStarted)
        {
            return;
        }
        turnIndex++;
        if (turnIndex >= actors.Count)
        {
            RemoveActors();
            turnIndex = 0;
        }
        int winners = actorManager.WinningTeam();
        if (winners >= 0)
        {
            if (winners == 0)
            {
                // Gain collected items.
            }
            else
            {
                // Lost collected items.
            }
            actorManager.ReturnToHub();
            return;
        }
        ActorsTurn();
        actorInfo.UpdateInfo(actors[turnIndex]);
    }

    public int ActorCurrentMovement()
    {
        return actors[turnIndex].movement;
    }

    public void ActorsTurn()
    {
        if (actors[turnIndex].health <= 0)
        {
            NextTurn();
            return;
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

    public void DelayTurn()
    {
        if (!actors[turnIndex].Delayable())
        {
            NextTurn();
            ActorStopMoving();
        }
        if (turnIndex == actors.Count - 1)
        {
            return;
        }
        TacticActor tempActor = actors[turnIndex];
        tempActor.delayed = true;
        actors.RemoveAt(turnIndex);
        actors.Add(tempActor);
        turnIndex--;
        NextTurn();
        ActorStopMoving();
    }

    public void ActorStartMoving()
    {
        GetReachableTiles();
    }

    public void ActorStopMoving()
    {
        freeView = false;
        if (turnIndex < 0){turnIndex++;}
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
        if (!CheckSkillActivatable())
        {
            return;
        }
        actors[turnIndex].LoadSkill(currentTarget);
        if (actors[turnIndex].activeSkill.lockOn == 0)
        {
            skillCenter = actors[turnIndex].locationIndex;
            skillSpan = actors[turnIndex].activeSkill.span;
            SeeSkillRange();
            SeeSkillSpan();
        }
        else
        {
            skillCenter = actors[turnIndex].locationIndex;
            SeeSkillRange();
            GetTargetableTiles(actors[turnIndex].activeSkill.range, actors[turnIndex].activeSkill.skillTarget);
            SeeTarget();
        }
    }

    public bool CheckSkillActivatable()
    {
        return actors[turnIndex].CheckSkillActivatable(currentTarget);
    }

    public void ActivateSkill()
    {
        if (actors[turnIndex].activeSkill.lockOn == 0)
        {
            NonLockOnSkillActivate();
        }
        else
        {
            LockOnSkillActivate();
        }
    }

    private void LockOnSkillActivate()
    {
        ActorStopMoving();
        actors[turnIndex].ActivateSkill();
        skillManager.ApplySkillEffect(ReturnCurrentTarget(), actors[turnIndex].activeSkill, actors[turnIndex]);
        actorInfo.UpdateInfo(actors[turnIndex]);
    }

    private void NonLockOnSkillActivate()
    {
        ActorStopMoving();
        actors[turnIndex].ActivateSkill();
        int tileNumber = 0;
        TacticActor target = null;
        for (int i = 0; i < targetableTiles.Count; i++)
        {
            tileNumber = targetableTiles[i];
            if (occupiedTiles[tileNumber] > 0)
            {
                target = ReturnActorOnTile(tileNumber);
                skillManager.ApplySkillEffect(target, actors[turnIndex].activeSkill, actors[turnIndex]);
                // Add hitbacks to some skills
            }
        }
        actorInfo.UpdateInfo(actors[turnIndex]);
    }

    public void SwitchSkill(bool right = true)
    {
        int skillsAmount = actors[turnIndex].activeSkillNames.Count;
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
        if (actors[turnIndex].activeSkillNames.Count <= 0)
        {
            return null;
        }
        actors[turnIndex].LoadSkill(currentTarget);
        return actors[turnIndex].activeSkill;
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

    public TacticActor ReturnCurrentTurnActor()
    {
        return actors[turnIndex];
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

    public TacticActor ReturnCurrentViewed()
    {
        return actors[currentTarget];
    }

    public TacticActor ReturnActorOnTile(int tileNumber)
    {
        if (tileNumber < 0 || occupiedTiles[tileNumber] == 0)
        {
            return null;
        }
        return actors[occupiedTiles[tileNumber]-1];
    }

    public TacticActor ReturnEnemyInRange(int currentLocation, int team, int attackRange)
    {
        TacticActor tempActor = null;
        targetableTiles = new List<int>(pathFinder.FindTilesInRange(currentLocation, attackRange, -1));
        for (int i = 0; i < targetableTiles.Count; i++)
        {
            tempActor = ReturnActorOnTile(targetableTiles[i]);
            if (tempActor == null){continue;}
            if (tempActor.team != team){return tempActor;}
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
        if (targetableTiles.Count <= 0 || !actors[turnIndex].CheckActions())
        {
            return;
        }
        // If they die while attacking, automatically end their turn.
        actors[turnIndex].actionsLeft--;
        // Find if they can counter attack.
        bool attackerDied = actorManager.BattleBetweenActors(actors[turnIndex], ReturnCurrentTarget(), Counterable(actors[turnIndex].locationIndex, ReturnCurrentTarget()), DetermineFlanking(ReturnCurrentTarget()));
        if (attackerDied)
        {
            NextTurn();
            ActorStopMoving();
        }
        actorInfo.UpdateInfo(actors[turnIndex]);
    }

    private bool DetermineFlanking(TacticActor attackTarget)
    {
        int adjacentEnemies = 0;
        int location = attackTarget.locationIndex;
        pathFinder.AdjacentFromIndex(location);
        for (int i = 0; i < pathFinder.adjacentTiles.Count; i++)
        {
            TacticActor tempActor = ReturnActorOnTile(pathFinder.adjacentTiles[i]);
            if (tempActor != null && tempActor.team != attackTarget.team)
            {
                adjacentEnemies++;
            }
        }
        if (adjacentEnemies >= 2)
        {
            return true;
        }
        return false;
    }

    public void NPCActorAttack(TacticActor attackTarget)
    {
        if (actors[turnIndex].actionsLeft <= 0)
        {
            return;
        }
        //actors[turnIndex].actionsLeft--; // handled in the NPC part
        actorManager.BattleBetweenActors(actors[turnIndex], attackTarget, Counterable(actors[turnIndex].locationIndex, attackTarget), DetermineFlanking(attackTarget));
    }

    public TacticActor FindNearestEnemy()
    {
        return pathFinder.FindNearestEnemy(actors[turnIndex], actors);
    }

    public void ViewActorInfo(bool right = true)
    {
        if (actors[currentTarget].health <= 0)
        {
            SwitchViewedActor(right);
            return;
        }
        actorInfo.UpdateInfo(actors[currentTarget]);
        UpdateCenterTile(actors[currentTarget].locationIndex);
        UpdateMap();
        ViewReachableTiles();
    }

    public void StartViewingActorInfo()
    {
        currentTarget = turnIndex;
        ViewActorInfo();
    }

    public void SwitchViewedActor(bool right)
    {
        if (right)
        {
            currentTarget = (currentTarget+1)%actors.Count;
        }
        else
        {
            if (currentTarget > 0)
            {
                currentTarget--;
            }
            else
            {
                currentTarget = actors.Count - 1;
            }
        }
        ViewActorInfo(right);
    }

    private bool Counterable(int attackerLocation, TacticActor defender)
    {
        int defenderLocation = defender.locationIndex;
        int range = defender.attackRange;
        int distance = pathFinder.CalculateDistance(defenderLocation, attackerLocation);
        return (distance <= range);
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

    public int ReturnMoveCost(int index, int moveType = 0)
    {
        int distance = 1;
        switch (moveType)
        {
            case 0:
                distance = pathFinder.terrainTile.ReturnMoveCost(terrainInfo[index], occupiedTiles[index]);
                break;
            case 1:
                distance = pathFinder.terrainTile.ReturnFlyingMoveCost(terrainInfo[index], occupiedTiles[index]);
                break;
            case 2:
                distance = pathFinder.terrainTile.ReturnRidingMoveCost(terrainInfo[index], occupiedTiles[index]);
                break;
            case 3:
                distance = pathFinder.terrainTile.ReturnSwimmingMoveCost(terrainInfo[index], occupiedTiles[index]);
                break;
            case 4:
                distance = pathFinder.terrainTile.ReturnScoutingMoveCost(terrainInfo[index], occupiedTiles[index]);
                break;
        }
        return distance;   
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

    public void RemoveActors()
    {
        for (int i = 0; i < actors.Count; i++)
        {
            if (actors[i].health <= 0)
            {
                if (actors[i].team != 0)
                {
                    // Get drops.
                }
                actors.RemoveAt(i);
            }
            UpdateMap();
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
            // Don't count the dead.
            if (actors[i].health <= 0)
            {
                continue;
            }
            occupiedTiles[actors[i].locationIndex] = i+1;
        }
    }

    private void UpdateMap()
    {
        UpdateOccupiedTiles();
        pathFinder.UpdateOccupiedTiles(occupiedTiles);
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
            if (actors[i].health <= 0)
            {
                continue;
            }
            int indexOfActor = currentTiles.IndexOf(actors[i].locationIndex);
            if (indexOfActor >= 0)
            {
                UpdateActor(indexOfActor, i);
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
            int tileType = terrainInfo[tileIndex];
            terrainTiles[imageIndex].UpdateColor(tileType);
            terrainTiles[imageIndex].UpdateTileImage(tileSprites[tileType]);
        }
    }

    private void GetReachableTiles()
    {
        int start = actors[turnIndex].locationIndex;
        int movement = actors[turnIndex].movement;
        highlightedTiles = pathFinder.FindTilesInRange(start, movement, actors[turnIndex].movementType);
        HighlightTiles();
    }

    private void ViewReachableTiles()
    {
        int start = actors[currentTarget].locationIndex;
        int movement = actors[currentTarget].baseMovement;
        highlightedTiles = pathFinder.FindTilesInRange(start, movement, actors[currentTarget].movementType);
        HighlightTiles();
    }

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

    // 0 for enemies, 1 for allies, 2 for everyone.
    private void GetTargetableTiles(int targetRange, int targetsType = 0)
    {
        currentTarget = 0;
        UpdateOccupiedTiles();
        targetableTiles.Clear();
        int start = actors[turnIndex].locationIndex;
        // Need a new list so that they don't both point to the same thing and automatically update with each other.
        highlightedTiles = new List<int>(pathFinder.FindTilesInRange(start, targetRange, -1));
        if (targetsType != 0)
        {
            highlightedTiles.Add(start);
        }
        // Check if the tiles in attack range have targets.
        for (int i = 0; i < highlightedTiles.Count; i++)
        {
            if (occupiedTiles[highlightedTiles[i]] > 0)
            {
                if (targetsType == 0 && SameTeam(actors[turnIndex], ReturnActorOnTile(highlightedTiles[i])))
                {
                    continue;
                }
                if (targetsType == 1 && !SameTeam(actors[turnIndex], ReturnActorOnTile(highlightedTiles[i])))
                {
                    continue;
                }
                targetableTiles.Add(highlightedTiles[i]);
            }
        }
    }

    private bool SameTeam(TacticActor actorOne, TacticActor actorTwo)
    {
        if (actorOne.team == actorTwo.team)
        {
            return true;
        }
        return false;
    }

    private void SeeTarget(bool blue = false)
    {
        UpdateCenterTile(targetableTiles[currentTarget]);
        UpdateMap();
        HighlightTiles(blue);
        // Update some info about the target.
    }

    // Just do this once at the beginning of skill movement to get the highlighted range of the skill.
    private void SeeSkillRange()
    {
        int start = actors[turnIndex].locationIndex;
        int skillRange = actors[turnIndex].activeSkill.range;
        highlightedTiles = new List<int>(pathFinder.FindTilesInRange(start, skillRange, -1));
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
        targetableTiles = new List<int>(pathFinder.FindTilesInRange(skillCenter, skillSpan, -1));
        targetableTiles.Add(skillCenter);
        HighlightSkillAOE();
    }

    public void MoveMap(int direction)
    {
        if (!freeView)
        {
            freeView = true;
            // Need something better later.
            fixedCenter = actors[turnIndex].locationIndex;
        }
        int previousFixedCenter = fixedCenter;
        switch (direction)
        {
            case -1:
                fixedCenter=fullSize*fullSize/2;
                break;
            case 0:
                if (previousFixedCenter < fullSize)
                {
                    break;
                }
                fixedCenter-=fullSize;
                break;
            case 1:
                if (previousFixedCenter%fullSize==fullSize-1)
                {
                    break;
                }
                fixedCenter++;
                break;
            case 2:
                if (previousFixedCenter>(fullSize*(fullSize-1))-1)
                {
                    break;
                }
                fixedCenter+=fullSize;
                break;
            case 3:
                if (previousFixedCenter%fullSize==0)
                {
                    break;
                }
                fixedCenter--;
                break;
            case 4:
                MoveMap(0);
                MoveMap(1);
                break;
            case 5:
                MoveMap(2);
                MoveMap(1);
                break;
            case 6:
                MoveMap(2);
                MoveMap(3);
                break;
            case 7:
                MoveMap(0);
                MoveMap(3);
                break;
        }
        UpdateCenterTile(fixedCenter);
        UpdateMap();
        HighlightTiles();
    }

    public void ActorDied()
    {
        UpdateMap();
        pathFinder.UpdateOccupiedTiles(occupiedTiles);
    }
}
