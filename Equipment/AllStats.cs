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
    public int ReturnMoveType()
    {
        return moveType;
    }
    public int size = 1;
    public int baseInitiative = 10;
    public List<int> allStatList;

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

    public void CopyAllStats(AllStats statsToCopy)
    {
        baseHealth = statsToCopy.baseHealth;
        baseAttack = statsToCopy.baseAttack;
        baseDefense = statsToCopy.baseDefense;
        baseEnergy = statsToCopy.baseEnergy;
        baseMovement = statsToCopy.baseMovement;
        moveType = statsToCopy.moveType;
        attackRange = statsToCopy.attackRange;
        baseActions = statsToCopy.baseActions;
        size = statsToCopy.size;
        baseInitiative = statsToCopy.baseInitiative;
    }

    public virtual List<int> ReturnStatList(bool main = true)
    {
        allStatList.Clear();
        allStatList.Add(baseHealth);
        allStatList.Add(baseAttack);
        allStatList.Add(baseDefense);
        allStatList.Add(baseMovement);
        allStatList.Add(baseEnergy);
        allStatList.Add(moveType);
        allStatList.Add(attackRange);
        allStatList.Add(baseActions);
        allStatList.Add(size);
        allStatList.Add(baseInitiative);
        return allStatList;
    }

    public virtual void LoadStatsFromStringList(List<string> allStats)
    {
        baseHealth = int.Parse(allStats[0]);
        baseAttack = int.Parse(allStats[1]);
        baseDefense = int.Parse(allStats[2]);
        baseEnergy = int.Parse(allStats[3]);
        baseMovement = int.Parse(allStats[4]);
        moveType = int.Parse(allStats[5]);
        attackRange = int.Parse(allStats[6]);
        baseActions = int.Parse(allStats[7]);
        size = int.Parse(allStats[8]);
        baseInitiative = int.Parse(allStats[9]);
    }

    public virtual void LoadStatsFromIntList(List<int> allStats)
    {
        baseHealth = (allStats[0]);
        baseAttack = (allStats[1]);
        baseDefense = (allStats[2]);
        baseEnergy = (allStats[3]);
        baseMovement = (allStats[4]);
        moveType = (allStats[5]);
        attackRange = (allStats[6]);
        baseActions = (allStats[7]);
        size = (allStats[8]);
        baseInitiative = (allStats[9]);
    }

    public virtual List<int> ReturnSumOfStatLists(List<string> newStats)
    {
        allStatList.Clear();
        allStatList.Add(baseHealth+int.Parse(newStats[0]));
        allStatList.Add(baseAttack+int.Parse(newStats[1]));
        allStatList.Add(baseDefense+int.Parse(newStats[2]));
        allStatList.Add(baseEnergy+int.Parse(newStats[3]));
        allStatList.Add(baseMovement+int.Parse(newStats[4]));
        allStatList.Add(moveType+int.Parse(newStats[5]));
        allStatList.Add(attackRange+int.Parse(newStats[6]));
        allStatList.Add(baseActions+int.Parse(newStats[7]));
        allStatList.Add(size+int.Parse(newStats[8]));
        allStatList.Add(baseInitiative+int.Parse(newStats[9]));
        return allStatList;
    }
}
