using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecruitmentShop : MonoBehaviour
{
    // lists of possible things to buy and their costs
    public string selectedFighterName = "";
    public int selectedFighterCost = 0;

    public void SelectFighter(string fighterName)
    {
        selectedFighterName = fighterName;
    }

    private bool AffordFighter(int cost)
    {
        if (GameManager.instance.goldCoins >= cost)
        {
            return true;
        }
        return false;
    }

    public void BuyFighter(int cost, string fighterName)
    {
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
