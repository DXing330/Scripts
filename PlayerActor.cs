using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActor : AllStats
{
    public TacticActor playerActor;
    public string allBaseStats;
    public string currentStatString;
    public string UpdateCurrentStats()
    {
        string newStats = "";
        List<int> basicStats = ReturnStatList(false);
        for (int i = 0; i < basicStats.Count; i++)
        {
            newStats += basicStats[i]+"|";
        }
        newStats += "@";
        for (int i = 0; i < learntSkills.Count; i++)
        {
            if (learntSkills.Count <= 0){break;}
            if (learntSkills[i].Length < 1){continue;}
            newStats += learntSkills[i]+",";
        }
        newStats += "@";
        for (int i = 0; i < learntPassives.Count; i++)
        {
            if (learntPassives.Count <= 0){break;}
            if (learntPassives[i].Length < 1){continue;}
            newStats += learntPassives[i]+",";
        }
        newStats += "@"+ReturnCurrentHealth()+"@"+ReturnCurrentEnergy();
        SetCurrentStats(newStats);
        return newStats;
    }
    public void SetCurrentStats(string newStats)
    {
        currentStatString = newStats;
        string[] blocks = currentStatString.Split("@");
        LoadStatsFromStringList(blocks[0].Split("|").ToList());
        learntSkills = blocks[1].Split(",").ToList();
        learntPassives = blocks[2].Split(",").ToList();
        UpdateCurrentHealth(int.Parse(blocks[3]));
        UpdateCurrentEnergy(int.Parse(blocks[4]));
    }
    public string ReturnCurrentStats()
    {
        UpdateCurrentStats();
        return currentStatString;
    }
    public string typeName;
    public string personalName;
    public void SetPersonalName(string newName){personalName = newName;}
    public string ReturnName()
    {
        if (personalName.Length < 2){return typeName;}
        return personalName;
    }
    public string ReturnType()
    {
        return typeName;
    }
    public int currentLevel;
    public int currentHealth = -1;
    public int ReturnCurrentHealth()
    {
        /*if (typeName == "Player" || typeName == "Familiar")
        {
            return baseHealth+((currentLevel-1) * healthPerLevel);
        }*/
        if (currentHealth < 0){return baseHealth;}
        else {return currentHealth;}
    }
    public void UpdateCurrentHealth(int newHealth = -1)
    {
        if (newHealth < 0){currentHealth = baseHealth;}
        else {currentHealth = newHealth;}
    }
    public int currentEnergy = -1;
    public void UpdateCurrentEnergy(int newEnergy = -1)
    {
        if (newEnergy < 0){currentEnergy = baseEnergy;}
        else {currentEnergy = newEnergy;}
    }
    public int ReturnCurrentEnergy()
    {
        if (currentEnergy < 0){return baseEnergy;}
        else {return currentEnergy;}
    }
    public int healthPerLevel = 3;
    public int energyPerLevel = 1;
    public string species = "Undead";
    public List<string> movementCosts;
    public List<string> learntPassives;
    protected void UpdatePassives()
    {
        playerActor.passiveSkillNames.Clear();
        if (learntPassives.Count > 0)
        {
            for (int i = 0; i < Mathf.Min(currentLevel, learntPassives.Count); i++)
            {
                playerActor.passiveSkillNames.Add(learntPassives[i]);
            }
        }
        allEquipment.UpdatePassives(playerActor);
        playerActor.passiveSkillNames = new List<string>(playerActor.passiveSkillNames.Distinct());
    }
    public List<string> ReturnPassives()
    {
        UpdatePassives();
        allEquipment.UpdateActorStats(playerActor);
        playerActor.passiveSkillNames = new List<string>(playerActor.passiveSkillNames.Distinct());
        return playerActor.passiveSkillNames;
    }
    public List<string> learntSkills;
    protected void UpdateActives()
    {
        playerActor.activeSkillNames.Clear();
        if (learntSkills.Count > 0)
        {
            for (int i = 0; i < Mathf.Min(currentLevel, learntSkills.Count); i++)
            {
                playerActor.activeSkillNames.Add(learntSkills[i]);
            }
        }
        allEquipment.UpdateActives(playerActor);
        playerActor.activeSkillNames = new List<string>(playerActor.activeSkillNames.Distinct());
    }
    public List<string> ReturnActives()
    {
        UpdateActives();
        return playerActor.activeSkillNames;
    }
    public EquipmentContainer allEquipment;

    public void ResetAllData()
    {
        typeName = "";
        currentLevel = 0;
        learntPassives.Clear();
        learntSkills.Clear();
    }

    public void ResetBaseStats()
    {
        if (allBaseStats.Length < 9){return;}
        LoadStatsFromStringList(allBaseStats.Split("|").ToList());
    }

    public override List<int> ReturnStatList(bool main = true)
    {
        allStatList.Clear();
        if (main)
        {
            allStatList.Add(baseHealth+((currentLevel-1) * healthPerLevel));
        }
        else
        {
            allStatList.Add(baseHealth);
        }
        allStatList.Add(baseAttack);
        allStatList.Add(baseDefense);
        allStatList.Add(baseMovement);
        if (main)
        {
            allStatList.Add(baseEnergy+((currentLevel-1) * energyPerLevel));
        }
        else
        {
            allStatList.Add(baseEnergy);
        }
        allStatList.Add(moveType);
        allStatList.Add(attackRange);
        allStatList.Add(baseActions);
        allStatList.Add(size);
        allStatList.Add(baseInitiative);
        return allStatList;
    }

    public override void LoadStatsFromStringList(List<string> allStats)
    {
        baseHealth = int.Parse(allStats[0]);
        baseAttack = int.Parse(allStats[1]);
        baseDefense = int.Parse(allStats[2]);
        baseMovement = int.Parse(allStats[3]);
        baseEnergy = int.Parse(allStats[4]);
        moveType = int.Parse(allStats[5]);
        attackRange = int.Parse(allStats[6]);
        baseActions = int.Parse(allStats[7]);
        size = int.Parse(allStats[8]);
        baseInitiative = int.Parse(allStats[9]);
    }

    public void UpdateStats()
    {
        currentLevel = GameManager.instance.playerLevel;
        playerActor.typeName = typeName;
        playerActor.level = currentLevel;
        playerActor.CopyAllStats(this);
        playerActor.movementCosts = movementCosts;
        // Set the max and current health.
        playerActor.baseHealth = baseHealth+((currentLevel-1) * healthPerLevel);
        playerActor.health = ReturnCurrentHealth();
        // Set the max and current energy.
        playerActor.baseEnergy = baseEnergy+((currentLevel-1)) * energyPerLevel;
        playerActor.energy = ReturnCurrentEnergy();
        playerActor.species = species;
        UpdateActives();
        UpdatePassives();
        allEquipment.UpdateActorStats(playerActor);
    }

    public void UpdateEquipStats()
    {
        allEquipment.UpdateActorStats(playerActor);
    }

    // Mob characters don't get any level bonuses, just equipment bonuses.
    // Only new characters use this otherwise use their previously saved stat string.
    public void SideCharacterUpdateStats()
    {
        GameManager.instance.actorData.LoadPlayerActorData(this, typeName);
        GameManager.instance.actorData.LoadActorData(playerActor, typeName);
        CopyAllStats(playerActor);
        playerActor.typeName = typeName;
        allEquipment.UpdateActorStats(playerActor);
        playerActor.health = Mathf.Min(ReturnCurrentHealth(), playerActor.baseHealth);
        playerActor.energy = Mathf.Min(ReturnCurrentEnergy(), playerActor.baseEnergy);
    }

    public void SetName(string newName)
    {
        if (newName == "none" || newName.Length < 1){return;}
        typeName = newName;
        SideCharacterUpdateStats();
    }
}
