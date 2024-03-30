using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleSimulator : MonoBehaviour
{
    public bool active = false;
    public TerrainMap simulatedBattle;
    public TMP_Text winChance;
    public List<int> terrainInfo;
    public List<int> allUnoccupied;
    public List<int> occupiedTiles;
    public TerrainPathfinder pathFinder;
    public List<TacticActor> actors;
    public List<TacticActor> aliveActors;
    public List<TacticActor> allActors;
    public MoveManager moveManager;
    public SkillEffectManager skillManager;
    public TurnOrderPanel turnOrder;
    public ActionLog actionLog;
    public ActionManager actionManager;
    public TerrainEffectManager terrainEffectManager;
    public int turnIndex = 0;
    public TacticActor tempActor;
    public int wins;
    public int plays;
    public bool started = false;

    private void CheckIfActive()
    {
        string check = GameManager.instance.ReturnPlayerSetting(0);
        if (check == "1"){active = true;}
        else{active = false;}
    }

    public void UpdateSimulation()
    {
        CheckIfActive();
        if (!active){return;}
        CreateCopyOfActors();
        terrainInfo = new List<int>(simulatedBattle.terrainInfo);
        allUnoccupied = new List<int>(simulatedBattle.allUnoccupied);
        pathFinder = simulatedBattle.pathFinder;
        moveManager = simulatedBattle.moveManager;
        skillManager = simulatedBattle.skillManager;
        turnOrder = simulatedBattle.turnOrder;
        actionLog = simulatedBattle.actionLog;
        actionManager = simulatedBattle.actionManager;
        terrainEffectManager = simulatedBattle.terrainEffectManager;
    }

    private void CreateCopyOfActors()
    {
        allActors.Clear();
        for (int i = 0; i < simulatedBattle.allActors.Count; i++)
        {
            allActors.Add(simulatedBattle.actorManager.GenerateCopyActor(simulatedBattle.allActors[i]));
        }
    }

    public void ResetWinChanceText()
    {
        winChance.text = "Auto";
    }

    public void RunNSimulations(int n = 20)
    {
        if (!active){return;}
        for (int i = 0; i < n; i++)
        {
            ResetSimulation();
            StartSimulation();
            for (int j = 0; j < simulatedBattle.roundLimit; j++)
            {
                if (!started){continue;}
                ActorsTurn();
                turnIndex++;
                if (turnIndex >= actors.Count)
                {
                    turnIndex = 0;
                }
            }
            if (started)
            {
                CheckWinners(true);
                plays++;
            }
        }
        winChance.text = "Auto"+"\n"+"(Win:~"+(wins*100/plays)+"% )";
        for (int i = 0; i < allActors.Count; i++)
        {
            Destroy(allActors[i].gameObject);
        }
        for (int i = 0; i < actors.Count; i++)
        {
            Destroy(actors[i].gameObject);
        }
    }

    private void ResetSimulation()
    {
        pathFinder.heap.ResetHeap();
        actionLog.ClearActionLog();
        allActors = turnOrder.InitiativeThreadedByTeam(allActors);
        for (int i = 0; i < actors.Count; i++)
        {
            Destroy(actors[i].gameObject);
        }
        actors.Clear();
        for (int i = 0; i < allActors.Count; i++)
        {
            actors.Add(simulatedBattle.actorManager.GenerateCopyActor(allActors[i]));
        }
    }

    private void StartSimulation()
    {
        started = true;
        actors = turnOrder.InitiativeThreadedByTeam(actors);
    }

    private void CheckWinners(bool write = false)
    {
        int winners = WinningTeam(write);
        if (winners >= 0 && started)
        {
            started = false;
            if (winners == 0)
            {
                wins++;
            }
            plays++;
        }
    }

    private int WinningTeam(bool write = false)
    {
        int teamOneCount = 0;
        int teamZeroCount = 0;
        for (int i = 0; i < actors.Count; i++)
        {
            if (actors[i].health <= 0){continue;}
            switch (actors[i].team)
            {
                case 0:
                    teamZeroCount++;
                    break;
                case 1:
                    teamOneCount++;
                    break;
            }
        }
        if (teamZeroCount <= 0)
        {
            return 1;
        }
        else if (teamOneCount <= 0)
        {
            return 0;
        }
        return -1;
    }

    private void ActorsTurn()
    {
        tempActor = actors[turnIndex];
        if (tempActor.health <= 0)
        {
            return;
        }
        terrainEffectManager.BaseTerrainEffect(tempActor, terrainInfo[tempActor.locationIndex]);
        UpdateOccupiedTiles();
        pathFinder.UpdateOccupiedTiles(occupiedTiles);
        tempActor.StartTurn();
        tempActor.UpdateTarget(FindClosestEnemyInAttackRange());
        if (tempActor.attackTarget == null)
        {
            tempActor.UpdateTarget(FindNearestEnemy());
        }
        ActorUpdateDestination();
        tempActor.currentPath = pathFinder.FindPathIndex(tempActor, tempActor.destinationIndex);
        tempActor.turnPath.Clear();
        MoveAction();
        tempActor.MoveAlongPath(false);
        ActorAttackAction();
        ActorSupportAction();
        CheckWinners();
    }

    private void ActorSupportAction()
    {
        if (tempActor.actionsLeft <= 0 || tempActor.health <= 0){return;}
        NPCLoadSkill(2);
        if (tempActor.CheckSkillActivatable())
        {
            NPCActivateSkill(tempActor.locationIndex);
            tempActor.ActivateSkill();
        }
    }

    private void ActorUpdateDestination()
    {
        switch (tempActor.AIType)
        {
            case 0:
                // Aggresive means you run towards the enemy.
                tempActor.UpdateDest(tempActor.attackTarget.locationIndex);
                break;
            case 1:
                // Passive means you don't move.
                tempActor.UpdateDest(tempActor.locationIndex);
                break;
            case 2:
                // Defensive means run away from the closest enemy.
                tempActor.UpdateDest(pathFinder.FindFurthestTileFromTarget(tempActor, tempActor.attackTarget));
                break;
        }
    }

    private void MoveAction()
    {
        if (ActorMoveable())
        {
            MoveAction();
        }
    }

    private bool ActorMoveable()
    {
        // Don't move if you can't.
        if (tempActor.currentPath.Count <= 0 || tempActor.currentPath[0] == tempActor.locationIndex || tempActor.health <= 0 || tempActor.actionsLeft <= 0)
        {
            return false;
        }
        // Don't move if you can attack your target.
        if (CheckTargetInRange(tempActor, tempActor.attackTarget))
        {
            return false;
        }
        if (tempActor.currentPath.Contains(tempActor.locationIndex) && tempActor.turnPath.Count <= 0)
        {
            // Move to the next step on the path.
            int cIndex = tempActor.currentPath.IndexOf(tempActor.locationIndex);
            if (CheckDistance(tempActor.currentPath[cIndex-1]))
            {
                tempActor.turnPath.Add(tempActor.currentPath[cIndex-1]);
                //locationIndex = currentPath[cIndex-1];
                return true;
            }
        }
        else if (tempActor.turnPath.Count > 0)
        {
            // Move to the next step on the path.
            int cIndex = tempActor.currentPath.IndexOf(tempActor.turnPath[^1]);
            // Don't move if you already reached the end.
            if (cIndex <= 0)
            {
                return false;
            }
            if (CheckDistance(tempActor.currentPath[cIndex-1]))
            {
                tempActor.turnPath.Add(tempActor.currentPath[cIndex-1]);
                return true;
            }
        }
        else
        {
            // Start from the end of the path.
            if (CheckDistance(tempActor.currentPath[^1]))
            {
                tempActor.turnPath.Add(tempActor.currentPath[^1]);
                //locationIndex = currentPath[^1];
                return true;
            }
        }
        return false;
    }

    private void ActorAttackAction()
    {
        if (tempActor.health <= 0 || tempActor.actionsLeft <= 0){return;}
        NPCLoadSkill(1);
        if (pathFinder.CalculateDistance(tempActor.locationIndex, tempActor.attackTarget.locationIndex) <= tempActor.currentAttackRange && tempActor.attackTarget.health > 0)
        {
            if (tempActor.attackTarget == null){return;}
            if (tempActor.CheckSkillActivatable())
            {
                NPCActivateSkill(tempActor.attackTarget.locationIndex);
                tempActor.ActivateSkill();
                CheckIfAttackAgain();
                return;
            }
            NPCActorAttack(tempActor.attackTarget);
            tempActor.actionsLeft -= tempActor.actionsToAttack;
        }
        // Otherwise look for another target.
        else
        {
            tempActor.attackTarget = ReturnEnemyInRange(tempActor);
            if (tempActor.attackTarget == null){return;}
            if (tempActor.CheckSkillActivatable())
            {
                NPCActivateSkill(tempActor.attackTarget.locationIndex);
                tempActor.ActivateSkill();
                CheckIfAttackAgain();
                return;
            }
            NPCActorAttack(tempActor.attackTarget);
            tempActor.actionsLeft -= tempActor.actionsToAttack;
        }
        CheckIfAttackAgain();
    }

    private void NPCLoadSkill(int type)
    {
        string skillName = "";
        switch (type)
        {
            case 0:
                skillName = tempActor.npcMoveSkill;
                break;
            case 1:
                skillName = tempActor.npcAttackSkill;
                break;
            case 2:
                skillName = tempActor.npcSupportSkill;
                break;
        }
        simulatedBattle.actorManager.LoadSkillData(tempActor.activeSkill, skillName);
    }

    private void CheckIfAttackAgain()
    {
        if (tempActor.actionsLeft >= tempActor.actionsToAttack){ActorAttackAction();}
    }

    private void UpdateOccupiedTiles()
    {
        occupiedTiles = new List<int>(allUnoccupied);
        aliveActors.Clear();
        if (started)
        {
            for (int i = 0; i < actors.Count; i++)
            {
                // Don't count the dead.
                if (actors[i].health <= 0)
                {
                    continue;
                }
                aliveActors.Add(actors[i]);
                occupiedTiles[actors[i].locationIndex] = i+1;
            }
            return;
        }
    }

    public TacticActor FindNearestEnemy()
    {
        return pathFinder.FindNearestEnemy(actors[turnIndex], aliveActors);
    }

    public TacticActor FindClosestEnemyInAttackRange()
    {
        return pathFinder.FindClosestEnemyInAttackRange(actors[turnIndex], aliveActors);
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
        TacticActor tempA = null;
        int team = actor.team;
        List<int> targetableTiles = new List<int>(pathFinder.FindTilesInAttackRange(actor));
        for (int i = 0; i < targetableTiles.Count; i++)
        {
            tempA = ReturnActorOnTile(targetableTiles[i]);
            if (tempA == null){continue;}
            if (tempA.team != team){return tempA;}
        }
        return null;
    }

    public void NPCActivateSkill(int skillTargetLocation)
    {
        UpdateOccupiedTiles();
        // Lock == single target.
        if (actors[turnIndex].activeSkill.lockOn == 1)
        {
            TacticActor skillTarget = ReturnActorOnTile(skillTargetLocation);
            if (skillTarget == null){return;}
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
                BattleBetweenActors(actors[turnIndex], target, actors[turnIndex].activeSkill.basePower);
                break;
            case "Battle+Status":
                BattleBetweenActors(actors[turnIndex], target, actors[turnIndex].activeSkill.basePower);
                break;
        }
    }

    private void BattleBetweenActors(TacticActor attacker, TacticActor defender, int skillMultiplier = 0)
    {
        simulatedBattle.actorManager.BattleBetweenActors(attacker, defender, Counterable(attacker.locationIndex, defender), DetermineFlanking(defender), skillMultiplier);
        CheckWinners();
    }

    public void NPCActorAttack(TacticActor attackTarget)
    {
        if (actors[turnIndex].actionsLeft <= 1)
        {
            return;
        }
        BattleBetweenActors(actors[turnIndex], attackTarget, actors[turnIndex].activeSkill.basePower);
    }

    private bool Counterable(int attackerLocation, TacticActor defender)
    {
        int defenderLocation = defender.locationIndex;
        int range = defender.currentAttackRange;
        int distance = pathFinder.CalculateDistance(defenderLocation, attackerLocation);
        return (distance <= range);
    }

    private bool DetermineFlanking(TacticActor attackTarget)
    {
        int adjacentEnemies = 0;
        int location = attackTarget.locationIndex;
        pathFinder.RecursiveAdjacency(location);
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

    public bool CheckDistance(int index)
    {
        int distance = ReturnMoveCost(index, tempActor.movementType);
        if (distance > tempActor.movement)
        {
            tempActor.CheckIfDistanceIsCoverable(distance);
        }
        if (distance <= tempActor.movement)
        {
            tempActor.movement -= distance;
            return true;
        }
        return false;
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
}
