using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActor : AllStats
{
    public TacticActor playerActor;
    public int currentLevel;
    public int healthPerLevel = 3;
    public int energyPerLevel = 1;
    public string species = "Undead";
    public List<string> learntPassives;
    public List<string> learntSkills;
    public EquipmentContainer allEquipment;

    void Start()
    {
        UpdateStats();
    }

    public void UpdateStats()
    {
        allEquipment.UpdateStats();
        currentLevel = GameManager.instance.playerLevel;
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
    }
}
