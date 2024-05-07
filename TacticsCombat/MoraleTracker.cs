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
    // Based on the health of the whole enemy team.
    public void SetOriginalEnemyMorale(int amount){originalEnemyMorale = amount; currentEnemyMorale = amount;}
    // Whenever someone dies or takes damage update this.
    protected int currentEnemyMorale;
    public void TakeDamage(int damage, int max_health, int team)
    {
        // Less enemies means they lose morale faster.
        int change = -((100*damage)/max_health)/currentEnemyCount;
        UpdateMorale(Mathf.Min(-1, change));
    }
    public void TeamMemberDies(int team, int base_health)
    {
        if (team == 0)
        {
            // Enemies gain morale.
            UpdateMorale(base_health);
        }
        else
        {
            // Enemies lose morale according to how many members there were originally and how strong that enemy was.
            UpdateMorale(-(originalEnemyMorale/originalEnemyCount + base_health));
            UpdateEnemyCount();
        }
    }
    public void UpdateMorale(int change = 0)
    {
        if (change == 0){change = -originalEnemyMorale/originalEnemyCount;}
        currentEnemyMorale += change;
        if (currentEnemyMorale > originalEnemyMorale)
        {
            currentEnemyMorale = originalEnemyMorale;
        }
    }
    public int ReturnEnemyMorale(){return currentEnemyMorale;}
    public int ReturnEnemyMoralePercent()
    {
        return 100*currentEnemyMorale/originalEnemyMorale;
    }
}
