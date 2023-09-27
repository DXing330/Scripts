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

    void Start()
    {
        UpdateStats();
    }

    public void UpdateStats()
    {
        currentLevel = GameManager.instance.playerLevel;
        playerActor.baseHealth = baseHealth + ((currentLevel - 1) * healthPerLevel);
        playerActor.baseAttack = baseAttack;
        playerActor.baseDefense = baseDefense;
        playerActor.baseEnergy = baseEnergy;
        playerActor.baseMovement = baseMovement;
        playerActor.attackRange = attackRange;
        playerActor.movementType = moveType;
        playerActor.passiveNames = new List<string>(learntPassives);
        playerActor.activeSkillNames = new List<string>(learntSkills);
    }
}
