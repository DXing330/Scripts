using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerrainMap : MonoBehaviour
{
    public bool hex = true;
    public BattleSimulator simulator;
    public bool battleStarted = false;
    public bool auto = false;
    public bool paused = false;
    protected int startIndex;
    protected int cornerColumn;
    protected int cornerRow;
    private int turnIndex = 0;
    public int roundIndex = 0;
    public int roundLimit = 30;
    private int fixedCenter;
    private bool lockedView = true;
    public int gridSize = 9;
    public bool square = true;
    public int fullSize = 13;
    public int totalRows;
    public int totalColumns;
    public int baseTerrain = 0;
    public List<TerrainTile> terrainTiles;
    public List<int> currentTiles;
    public List<int> terrainInfo;
    public List<string> terrainEffects;
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
    public List<Sprite> tileSprites;
    public List<Sprite> hexTileSprites;
    public List<TacticActor> actors;
    public List<TacticActor> allActors;
    public List<TacticActor> aliveActors;
    public ActorManager actorManager;
    public MoveManager moveManager;
    public SkillEffectManager skillManager;
    public TacticActorInfo actorInfo;
    public TurnOrderPanel turnOrder;
    public ActionLog actionLog;
    public ActionManager actionManager;
    public TerrainEffectManager terrainEffectManager;


    public void StartBattle()
    {
        simulator.ResetWinChanceText();
        battleStarted = true;
        SortByInitiative();
        ActorsTurn();
    }

    public void AutoTurn()
    {
        if (!battleStarted)
        {
            auto = true;
            StartBattle();
        }
        else
        {
            if (actors[turnIndex].team == 0)
            {
                NPCActorsTurn(false);
            }
        }
    }

    protected void Start()
    {
        actorManager.GetActorData();
        actorManager.skillData.LoadAllData();
        for (int i = 0; i < terrainTiles.Count; i++)
        {
            if (hex)
            {
                terrainTiles[i].SetTileNumber(i);
            }
        }
        if (GameManager.instance.randomBattle > 0)
        {
            terrainInfo = GameManager.instance.fixedBattleTerrain;
            fullSize = (int) Mathf.Sqrt(terrainInfo.Count);
            allUnoccupied.Clear();
            for (int i = 0; i < fullSize * fullSize; i++)
            {
                allUnoccupied.Add(0);
                terrainEffects.Add("none");
                terrainEffectDurations.Add(0);
            }
            actorManager.LoadFixedEnemyTeam();
            actorManager.LoadPlayerTeam(true);
        }
        else if (GameManager.instance.randomBattle <= 0)
        {
            baseTerrain = GameManager.instance.battleLocationType;
            GenerateMap(baseTerrain, fullSize);
            actorManager.LoadEnemyTeam();
            actorManager.LoadPlayerTeam();
        }
        pathFinder.SetTerrainInfo(terrainInfo, totalRows, totalColumns, occupiedTiles);
        UpdateCenterTile(1);
        UpdateMap();
        simulator.UpdateSimulation();
        simulator.RunNSimulations();
        actionLog.ClearActionLog();
    }

    private void CheckWinners()
    {
        // Some kind of turn limit.
        if (roundIndex > roundLimit)
        {
            actorManager.ReturnToHub(false);
            return;
        }
        // If you delay turn then the turnIndex might go negative which messes things up.
        if (turnIndex < 0){return;}
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
        if (battleStarted)
        {
            turnOrder.UpdateTurnOrder(turnIndex);
        }
    }

    private void SortByInitiative()
    {
        aliveActors.Clear();
        for (int i = 0; i < allActors.Count; i++)
        {
            if (allActors[i].health > 0)
            {
                aliveActors.Add(allActors[i]);
            }
        }
        actors = turnOrder.InitiativeThreadedByTeam(aliveActors);
        UpdateOccupiedTiles();
    }

    public void NextTurn()
    {
        // Need this first one in case they try to click the end turn button before player team loads in.
        if (!battleStarted){return;}
        CheckWinners();
        if (!battleStarted){return;}
        turnIndex++;
        if (turnIndex >= actors.Count)
        {
            //RemoveActors();
            turnIndex = 0;
            roundIndex++;
            if (roundIndex > roundLimit)
            {
                actorManager.ReturnToHub(false);
                return;
            }
            SortByInitiative();
        }
        ClearHighlightedTiles();
        ActorsTurn();
    }

    public int ActorCurrentMovement()
    {
        return actors[turnIndex].currentMovespeed;
    }

    public void ActorsTurn()
    {
        // Check on terrain effects.
        actionLog.AddTerrainEffect(actors[turnIndex], terrainInfo[actors[turnIndex].locationIndex]);
        terrainEffectManager.AffectActorOnTerrain(actors[turnIndex], terrainInfo[actors[turnIndex].locationIndex]);
        if (!actors[turnIndex].Actable())
        {
            NextTurn();
            return;
        }
        moveManager.UpdateMoveMenu();
        pathFinder.UpdateOccupiedTiles(occupiedTiles);
        UpdateCenterTile();
        UpdateMap();
        actors[turnIndex].StartTurn();
        if (actors[turnIndex].team > 0 || (auto))
        {
            NPCActorsTurn();
            return;
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
            ViewActorClearHighlights();
            return;
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
        ViewActorClearHighlights();
    }

    public void ActorStartMoving()
    {
        ClearHighlightedTiles();
        GetReachableTiles();
    }

    public void AlertEnemyTeam()
    {
        for (int i = 0; i < actors.Count; i++)
        {
            actors[i].AlertedByAlly();
        }
    }

    private void ViewActorClearHighlights()
    {
        ViewCurrentActor();
        ClearHighlightedTiles();
    }

    public void ActorStopMoving()
    {
        ViewActorClearHighlights();
        // Auto end turn when out of moves and actions.
        if (!actors[turnIndex].Delayable() && actors[turnIndex].actionsLeft <= 0)
        {
            ActorEndTurn();
        }
    }

    public void ViewCurrentActor()
    {
        if (turnIndex >= actors.Count)
        {
            return;
        }
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
            int range = 0;
            string targetShape = actors[turnIndex].activeSkill.targetingShape;
            switch (targetShape)
            {
                case "none":
                    range = SeeSkillRange();
                    break;
                case "Line":
                    range = LineSkillRange();
                    break;
            }
            SkillTargetableTiles(actors[turnIndex].activeSkill.skillTarget);
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
            if (skillTarget == null){return;}
            string targetName = skillTarget.typeName;
            bool specialEffect = false;
            actionLog.AddSkillAction(actors[turnIndex], skillTarget);
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
                BattleBetweenActors(actors[turnIndex], target, actors[turnIndex].activeSkill.basePower, false);
                break;
            case "Battle+Status":
                BattleBetweenActors(actors[turnIndex], target, actors[turnIndex].activeSkill.basePower, false);
                break;
            case "Battle+Displace":
                BattleBetweenActors(actors[turnIndex], target, actors[turnIndex].activeSkill.basePower, false);
                // If battling and displacing then the power of the displacement is always 1.
                moveManager.Displace(actors[turnIndex], target, 1, actors[turnIndex].activeSkill.effectSpecifics);
                break;
            case "Summon":
                SummonSkillActivate(targetableTiles[currentTarget]);
                break;
            case "Displace":
                moveManager.Displace(actors[turnIndex], target, actors[turnIndex].activeSkill.basePower, actors[turnIndex].activeSkill.effectSpecifics);
                break;
            case "Teleport":
                moveManager.Teleport(actors[turnIndex], targetableTiles[currentTarget]);
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

    public void SelectSkill(int skillIndex)
    {
        if (skillIndex < actors[turnIndex].activeSkillNames.Count)
        {
            currentSkill = skillIndex;
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
        if (turnIndex >= actors.Count)
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
        int distance = 0;
        if (actor.turnPath.Count > 0)
        {
            distance = pathFinder.CalculateDistance(actor.turnPath[^1], target.locationIndex);
        }
        else
        {
            distance = pathFinder.CalculateDistance(actor.locationIndex, target.locationIndex);
        }
        if (distance <= actor.currentAttackRange)
        {
            return true;
        }
        return false;
    }

    public void ActorStopAttacking()
    {
        UpdateCenterTile();
        UpdateMap();
    }

    private void BattleBetweenActors(TacticActor attacker, TacticActor defender, int skillMultiplier = 0, bool actionText = true)
    {
        if (actionText)
        {
            actionLog.AddActionLog(attacker.typeName+" attacks "+defender.typeName+".");
        }
        // Rotate the attacker to face the defender if possible.
        int newDirection = pathFinder.DirectionBetweenLocations(attacker.locationIndex, defender.locationIndex);
        if (newDirection >= 0)
        {
            attacker.currentDirection = newDirection;
        }
        // If you don't use the action text them it must be a skill that triggered the battle.
        bool attackerDied = actorManager.BattleBetweenActors(attacker, defender, Counterable(attacker, defender, !actionText), DetermineFlanking(defender), skillMultiplier);
        if (attackerDied && attacker.team == 0)
        {
            ActorStopMoving();
            actionManager.ChangeState(0);
        }
        CheckWinners();
        if (battleStarted)
        {
            UpdateMap();
        }
    }

    public void CurrentActorAttack()
    {
        if (targetableTiles.Count <= 0 || !actors[turnIndex].CheckActions())
        {
            return;
        }
        actors[turnIndex].actionsLeft--;
        TacticActor target = ReturnCurrentTarget();
        int newDirection = pathFinder.DirectionBetweenLocations(actors[turnIndex].locationIndex, target.locationIndex);
        if (newDirection >= 0)
        {
            actors[turnIndex].currentDirection = newDirection;
        }
        // Find if they can counter attack.
        bool attackerDied = actorManager.BattleBetweenActors(actors[turnIndex], target, Counterable(actors[turnIndex], target), DetermineFlanking(target));
        // If they die while attacking, automatically end their turn.
        if (attackerDied || actors[turnIndex].actionsLeft <= 0)
        {
            ActorStopMoving();
            actionManager.ChangeState(0);
            CheckWinners();
            return;
        }
        else
        {
            actorInfo.UpdateInfo(actors[turnIndex]);
        }
        CheckWinners();
        if (battleStarted)
        {
            GetTargetableTiles(actors[turnIndex].currentAttackRange, 0, false);
            UpdateMap();
            HighlightTiles(false);
            actionManager.UpdateActionsLeft();
        }
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
        UpdateCenterTile();
        UpdateMap();
        ViewAttackableTiles();
    }

    public void ViewActorByIndex(int index)
    {
        currentViewed = turnOrder.ReturnActorIndex(index);
        if (currentViewed < 0)
        {
            return;
        }
        ViewActorInfo();
    }

    public void ViewActorByTile(int tile)
    {
        // Check if an actor is actually on the tile.
        int tileNumber = currentTiles[tile];
        TacticActor tempActor = ReturnActorOnTile(tileNumber);
        if (tempActor == null){return;}
        if (!tempActor.Actable()){return;}
        currentViewed = actors.IndexOf(tempActor);
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

    private bool Counterable(TacticActor attacker, TacticActor defender, bool skill = false)
    {
        // Check if the defender has counter attacks left.
        if (defender.counterAttacksLeft <= 0){return false;}
        // Check if the defender is facing the right way.
        if (!pathFinder.FaceOffCheck(attacker.currentDirection, defender.currentDirection)){return false;}
        // Finally check if the attacker is in range.
        int attackerLocation = attacker.locationIndex;
        int defenderLocation = defender.locationIndex;
        int range = defender.currentAttackRange;
        int distance = pathFinder.CalculateDistance(defenderLocation, attackerLocation);
        if (distance <= range)
        {
            defender.counterAttacksLeft--;
            return true;
        }
        return false;
    }

    public void MoveActor(int direction)
    {
        if (actors[turnIndex].team == 0)
        {
            actors[turnIndex].currentDirection = direction;
            moveManager.MoveInDirection(actors[turnIndex], direction);
            UpdateAfterMovingActor();
        }
    }

    private void NPCActorsTurn(bool npc = true)
    {
        if (auto){npc = false;}
        actors[turnIndex].NPCStartTurn(npc);
        NextTurn();
        return;
    }

    private void UpdateAfterMovingActor()
    {
        UpdateOnActorTurn();
        GetReachableTiles();
        actorInfo.UpdateInfo(actors[turnIndex]);
        actionManager.UpdateActionsLeft();
    }

    public void UpdateOnActorTurn()
    {
        UpdateOccupiedTiles();
        pathFinder.UpdateOccupiedTiles(occupiedTiles);
        UpdateCenterTile();
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

    public int FindFurthestTileFromTarget(TacticActor actor, TacticActor target)
    {
        return pathFinder.FindFurthestTileFromTarget(actor, target);
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
        allActors.Add(newActor);
    }

    public void RemoveActors(bool win = false)
    {
        for (int i = 0; i < allActors.Count; i++)
        {
            if (allActors[i].health <= 0 && allActors[i].team == 0)
            {
                actorManager.RemoveFromPlayerTeam(allActors[i]);
                allActors.RemoveAt(i);
            }
        }
        // If you win make sure you collect the drops from all the enemies.
        // This is not needed on some maps and my be causing bugs elsewhere.
        if (win)
        {
            for (int i = 0; i < allActors.Count; i++)
            {
                if (allActors[i].team != 0 && allActors[i].health <= 0)
                {
                    actorManager.GetDrops(allActors[i]);
                }
            }
            allActors.Clear();
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
        if (type == 1)
        {
            int terrainIndex = Random.Range(0, GameManager.instance.forestFixedTerrains.Count);
            if (GameManager.instance.forestFixedTerrains[terrainIndex].Length > size)
            {
                // Get the terrain from the GameManager.
                string[] fixedTerrainInfo = GameManager.instance.forestFixedTerrains[terrainIndex].Split(",");
                string[] fixedTerrain = fixedTerrainInfo[0].Split("|");
                // Enable backwards compatability for now.
                if (fixedTerrainInfo.Length > 2)
                {
                    square = false;
                    totalRows = int.Parse(fixedTerrainInfo[1]);
                    totalColumns = int.Parse(fixedTerrainInfo[2]);
                }
                else
                {
                    fullSize = (int) Mathf.Sqrt(fixedTerrain.Length);
                }
                terrainInfo.Clear();
                for (int j = 0; j < fixedTerrain.Length; j++)
                {
                    terrainInfo.Add(int.Parse(fixedTerrain[j]));
                }
            }
        }
        if (square)
        {
            totalRows = fullSize;
            totalColumns = fullSize;
        }
        allUnoccupied.Clear();
        for (int i = 0; i < totalRows * totalColumns; i++)
        {
            allUnoccupied.Add(0);
        }
    }

    protected void DetermineCurrentTiles()
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

    protected void AddCurrentTile(int row, int column)
    {
        if (row < 0 || column < 0 || column >= totalColumns || row >= totalRows)
        {
            currentTiles.Add(-1);
            return;
        }
        currentTiles.Add((row*totalColumns)+column);
    }

    protected void DetermineCornerRowColumn()
    {
        int start = startIndex;
        cornerRow = -(gridSize/2);
        cornerColumn = -(gridSize/2);
        while (start >= totalColumns)
        {
            start -= totalColumns;
            cornerRow++;
        }
        cornerColumn += start;
    }

    protected void UpdateTile(int imageIndex, int tileIndex)
    {
        // Undefined tiles are black.
        if (tileIndex < 0 || tileIndex >= (totalColumns * totalRows))
        {
            terrainTiles[imageIndex].UpdateColor(-1);
        }
        else
        {
            int tileType = terrainInfo[tileIndex];
            terrainTiles[imageIndex].UpdateColor(tileType);
            if (hex)
            {
                if (tileType == 7)
                {
                    terrainTiles[imageIndex].UpdateLocationImage(hexTileSprites[tileType]);
                    terrainTiles[imageIndex].UpdateTileImage(hexTileSprites[0]);
                    return;
                }
                terrainTiles[imageIndex].UpdateTileImage(hexTileSprites[tileType]);
            }
            else
            {
                terrainTiles[imageIndex].UpdateTileImage(tileSprites[tileType]);
            }
        }
    }

    private void UpdateCenterTile(int center = -1)
    {
        if (center > 0)
        {
            DetermineCentermostTile();
        }
        startIndex = fixedCenter;
        DetermineCornerRowColumn();
        DetermineCurrentTiles();
    }

    private void DetermineCentermostTile()
    {
        if (totalRows%2 == 1)
        {
            fixedCenter = (totalRows * totalColumns)/2;
        }
        else
        {
            fixedCenter = ((totalRows - 1) * totalColumns)/2;
        }
    }

    private void UpdateOccupiedTiles()
    {
        occupiedTiles = new List<int>(allUnoccupied);
        if (battleStarted)
        {
            for (int i = 0; i < actors.Count; i++)
            {
                // Don't count the dead.
                if (actors[i].health <= 0)
                {
                    continue;
                }
                occupiedTiles[actors[i].locationIndex] = i+1;
            }
            return;
        }
        for (int i = 0; i < allActors.Count; i++)
        {
            // Don't count the dead.
            if (allActors[i].health <= 0)
            {
                continue;
            }
            occupiedTiles[allActors[i].locationIndex] = i+1;
        }
    }

    private void UpdateMap()
    {
        UpdateOccupiedTiles();
        pathFinder.UpdateOccupiedTiles(occupiedTiles);
        if (auto){return;}
        // O(n)
        for (int i = 0; i < terrainTiles.Count; i++)
        {
            terrainTiles[i].ResetImage();
            terrainTiles[i].ResetLocationImage();
            terrainTiles[i].ResetHighlight();
            terrainTiles[i].ResetAOEHighlight();
            terrainTiles[i].ResetDirectionalArrows();
            UpdateTile(i, currentTiles[i]);
        }
        // O(n^2)
        for (int i = 0; i < allActors.Count; i++)
        {
            if (allActors[i].health <= 0)
            {
                continue;
            }
            int indexOfActor = currentTiles.IndexOf(allActors[i].locationIndex);
            if (indexOfActor >= 0)
            {
                UpdateActorImage(indexOfActor, i);
            }
        }
    }

    private void UpdateActorImage(int imageIndex, int actorIndex)
    {
        terrainTiles[imageIndex].UpdateImage(allActors[actorIndex].spriteRenderer.sprite);
        terrainTiles[imageIndex].UpdateDirectionalArrow(allActors[actorIndex].currentDirection);
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
        if (auto){return;}
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
        /*if (currentHighlighting != 3)
        {
            return;
        }*/
        int targetedTile = ReturnCurrentTargetTile();
        index = currentTiles.IndexOf(targetedTile);
        if (index >= 0 && targetedTile >= 0)
        {
            HighlightTile(index, !cyan);
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
    private void GetTargetableTiles(int targetRange, int targetsType = 0, bool reset = true)
    {
        if (reset)
        {
            currentTarget = 0;
        }
        UpdateOccupiedTiles();
        targetableTiles.Clear();
        if (targetsType == 3)
        {
            targetableTiles.Add(actors[turnIndex].locationIndex);
            return;
        }
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

    private void SkillTargetableTiles(int targetsType = 0)
    {
        currentTarget = 0;
        UpdateOccupiedTiles();
        targetableTiles.Clear();
        if (targetsType == 3)
        {
            targetableTiles.Add(actors[turnIndex].locationIndex);
            return;
        }
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
        if (targetsType == 5)
        {
            int selfIndex = targetableTiles.IndexOf(actors[turnIndex].locationIndex);
            if (selfIndex >= 0)
            {
                targetableTiles.RemoveAt(selfIndex);
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
        UpdateCenterTile();
        UpdateMap();
        HighlightTiles(false);
        // Update some info about the target.
    }

    private int ReturnCurrentTargetTile()
    {
        if (targetableTiles.Count <= 0)
        {
            return -1;
        }
        return targetableTiles[currentTarget];
    }

    // Just do this once at the beginning of skill movement to get the highlighted range of the skill.
    private int SeeSkillRange()
    {
        int skillRange = actors[turnIndex].activeSkill.range;
        if (actors[turnIndex].activeSkill.effect == "Battle")
        {
            skillRange = Mathf.Max(actors[turnIndex].currentAttackRange, actors[turnIndex].activeSkill.range);
        }
        highlightedTiles = new List<int>(pathFinder.FindTilesInSkillRange(actors[turnIndex], skillRange));
        highlightedTiles.Add(actors[turnIndex].locationIndex);
        return skillRange;
    }

    private int LineSkillRange()
    {
        int skillRange = actors[turnIndex].activeSkill.range;
        if (actors[turnIndex].activeSkill.effect == "Battle")
        {
            skillRange = Mathf.Max(actors[turnIndex].currentAttackRange, actors[turnIndex].activeSkill.range);
        }
        highlightedTiles = new List<int>(pathFinder.FindTilesInLineRange(actors[turnIndex], skillRange));
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
                nextTile -= totalColumns;
                break;
            case 1:
                nextTile++;
                break;
            case 2:
                nextTile += totalColumns;
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
        //UpdateCenterTile(skillCenter);
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
        if (!hex)
        {
            switch (direction)
            {
                case -1:
                    fixedCenter=((totalRows * totalColumns) + totalColumns)/2;
                    break;
                case 0:
                    if (previousFixedCenter < totalColumns)
                    {
                        break;
                    }
                    fixedCenter-=totalColumns;
                    break;
                case 1:
                    if (previousFixedCenter%totalColumns==totalColumns-1)
                    {
                        break;
                    }
                    fixedCenter++;
                    break;
                case 2:
                    if (previousFixedCenter>(totalColumns*(totalRows-1))-1)
                    {
                        break;
                    }
                    fixedCenter+=totalColumns;
                    break;
                case 3:
                    if (previousFixedCenter%totalColumns==0)
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
        }
        if (hex)
        {
            switch (direction)
            {
                case 0:
                    if (previousFixedCenter < totalColumns){break;}
                    fixedCenter-=totalColumns;
                    break;
                case 1:
                    if (previousFixedCenter%totalColumns >= totalColumns - 2){break;}
                    fixedCenter += 2;
                    break;
                case 2:
                    if (previousFixedCenter%totalColumns >= totalColumns - 2){break;}
                    fixedCenter += 2;
                    break;
                case 3:
                    if (previousFixedCenter > (totalColumns*(totalRows-1))-1){break;}
                    fixedCenter+=totalColumns;
                    break;
                case 4:
                    if (previousFixedCenter%totalColumns <= 1){break;}
                    fixedCenter -= 2;
                    break;
                case 5:
                    if (previousFixedCenter%totalColumns <= 1){break;}
                    fixedCenter -= 2;
                    break;
            }
        }
        UpdateCenterTile();
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
    }

    public void ClickOnTile(int tileNumber)
    {
        if (!battleStarted){return;}
        switch (actionManager.state)
        {
            case 0:
                ViewActorByTile(tileNumber);
                break;
            case 1:
                if (highlightedTiles.Contains(currentTiles[tileNumber]))
                {
                    moveManager.MoveActorToTile(actors[turnIndex], currentTiles[tileNumber], pathFinder.CalculateDistanceToLocation(actors[turnIndex], currentTiles[tileNumber]));
                    UpdateAfterMovingActor();
                }
                break;
            case 2:
                int tempTarget = targetableTiles.IndexOf(currentTiles[tileNumber]);
                if (tempTarget < 0)
                {
                    return;
                }
                if (currentTarget == tempTarget)
                {
                    CurrentActorAttack();
                }
                else
                {
                    currentTarget = tempTarget;
                    SeeTarget();
                }
                actionManager.attackMenu.UpdateTarget(ReturnCurrentTarget());
                break;
            case 3:
                int tempSTarget = targetableTiles.IndexOf(currentTiles[tileNumber]);
                if (tempSTarget < 0)
                {
                    return;
                }
                if (currentTarget == tempSTarget)
                {
                    ActivateSkill();
                    actionManager.ChangeState(0);
                }
                else
                {
                    currentTarget = tempSTarget;
                    SeeTarget();
                }
                actionManager.skillMenu.lockOnMenu.UpdateTarget(ReturnCurrentTarget());
                break;
        }
    }
}
