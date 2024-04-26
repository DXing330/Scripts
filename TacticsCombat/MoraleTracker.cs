using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoraleTracker : MonoBehaviour
{
    protected int originalEnemyCount;
    public void SetOriginalEnemyCount(int count){originalEnemyCount = count; currentEnemyCount = count;}
    protected int currentEnemyCount;
    public void UpdateEnemyCount(int change = -1){currentEnemyCount += change;}
    public int ReturnCurrentEnemyCount(){return currentEnemyCount;}
    protected int originalEnemyMorale;
    public void SetOriginalEnemyMorale(int amount){originalEnemyMorale = amount; currentEnemyMorale = amount;}
    protected int currentEnemyMorale;
    public void UpdateMorale(int change = -1){currentEnemyMorale += change;}
    public int ReturnEnemyMorale(){return currentEnemyMorale;}
    public int ReturnEnemyMoralePercent()
    {
        return 100*currentEnemyMorale/originalEnemyMorale;
    }
}
