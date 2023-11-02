using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerrainMap : Map
{
    public bool battleStarted = false;
    private int turnIndex = 0;
    public int roundIndex = 0;
    private int fixedCenter;
    private bool lockedView = false;
    public new int fullSize = 13;
    public int baseTerrain = 0;
    public List<int> terrainInfo;
    public List<int> terrainEffects;
    public List<int> terrainEffectDurations;
    public List<int> allUnoccupied;
    public List<int> occupiedTiles;
    public List<int> highlightedTiles;
    public List<int> redHighlightedTiles;
    public List<int> targetableTiles;
    // 0 = none, 1 = viewing(ie attack range), 2 = moving, 3 = ST skill, 4 = aoe skill
    private int currentHighlighting = 0;
    private int currentTarget = 0;
    private int currentSkill = 0;
    private int currentViewed = 0;
    private int skillCenter;
    private int skillSpan;
    public TerrainMaker terrainMaker;
    public TerrainPathfinder pathFinder;
    public List<TacticActor> actors;
    public ActorManager actorManager;
    public MoveManager moveManager;
    public SkillEffectManager skillManager;
    public TacticActorInfo actorInfo;
    public TurnOrderPanel turnOrder;
    public ActionManager actionManager;


    public void StartBattle()
    {
        battleStarted = true;
        ActorsTurn();
    }

    protected override void Start()
    {
        Application.targetFrameRate = 30;
        actorManager.GetActorData();
        if (GameManager.instance.randomBattle > 0)
        {
            terrainInfo = GameManager.instance.fixedBattleTerrain;
            fullSize = (int) Mathf.Sqrt(terrainInfo.Count);
            allUnoccupied.Clear();
            for (int i = 0; i < fullSize * fullSize; i++)
            {
                allUnoccupied.Add(0);
            }
            actorManager.LoadFixedEnemyTeam();
        }
        else if (GameManager.instance.randomBattle <= 0)
        {
            baseTerrain = GameManager.instance.battleLocationType;
            GenerateMap(baseTerrain, fullSize);
            actorManager.LoadEnemyTeam();
        }
        UpdateCenterTile((fullSize * fullSize)/2);
        UpdateMap();
        pathFinder.SetTerrainInfo(terrainInfo, fullSize, occupiedTiles);
        fixedCenter=fullSize*fullSize/2;
    }

    private void CheckWinners()
    {
        int winners = actorManager.WinningTeam();
        if (winners >= 0)
        {
            battleStarted = false;
            bool win = false;
            if (winners == 0)
            {
                // Need to remove these actors since they have the drops we need.
                win = true;
                RemoveActors(win);
            }
            actorManager.ReturnToHub(win);
            return;
        }
        turnOrder.UpdateTurnOrder(turnIndex);
    }

    public void NextTurn()
    {
        if (!battleStarted){return;}
        turnIndex++;
        if (turnIndex >= actors.Count)
        {
            RemoveActors();
            turnIndex = 0;
            roundIndex++;
        }
        CheckWinners();
        if (!battleStarted){return;}
        ActorsTurn();
    }

    public int ActorCurrentMovement()
    {
        return actors[turnIndex].currentMovespeed;
    }

    public void ActorsTurn()
    {
        if (!actors[turnIndex].Actable())
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
        // If the NPC die on their turn you don't need to do anything else.
        if (battleStarted)
        {
            actorInfo.UpdateInfo(actors[turnIndex]);
            turnOrder.UpdateTurnOrder(turnIndex);
        }
    }

    public void ActorEndTurn()
    {
        NextTurn();
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

    public void AlertEnemyTeam()
    {
        for (int i = 0; i < actors.Count; i++)
        {
            actors[i].AlertedByAlly();
        }
    }

    public void ActorStopMoving()
    {
        ViewCurrentActor();
        ClearHighlightedTiles();
        // Auto end turn when out of moves and actions.
        if (!actors[turnIndex].Delayable())
        {
            ActorEndTurn();
        }
    }

    public void ViewCurrentActor()
    {
        UpdateCenterTile(actors[turnIndex].locationIndex);
        UpdateMap();
        actorInfo.UpdateInfo(actors[turnIndex]);
    }

    public void ActorStartAttacking()
    {
        GetTargetableTiles(actors[turnIndex].currentAttackRange);
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
        currentSkill = 0;
    }

    public void SelectSkill()
    {
        string skillName = actors[turnIndex].LoadSkillName(currentSkill);
        actorManager.LoadSkillData(actors[turnIndex].activeSkill, skillName);
        if (!CheckSkillActivatable())
        {
            return;
        }
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
            int range = SeeSkillRange();
            GetTargetableTiles(range, actors[turnIndex].activeSkill.skillTarget);
            if (targetableTiles.Count <= 0)
            {
                return;
            }
            SeeTarget();
        }
    }

    public bool CheckSkillActivatable()
    {
        return actors[turnIndex].CheckSkillActivatable();
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

    public void NPCActivateSkill(int skillTargetLocation)
    {
        UpdateOccupiedTiles();
        // Lock == single target.
        if (actors[turnIndex].activeSkill.lockOn == 1)
        {
            TacticActor skillTarget = ReturnActorOnTile(skillTargetLocation);
            bool specialEffect = false;
            Debug.Log(actors[turnIndex].typeName+" used "+actors[turnIndex].activeSkill.skillName+" on "+skillTarget.typeName+".");
            specialEffect = skillManager.ApplySkillEffect(skillTarget, actors[turnIndex].activeSkill, actors[turnIndex]);
            if (specialEffect)
            {
                SpecialSkillActivation(skillTarget);
            }
        }
        // !Lock == aoe.
    }

    private void SpecialSkillActivation(TacticActor target)
    {
        switch (actors[turnIndex].activeSkill.effect)
        {
            case "Battle":
                BattleBetweenActors(actors[turnIndex], target, actors[turnIndex].activeSkill.basePower);
                break;
            case "Summon":
                SummonSkillActivate(targetableTiles[currentTarget]);
                break;
        }
    }

    private void SummonSkillActivate(int location)
    {
        // Can't summon on occupied tiles.
        if (occupiedTiles[location] > 0)
        {
            return;
        }
        actorManager.GenerateActor(location, actors[turnIndex].activeSkill.effectSpecifics, actors[turnIndex].team, false);
        UpdateOccupiedTiles();
        UpdateMap();
    }

    private void LockOnSkillActivate()
    {
        bool specialEffect = false;
        specialEffect = skillManager.ApplySkillEffect(ReturnCurrentTarget(), actors[turnIndex].activeSkill, actors[turnIndex]);
        if (specialEffect)
        {
            SpecialSkillActivation(ReturnCurrentTarget());
        }
        actors[turnIndex].ActivateSkill();
        actorInfo.UpdateInfo(actors[turnIndex]);
        ActorStopMoving();
    }

    private void NonLockOnSkillActivate()
    {
        ActorStopMoving();
        int tileNumber = 0;
        TacticActor target = null;
        bool specialEffect = false;
        for (int i = 0; i < targetableTiles.Count; i++)
        {
            tileNumber = targetableTiles[i];
            if (occupiedTiles[tileNumber] > 0)
            {
                target = ReturnActorOnTile(tileNumber);
                specialEffect = skillManager.ApplySkillEffect(target, actors[turnIndex].activeSkill, actors[turnIndex]);
                if (specialEffect)
                {
                    SpecialSkillActivation(target);
                }
            }
        }
        actors[turnIndex].ActivateSkill();
        actorInfo.UpdateInfo(actors[turnIndex]);
    }

    public void SwitchSkill(bool right = true)
    {
        int skillsAmount = actors[turnIndex].activeSkillNames.Count;
        if (right)
        {
            if (currentSkill + 1 < skillsAmount)
            {
                currentSkill++;
            }
            else
            {
                currentSkill = 0;
            }
        }
        else
        {
            if (currentSkill > 0)
            {
                currentSkill--;
            }
            else
            {
                currentSkill = skillsAmount - 1;
            }
        }
    }

    public TacticActiveSkill ReturnCurrentSkill()
    {
        if (actors[turnIndex].activeSkillNames.Count <= 0)
        {
            return null;
        }
        string skillName = actors[turnIndex].LoadSkillName(currentSkill);
        actorManager.LoadSkillData(actors[turnIndex].activeSkill, skillName);
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
        if (!battleStarted)
        {
            return null;
        }
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

    public TacticActor ReturnCurrentAttackTarget()
    {
        return actors[currentTarget];
    }

    public TacticActor ReturnCurrentViewedTarget()
    {
        return actors[currentViewed];
    }

    public TacticActor ReturnActorOnTile(int tileNumber)
    {
        if (tileNumber < 0 || occupiedTiles[tileNumber] == 0)
        {
            return null;
        }
        return actors[occupiedTiles[tileNumber]-1];
    }

    public TacticActor ReturnEnemyInRange(TacticActor actor)
    {
        TacticActor tempActor = null;
        int team = actor.team;
        targetableTiles = new List<int>(pathFinder.FindTilesInAttackRange(actor));
        for (int i = 0; i < targetableTiles.Count; i++)
        {
            tempActor = ReturnActorOnTile(targetableTiles[i]);
            if (tempActor == null){continue;}
            if (tempActor.team != team){return tempActor;}
        }
        return null;
    }

    public bool CheckTargetInRange(TacticActor actor, TacticActor target)
    {
        int distance = pathFinder.CalculateDistance(actor.locationIndex, target.locationIndex);
        if (distance <= actor.currentAttackRange)
        {
            return true;
        }
        return false;
        /*targetableTiles = new List<int>(pathFinder.FindTilesInAttackRange(actor));
        for (int i = 0; i < targetableTiles.Count; i++)
        {
            if (ReturnActorOnTile(targetableTiles[i]) == null){continue;}
            if (ReturnActorOnTile(targetableTiles[i]) == target){return true;}
        }
        return false;*/
    }

    public void ActorStopAttacking()
    {
        UpdateCenterTile(actors[turnIndex].locationIndex);
        UpdateMap();
    }

    private void BattleBetweenActors(TacticActor attacker, TacticActor defender, int skillMultiplier = 0)
    {
        bool attackerDied = actorManager.BattleBetweenActors(attacker, defender, Counterable(attacker.locationIndex, defender), DetermineFlanking(defender), skillMultiplier);
        if (attackerDied && attacker.team == 0)
        {
            ActorStopMoving();
            actionManager.ChangeState(0);
        }
        CheckWinners();
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
        TacticActor target = ReturnCurrentTarget();
        bool attackerDied = actorManager.BattleBetweenActors(actors[turnIndex], target, Counterable(actors[turnIndex].locationIndex, target), DetermineFlanking(target));
        if (attackerDied)
        {
            ActorStopMoving();
            actionManager.ChangeState(0);
        }
        else
        {
            actorInfo.UpdateInfo(actors[turnIndex]);
        }
        CheckWinners();
        GetTargetableTiles(actors[turnIndex].currentAttackRange);
    }

    private bool DetermineFlanking(TacticActor attackTarget)
    {
        int adjacentEnemies = 0;
        int location = attackTarget.locationIndex;
        pathFinder.RecurviseAdjacency(location);
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
        if (actors[turnIndex].actionsLeft <= 1)
        {
            return;
        }
        BattleBetweenActors(actors[turnIndex], attackTarget, actors[turnIndex].activeSkill.basePower);
        CheckWinners();
    }

    public TacticActor FindNearestEnemy()
    {
        return pathFinder.FindNearestEnemy(actors[turnIndex], actors);
    }

    public TacticActor FindClosestEnemyInAttackRange()
    {
        return pathFinder.FindClosestEnemyInAttackRange(actors[turnIndex], actors);
    }

    public void ViewActorInfo(bool right = true)
    {
        if (!actors[currentViewed].Actable())
        {
            SwitchViewedActor(right);
            return;
        }
        actorInfo.UpdateInfo(actors[currentViewed]);
        UpdateCenterTile(actors[currentViewed].locationIndex);
        UpdateMap();
        //ViewReachableTiles();
        ViewAttackableTiles();
    }

    public void ViewActorByIndex(int index)
    {
        lockedView = false;
        currentViewed = turnOrder.ReturnActorIndex(index);
        if (currentViewed < 0)
        {
            return;
        }
        ViewActorInfo();
    }

    public void StartViewingActorInfo()
    {
        currentViewed = turnIndex;
        ViewActorInfo();
    }

    public void SwitchViewedActor(bool right)
    {
        if (right)
        {
            currentViewed = (currentViewed+1)%actors.Count;
        }
        else
        {
            if (currentViewed > 0)
            {
                currentViewed--;
            }
            else
            {
                currentViewed = actors.Count - 1;
            }
        }
        ViewActorInfo(right);
    }

    private bool Counterable(int attackerLocation, TacticActor defender)
    {
        int defenderLocation = defender.locationIndex;
        int range = defender.currentAttackRange;
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
        pathFinder.RecurviseAdjacency(location);
        if (pathFinder.adjacentTiles.Contains(target))
        {
            return true;
        }
        return false;
    }

    public void AddActor(TacticActor newActor, bool start = true)
    {
        if (start)
        {
            actors.Insert(0, newActor);
        }
        else
        {
            actors.Add(newActor);
        }
        //UpdateOccupiedTiles();
        //UpdateMap();
    }

    public void RemoveActors(bool win = false)
    {
        for (int i = 0; i < actors.Count; i++)
        {
            if (actors[i].health <= 0)
            {
                if (actors[i].team != 0)
                {
                    actorManager.GetDrops(actors[i]);
                }
                else
                {
                    actorManager.RemoveFromPlayerTeam(actors[i]);
                }
                actors.RemoveAt(i);
            }
            UpdateMap();
        }
        // If you win make sure you collect the drops from all the enemies.
        if (win)
        {
            for (int i = 0; i < actors.Count; i++)
            {
                if (actors[i].team != 0)
                {
                    actorManager.GetDrops(actors[i]);
                }
            }
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

    protected override void DetermineCurrentTiles()
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

    protected override void AddCurrentTile(int row, int column)
    {
        if (row < 0 || column < 0 || column >= fullSize || row >= fullSize)
        {
            currentTiles.Add(-1);
            return;
        }
        currentTiles.Add((row*fullSize)+column);
    }

    protected override void DetermineCornerRowColumn()
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

    protected override void UpdateTile(int imageIndex, int tileIndex)
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

    private void UpdateCenterTile(int index)
    {
        if (lockedView)
        {
            startIndex = fixedCenter;
        }
        else
        {
            startIndex = index;
            if (startIndex < 0)
            {
                startIndex = actors[turnIndex].locationIndex;   
            }
        }
        DetermineCornerRowColumn();
        DetermineCurrentTiles();
    }

    private void UpdateOccupiedTiles()
    {
        occupiedTiles = new List<int>(allUnoccupied);
        for (int i = 0; i < actors.Count; i++)
        {
            // Don't count the dead.
            if (!actors[i].Actable())
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
            terrainTiles[i].ResetLocationImage();
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

    private void GetReachableTiles()
    {
        currentHighlighting = 2;
        highlightedTiles = new List<int>(pathFinder.FindTilesInMoveRange(actors[turnIndex], true));
        HighlightTiles();
    }

    private void ViewAttackableTiles()
    {
        currentHighlighting = 1;
        bool current = false;
        if (currentViewed == turnIndex)
        {
            current = true;
        }
        redHighlightedTiles = new List<int>(pathFinder.FindTilesInAttackRange(actors[currentViewed], current));
        highlightedTiles = new List<int>(pathFinder.FindTilesInMoveRange(actors[currentViewed], current));
        highlightedTiles = new List<int>(highlightedTiles.Except(redHighlightedTiles));
        HighlightAttackableTilesInReach();
    }

    private void ClearHighlightedTiles()
    {
        highlightedTiles.Clear();
        // Clearing this makes an error when you try to get a actor on a targeted tile.
        targetableTiles.Clear();
        redHighlightedTiles.Clear();
        currentHighlighting = 0;
    }

    private void Rehighlight()
    {
        switch (currentHighlighting)
        {
            case 0:
                break;
            case 1:
                HighlightAttackableTilesInReach();
                break;
            case 2:
                HighlightTiles();
                break;
            case 3:
                HighlightTiles(false);
                break;
            case 4:
                HighlightTiles();
                HighlightSkillAOE();
                break;
        }
    }

    private void HighlightAttackableTilesInReach()
    {
        int index = -1;
        for (int i = 0; i < redHighlightedTiles.Count; i++)
        {
            index = currentTiles.IndexOf(redHighlightedTiles[i]);
            if (index >= 0)
            {
                HighlightTile(index, false);
            }
        }
        for (int i = 0; i < highlightedTiles.Count; i++)
        {
            index = currentTiles.IndexOf(highlightedTiles[i]);
            if (index >= 0)
            {
                HighlightTile(index, true);
            }
        }
    }

    private void HighlightTiles(bool cyan = true)
    {
        int index = -1;
        for (int i = 0; i < terrainTiles.Count; i++)
        {
            terrainTiles[i].ResetHighlight();
        }
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
        if (imageIndex < 0)
        {
            return;
        }
        terrainTiles[imageIndex].AoeHighlight(red);
    }

    // 0 for enemies, 1 for allies, 2 for everyone, 3 for self, 4 for unoccupied.
    private void GetTargetableTiles(int targetRange, int targetsType = 0)
    {
        currentTarget = 0;
        UpdateOccupiedTiles();
        targetableTiles.Clear();
        // Need a new list so that they don't both point to the same thing and automatically update with each other.
        highlightedTiles = new List<int>(pathFinder.FindTilesInSkillRange(actors[turnIndex], targetRange));
        // Some skills can target the user.
        if (targetsType != 0 && targetsType != 4)
        {
            highlightedTiles.Add(actors[turnIndex].locationIndex);
        }
        // Check if the tiles in attack range have targets.
        //targetableTiles.AddRange(highlightedTiles);
        for (int i = 0; i < highlightedTiles.Count; i++)
        {
            if (targetsType == 4)
            {
                if (occupiedTiles[highlightedTiles[i]] <= 0)
                {
                    targetableTiles.Add(highlightedTiles[i]);
                }
                continue;
            }
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

    private void SeeTarget()
    {
        currentHighlighting = 3;
        UpdateCenterTile(targetableTiles[currentTarget]);
        UpdateMap();
        HighlightTiles(false);
        // Update some info about the target.
    }

    // Just do this once at the beginning of skill movement to get the highlighted range of the skill.
    private int SeeSkillRange()
    {
        int skillRange = Mathf.Max(actors[turnIndex].currentAttackRange, actors[turnIndex].activeSkill.range);
        highlightedTiles = new List<int>(pathFinder.FindTilesInSkillRange(actors[turnIndex], skillRange));
        highlightedTiles.Add(actors[turnIndex].locationIndex);
        return skillRange;
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
        currentHighlighting = 4;
        UpdateCenterTile(skillCenter);
        UpdateMap();
        HighlightTiles();
        // Need to keep track of the skill's center location.
        // Highlight the center location and tiles around it within the span.
        targetableTiles = new List<int>(pathFinder.FindTilesInSkillSpan(skillCenter, skillSpan));
        targetableTiles.Add(skillCenter);
        HighlightSkillAOE();
    }

    public void MoveMap(int direction)
    {
        lockedView = true;
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
        // Need to maintain the highlights according to what the player was doing.
        // Moving -> move highlights, skill -> skill highlights, etc.
        Rehighlight();
    }

    public void ChangeLockMapView()
    {
        lockedView = !lockedView;
    }

    public void ActorDied()
    {
        UpdateMap();
        pathFinder.UpdateOccupiedTiles(occupiedTiles);
    }
}
