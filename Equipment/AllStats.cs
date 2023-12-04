using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllStats : MonoBehaviour
{
    public int baseHealth = 30;
    public int baseAttack = 10;
    public int baseDefense = 3;
    public int baseEnergy = 6;
    public int baseMovement = 3;
    public int attackRange = 1;
    public int baseActions = 2;
    public int moveType = 0;
    public int size = 1;
    public int baseInitiative = 10;

    public void NullAllStats()
    {
        baseHealth = 0;
        baseAttack = 0;
        baseDefense = 0;
        baseEnergy = 0;
        baseMovement = 0;
        moveType = 0;
        attackRange = 0;
        baseActions = 0;
        size = 0;
        baseInitiative = 0;
    }
}
