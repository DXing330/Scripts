using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecruitmentShop : MonoBehaviour
{
    // lists of possible things to buy and their costs
    public List<string> availableFighters;
    public List<int> fighterCosts;

    private bool AffordFighter(int cost, int type = 2)
    {
        return GameManager.instance.CheckCost(type, cost);
    }

    public void BuyFighter(string fighterName)
    {
        int indexOf = availableFighters.IndexOf(fighterName);
        int cost = fighterCosts[indexOf];
        if (AffordFighter(cost))
        {
            GameManager.instance.goldCoins -= cost;
            GainFighter(fighterName);
        }
    }

    private void GainFighter(string fighterName)
    {
        GameManager.instance.armyData.GainFighter(fighterName);
    }

    // Quick and dirty method, do it better later.
    public void BuySkeleton()
    {
        if (GameManager.instance.goldCoins >= 6)
        {
            GameManager.instance.goldCoins -= 6;
            GainFighter("Skeleton");
        }
    }
}
