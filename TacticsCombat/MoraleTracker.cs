using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoraleTracker : MonoBehaviour
{
    public void GetOriginalEnemies(List<TacticActor> actors)
    {
        int count = 0;
        int totalHealth = 0;
        foreach (TacticActor actor in actors)
        {
            if (actor.team > 0)
            {
                count++;
                totalHealth += actor.baseHealth;
            }
        }
        SetOriginalEnemyCount(count);
        SetOriginalEnemyMorale(totalHealth);
    }
    public void AdjustMoraleByPercent(int percentage)
    {
        int newMorale = originalEnemyMorale*(100 + percentage)/100;
        SetOriginalEnemyMorale(newMorale);
    }
    protected int originalEnemyCount;
    protected void SetOriginalEnemyCount(int count){originalEnemyCount = count; currentEnemyCount = count;}
    protected int currentEnemyCount;
    public void UpdateEnemyCount(int change = -1){currentEnemyCount += change;}
    public int ReturnCurrentEnemyCount(){return currentEnemyCount;}
    public int originalEnemyMorale;
    // Based on the health of the whole enemy team.
    protected void SetOriginalEnemyMorale(int amount)
    {
        originalEnemyMorale = amount;
        currentEnemyMorale = amount;
    }
    // Whenever someone dies or takes damage update this.
    public int currentEnemyMorale;
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
            // Enemies lose morale according to how many members there.
            UpdateEnemyCount();
            UpdateMorale(-(originalEnemyMorale/(currentEnemyCount+originalEnemyCount)));
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
