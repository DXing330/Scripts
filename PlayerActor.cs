using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActor : AllStats
{
    public TacticActor playerActor;
    public string typeName;
    public int currentLevel;
    public int healthPerLevel = 3;
    public int energyPerLevel = 1;
    public string species = "Undead";
    public List<string> learntPassives;
    public List<string> learntSkills;
    public List<string> equipStrings;
    public string equipWeapon = "none";
    public string equipArmor = "none";
    public string equipHelmet = "none";
    public string equipBoots = "none";
    public string equipAccessory = "none";
    public EquipmentContainer allEquipment;

    // If you load an invalid equip set.
    private void UnequipAll()
    {
        equipWeapon = "none";
        equipArmor = "none";
        equipHelmet = "none";
        equipBoots = "none";
        equipAccessory = "none";
    }

    // For saving your equip set.
    public string ReturnAllEquipped()
    {
        return equipWeapon+"+"+equipArmor+"+"+equipHelmet+"+"+equipBoots+"+"+equipAccessory;
    }

    public List<string> ReturnEquippedInList()
    {
        List<string> allEquips = new List<string>();
        allEquips.Clear();
        allEquips.Add(equipWeapon);
        allEquips.Add(equipArmor);
        allEquips.Add(equipHelmet);
        allEquips.Add(equipBoots);
        allEquips.Add(equipAccessory);
        return allEquips;
    }

    // Load upon startup your previous equipment set.
    public void LoadAllEquipped(string allEquipment)
    {
        if (allEquipment.Length < 10)
        {
            Debug.Log(allEquipment);
            UnequipAll();
            return;
        }
        equipStrings = allEquipment.Split("+").ToList();
        equipWeapon = equipStrings[0];
        equipArmor = equipStrings[1];
        equipHelmet = equipStrings[2];
        equipBoots = equipStrings[3];
        equipAccessory = equipStrings[4];
        UpdateEquipment();
    }

    public void UpdateEquipment()
    {
        EquipmentData data = GameManager.instance.equipData;
        data.LoadEquipData(allEquipment.weaponSlot, equipWeapon);
        data.LoadEquipData(allEquipment.armorSlot, equipArmor);
        data.LoadEquipData(allEquipment.helmetSlot, equipHelmet);
        data.LoadEquipData(allEquipment.bootsSlot, equipBoots);
        data.LoadEquipData(allEquipment.accessorySlot, equipAccessory);
        UpdateStats();
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
        if (main)
        {
            allStatList.Add(baseEnergy+((currentLevel-1) * energyPerLevel));
        }
        else
        {
            allStatList.Add(baseEnergy);
        }
        allStatList.Add(baseMovement);
        allStatList.Add(attackRange);
        allStatList.Add(baseActions);
        allStatList.Add(size);
        allStatList.Add(baseInitiative);
        return allStatList;
    }

    public void UpdateStats()
    {
        allEquipment.UpdateStats();
        currentLevel = GameManager.instance.playerLevel;
        playerActor.typeName = typeName;
        playerActor.level = currentLevel;
        playerActor.baseHealth = baseHealth+((currentLevel-1) * healthPerLevel)+allEquipment.baseHealth;
        playerActor.baseAttack = baseAttack+allEquipment.baseAttack;
        playerActor.baseDefense = baseDefense+allEquipment.baseDefense;
        playerActor.baseEnergy = baseEnergy+((currentLevel-1) * energyPerLevel)+allEquipment.baseEnergy;
        playerActor.baseMovement = baseMovement+allEquipment.baseMovement;
        playerActor.attackRange = Mathf.Max(attackRange, allEquipment.attackRange);
        playerActor.baseActions = baseActions+allEquipment.baseActions;
        playerActor.movementType = moveType;
        playerActor.size = size+allEquipment.size;
        playerActor.species = species;
        playerActor.baseInitiative = baseInitiative+allEquipment.baseInitiative;
        if (learntSkills.Count <= 0)
        {
            return;
        }
        playerActor.activeSkillNames.Clear();
        for (int i = 0; i < Mathf.Min(currentLevel, learntSkills.Count); i++)
        {
            playerActor.activeSkillNames.Add(learntSkills[i]);
        }
        playerActor.passiveSkillNames.Clear();
        for (int i = 0; i < Mathf.Min(currentLevel, learntPassives.Count); i++)
        {
            playerActor.passiveSkillNames.Add(learntPassives[i]);
        }
    }

    // Mob characters don't get any level bonuses, just equipment bonuses.
    public void SideCharacterUpdateStats()
    {
        allEquipment.UpdateStats();
        GameManager.instance.actorData.LoadActorData(playerActor, typeName);
        CopyAllStats(playerActor);
        playerActor.baseHealth += allEquipment.baseHealth;
        playerActor.baseAttack += allEquipment.baseAttack;
        playerActor.baseDefense += allEquipment.baseDefense;
        playerActor.baseEnergy += allEquipment.baseEnergy;
        playerActor.baseMovement += allEquipment.baseMovement;
        playerActor.attackRange += Mathf.Max(attackRange, allEquipment.attackRange);
        playerActor.baseActions += allEquipment.baseActions;
        playerActor.size += allEquipment.size;
        playerActor.baseInitiative += allEquipment.baseInitiative;
    }

    public void SetName(string newName)
    {
        if (newName == "none" || newName.Length < 3){return;}
        typeName = newName;
        SideCharacterUpdateStats();
    }
}
