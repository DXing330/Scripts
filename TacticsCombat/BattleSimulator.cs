using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSimulator : MonoBehaviour
{
    public TerrainMap simulatedBattle;
    public List<int> terrainInfo;
    public List<int> allUnoccupied;
    public List<int> occupiedTiles;
    public TerrainPathfinder pathFinder;
    public List<TacticActor> actors;
    public List<TacticActor> allActors;
    public MoveManager moveManager;
    public SkillEffectManager skillManager;
    public TurnOrderPanel turnOrder;
    public ActionManager actionManager;
    public TerrainEffectManager terrainEffectManager;
    public int turnIndex = 0;
    public TacticActor tempActor;
    public int wins;
    public int plays;
    public bool started = false;

    public void UpdateSimulation()
    {
        actors = new List<TacticActor>(simulatedBattle.actors);
        allActors = new List<TacticActor>(simulatedBattle.allActors);
        terrainInfo = new List<int>(simulatedBattle.terrainInfo);
        allUnoccupied = new List<int>(simulatedBattle.allUnoccupied);
        pathFinder = simulatedBattle.pathFinder;
        moveManager = simulatedBattle.moveManager;
        skillManager = simulatedBattle.skillManager;
        turnOrder = simulatedBattle.turnOrder;
        actionManager = simulatedBattle.actionManager;
        terrainEffectManager = simulatedBattle.terrainEffectManager;
    }

    public void RunNSimulations(int n = 10)
    {
        Debug.Log("Starting "+n+" simulations.");
        for (int i = 0; i < n; i++)
        {
            ResetSimulation();
            StartSimulation();
            while (started)
            {
                ActorsTurn();
                turnIndex++;
                if (turnIndex >= actors.Count)
                {
                    turnIndex = 0;
                }
            }
        }
        Debug.Log(wins*100/plays);
    }

    private void ResetSimulation()
    {
        actors  = new List<TacticActor>(allActors);
    }

    private void StartSimulation()
    {
        started = true;
    }

    private void CheckWinners()
    {
        int winners = WinningTeam();
        if (winners >= 0)
        {
            started = false;
            if (winners == 0)
            {
                wins++;
            }
            plays++;
        }
    }

    private int WinningTeam()
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
        Debug.Log(tempActor.typeName+": "+tempActor.health);
        terrainEffectManager.AffectActorOnTerrain(tempActor, terrainInfo[tempActor.locationIndex]);
        UpdateOccupiedTiles();
        pathFinder.UpdateOccupiedTiles(occupiedTiles);
        tempActor.StartTurn();
        tempActor.UpdateTarget(FindClosestEnemyInAttackRange());
        if (tempActor.attackTarget == null)
        {
            tempActor.UpdateTarget(FindNearestEnemy());
        }
        tempActor.GetPath();
        tempActor.MoveAction();
        tempActor.MoveAlongPath(false);
        ActorAttackAction();
        CheckWinners();
    }

    private void ActorAttackAction()
    {
        if (tempActor.health <= 0 || tempActor.actionsLeft <= 0){return;}
        tempActor.NPCLoadSkill(1);
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

    private void CheckIfAttackAgain()
    {
        if (tempActor.actionsLeft >= tempActor.actionsToAttack){ActorAttackAction();}
    }

    private void UpdateOccupiedTiles()
    {
        occupiedTiles = new List<int>(allUnoccupied);
        if (started)
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
    }

    public TacticActor FindNearestEnemy()
    {
        return pathFinder.FindNearestEnemy(actors[turnIndex], actors);
    }

    public TacticActor FindClosestEnemyInAttackRange()
    {
        return pathFinder.FindClosestEnemyInAttackRange(actors[turnIndex], actors);
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
            string targetName = skillTarget.typeName;
            bool specialEffect = false;
            Debug.Log(actors[turnIndex].typeName+" used "+actors[turnIndex].activeSkill.skillName+" on "+targetName+".");
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
}
