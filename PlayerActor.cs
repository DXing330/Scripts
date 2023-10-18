using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActor : MonoBehaviour
{
    public TacticActor playerActor;
    public int currentLevel;
    public int baseHealth = 20;
    public int healthPerLevel = 3;
    public int baseAttack = 10;
    public int baseDefense = 3;
    public int baseEnergy = 3;
    public int energyPerLevel = 1;
    public int baseMovement = 3;
    public int attackRange = 1;
    public int baseActions = 4;
    public int moveType = 0;
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
        playerActor.baseHealth = baseHealth+((currentLevel-1) * healthPerLevel)+allEquipment.totalBonusHealth;
        playerActor.baseAttack = baseAttack+allEquipment.totalBonusAttack;
        playerActor.baseDefense = baseDefense+allEquipment.totalBonusDefense;
        playerActor.baseEnergy = baseEnergy+((currentLevel-1) * energyPerLevel)+allEquipment.totalBonusEnergy;
        playerActor.baseMovement = baseMovement+allEquipment.totalBonusMovement;
        playerActor.attackRange = attackRange;
        playerActor.baseActions = baseActions;
        playerActor.movementType = moveType;
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
