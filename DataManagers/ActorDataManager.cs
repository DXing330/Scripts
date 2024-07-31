using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorDataManager : MonoBehaviour
{
    public string moveConfigData;
    public List<string> moveTypeTerrainData;
    public string configData;
    public List<string> actorClasses;
    public List<string> actorMovetypes;
    public List<string> actorNames;
    public List<string> actorHealths;
    public List<string> actorAttacks;
    public List<string> actorDefenses;
    public List<string> actorEnergies;
    public List<string> actorMovements;
    public List<string> actorActions;
    public List<string> actorRanges;
    public List<string> actorMoveSkills;
    public List<string> actorAttackSkills;
    public List<string> actorSupportSkills;
    public List<string> actorPassiveSkills;
    //public List<string> actorDropAmounts;
    public List<string> actorAITypes;
    public List<string> actorSizes;
    public List<string> actorSpecies;
    public List<string> actorInitiatives;

    [ContextMenu("Load")]
    public void LoadConfigData()
    {
        string[] configBlocks = configData.Split("#");
        actorClasses = configBlocks[0].Split("|").ToList();
        actorMovetypes = configBlocks[1].Split("|").ToList();
        actorNames = configBlocks[2].Split("|").ToList();
        actorHealths = configBlocks[3].Split("|").ToList();
        actorAttacks = configBlocks[4].Split("|").ToList();
        actorDefenses = configBlocks[5].Split("|").ToList();
        actorEnergies = configBlocks[6].Split("|").ToList();
        actorMovements = configBlocks[7].Split("|").ToList();
        actorActions = configBlocks[8].Split("|").ToList();
        actorRanges = configBlocks[9].Split("|").ToList();
        actorMoveSkills = configBlocks[10].Split("|").ToList();
        actorAttackSkills = configBlocks[11].Split("|").ToList();
        actorSupportSkills = configBlocks[12].Split("|").ToList();
        actorPassiveSkills = configBlocks[13].Split("|").ToList();
        //actorDropAmounts = configBlocks[14].Split("|").ToList();
        actorAITypes = configBlocks[15].Split("|").ToList();
        actorSizes = configBlocks[16].Split("|").ToList();
        actorSpecies = configBlocks[17].Split("|").ToList();
        actorInitiatives = configBlocks[18].Split("|").ToList();
        moveTypeTerrainData = moveConfigData.Split("#").ToList();
    }

    void Start()
    {
        LoadConfigData();
    }

    public void LoadActorData(TacticActor actor, string newName)
    {
        int index = actorNames.IndexOf(newName);
        if (index < 0)
        {
            return;
        }
        actor.baseHealth = int.Parse(actorHealths[index]);
        actor.baseAttack = int.Parse(actorAttacks[index]);
        actor.baseDefense = int.Parse(actorDefenses[index]);
        actor.baseEnergy = int.Parse(actorEnergies[index]);
        actor.baseMovement = int.Parse(actorMovements[index]);
        actor.baseActions = int.Parse(actorActions[index]);
        actor.attackRange = int.Parse(actorRanges[index]);
        actor.movementType = int.Parse(actorMovetypes[index]);
        actor.LoadPassiveSkills(actorPassiveSkills[index].Split(",").ToList());
        //actor.activeSkillNames = actorSkills[index].Split(",").ToList();
        actor.npcMoveSkill = actorMoveSkills[index];
        actor.npcAttackSkill = actorAttackSkills[index];
        actor.npcSupportSkill = actorSupportSkills[index];
        actor.AIType = int.Parse(actorAITypes[index]);
        actor.size = int.Parse(actorSizes[index]);
        actor.species = actorSpecies[index];
        actor.baseInitiative = int.Parse(actorInitiatives[index]);
        UpdateActorMoveCosts(actor);
    }

    public void LoadPlayerActorData(PlayerActor actor, string newName)
    {
        int index = actorNames.IndexOf(newName);
        Debug.Log(newName);
        Debug.Log(index);
        if (index < 0)
        {
            return;
        }
        actor.baseHealth = int.Parse(actorHealths[index]);
        actor.baseAttack = int.Parse(actorAttacks[index]);
        Debug.Log(actor.baseDefense);
        Debug.Log(actorDefenses[index]);
        actor.baseDefense = int.Parse(actorDefenses[index]);
        actor.baseEnergy = int.Parse(actorEnergies[index]);
        actor.baseMovement = int.Parse(actorMovements[index]);
        actor.baseActions = int.Parse(actorActions[index]);
        actor.attackRange = int.Parse(actorRanges[index]);
        actor.moveType = int.Parse(actorMovetypes[index]);
        actor.playerActor.movementType = int.Parse(actorMovetypes[index]);
        actor.size = int.Parse(actorSizes[index]);
        actor.species = actorSpecies[index];
        actor.baseInitiative = int.Parse(actorInitiatives[index]);
        UpdateActorMoveCosts(actor.playerActor);
    }

    public string ReturnActorBaseStats(string actorName)
    {
        int index = actorNames.IndexOf(actorName);
        if (index < 0)
        {
            return "";
        }
        string baseStats = "";
        baseStats += (actorHealths[index])+"|";
        baseStats += (actorAttacks[index])+"|";
        baseStats += (actorDefenses[index])+"|";
        return baseStats;
    }

    public void UpdateActorMoveCosts(TacticActor actor)
    {
        string costs = ReturnActorMoveCosts(actor.movementType);
        actor.SetMoveCosts(ReturnActorMoveCostsList(costs));
    }
    
    protected string ReturnActorMoveCosts(int moveType)
    {
        if (moveType < 0 || moveType >= moveTypeTerrainData.Count)
        {
            return "";
        }
        return moveTypeTerrainData[moveType];
    }

    protected List<string> ReturnActorMoveCostsList(string moveCosts)
    {
        return moveCosts.Split("|").ToList();
    }
}
