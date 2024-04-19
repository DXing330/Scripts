using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActor : AllStats
{
    public TacticActor playerActor;
    public string allBaseStats;
    public string typeName;
    public string personalName;
    public string ReturnName()
    {
        if (personalName.Length < 2){return typeName;}
        return personalName;
    }
    public int currentLevel;
    public int currentHealth = -1;
    public int ReturnCurrentHealth()
    {
        if (typeName == "Player" || typeName == "Familiar")
        {
            return baseHealth+((currentLevel-1) * healthPerLevel);
        }
        if (currentHealth < 0){return baseHealth;}
        else {return currentHealth;}
    }
    public void UpdateCurrentHealth(int newHealth = -1)
    {
        if (newHealth < 0){currentHealth = baseHealth;}
        else {currentHealth = newHealth;}
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
        allStatList.Add(attackRange);
        allStatList.Add(baseActions);
        allStatList.Add(size);
        allStatList.Add(baseInitiative);
        return allStatList;
    }

    public void UpdateStats()
    {
        currentLevel = GameManager.instance.playerLevel;
        playerActor.typeName = typeName;
        playerActor.level = currentLevel;
        playerActor.CopyAllStats(this);
        playerActor.movementCosts = movementCosts;
        if (currentHealth < 0 || currentHealth >= baseHealth)
        {
            playerActor.baseHealth = baseHealth+((currentLevel-1) * healthPerLevel);
        }
        else
        {
            playerActor.baseHealth = currentHealth;
        }
        playerActor.species = species;
        /*if (learntSkills.Count <= 0)
        {
            return;
        }*/
        UpdateActives();
        UpdatePassives();
        allEquipment.UpdateActorStats(playerActor);
    }

    // Mob characters don't get any level bonuses, just equipment bonuses.
    public void SideCharacterUpdateStats()
    {
        GameManager.instance.actorData.LoadPlayerActorData(this, typeName);
        GameManager.instance.actorData.LoadActorData(playerActor, typeName);
        CopyAllStats(playerActor);
        playerActor.typeName = typeName;
        if (currentHealth >= 0)
        {
            playerActor.baseHealth = Mathf.Min(currentHealth, playerActor.baseHealth);
            allEquipment.UpdateActorStats(playerActor);
        }
    }

    public void SetName(string newName)
    {
        if (newName == "none" || newName.Length < 1){return;}
        typeName = newName;
        SideCharacterUpdateStats();
    }
}
