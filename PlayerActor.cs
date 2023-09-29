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
    public int baseMovement = 3;
    public int attackRange = 1;
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
        playerActor.baseHealth = baseHealth + ((currentLevel - 1) * healthPerLevel)+allEquipment.totalBonusHealth;
        playerActor.baseAttack = baseAttack+allEquipment.totalBonusAttack;
        playerActor.baseDefense = baseDefense+allEquipment.totalBonusDefense;
        playerActor.baseEnergy = baseEnergy+allEquipment.totalBonusEnergy;
        playerActor.baseMovement = baseMovement+allEquipment.totalBonusMovement;
        playerActor.attackRange = attackRange;
        playerActor.movementType = moveType;
        playerActor.passiveNames = new List<string>(learntPassives);
        playerActor.activeSkillNames = new List<string>(learntSkills);
    }
}
