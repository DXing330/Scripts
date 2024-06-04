using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticActorAI : MonoBehaviour
{
    // Keep track of who hurt you and by how much.
    public List<TacticActor> bullies;
    protected void SortBullies()
    {
        CheckOnBullies();
        GameManager.instance.utility.QuickSortActorsByIntList(bullies, hurtAmount, 0, bullies.Count - 1);
    }
    protected void CheckOnBullies()
    {
        if (bullies.Count > 0)
        {
            for (int i = 0; i < bullies.Count; i++)
            {
                if (bullies[i].health <= 0)
                {
                    bullies.RemoveAt(i);
                    hurtAmount.RemoveAt(i);
                }
            }
        }
    }
    public List<int> hurtAmount;
    public void AddHurt(int amount, TacticActor bully)
    {
        int indexOf = bullies.IndexOf(bully);
        if (indexOf >= 0)
        {
            hurtAmount[indexOf] += amount;
        }
        else
        {
            bullies.Add(bully);
            hurtAmount.Add(amount);
        }
        SortBullies();
    }
    public TacticActor ReturnBiggestBully()
    {
        if (bullies.Count >= 0){return bullies[0];}
    }    
}
