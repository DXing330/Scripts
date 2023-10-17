using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecruitmentShop : MonoBehaviour
{
    // lists of possible things to buy and their costs
    public int acceptedCurrency = 2;
    public List<int> acceptedCurrencies;
    public List<string> availableFighters;
    public List<int> fighterCosts;
    public List<string> investmentFlavorTexts;
    public List<string> fighterFlavorTexts;
    public int currentlyViewedFighter = 0;
    public Image fighterImage;
    public List<Sprite> fighterSprites;
    public TMP_Text fighterCost;
    public TMP_Text investmentCost;
    public TMP_Text fighterFlavor;
    public TMP_Text investmentFlavor;
    public TMP_Text currentCurrency;
    public UpgradeUnitPanel upgradePanel;

    void Start()
    {
        UpdatePanel();
        UpdateStoreTexts();
    }

    public void ChangeViewed(bool right)
    {
        if (right)
        {
            currentlyViewedFighter = (currentlyViewedFighter+1)%availableFighters.Count;
        }
        else
        {
            currentlyViewedFighter--;
            if (currentlyViewedFighter < 0)
            {
                currentlyViewedFighter = availableFighters.Count - 1;
            }
        }
        UpdatePanel();
        UpdateStoreTexts();
    }

    public void UpdatePanel()
    {
        upgradePanel.unitName = availableFighters[currentlyViewedFighter];
        upgradePanel.upgradeCostType = acceptedCurrency;
        upgradePanel.upgradeCostAmount = fighterCosts[currentlyViewedFighter] * 6;
        upgradePanel.UpdateUpgradeInfo();
    }

    public void UpdateStoreTexts()
    {
        int cost = fighterCosts[currentlyViewedFighter];
        fighterCost.text = "Buy"+"\n"+"("+cost+" Gold)";
        investmentCost.text = "Invest"+"\n"+"("+(cost*6)+" Gold)";
        fighterFlavor.text = fighterFlavorTexts[currentlyViewedFighter];
        investmentFlavor.text = investmentFlavorTexts[currentlyViewedFighter];
        fighterImage.sprite = fighterSprites[currentlyViewedFighter];
        currentCurrency.text = GameManager.instance.ReturnCurrency(acceptedCurrency).ToString();
    }

    private bool AffordFighter(int cost, int type = 2)
    {
        return GameManager.instance.CheckCost(type, cost);
    }

    public void BuyFighter()
    {
        int cost = fighterCosts[currentlyViewedFighter];
        if (AffordFighter(cost))
        {
            GameManager.instance.goldCoins -= cost;
            GainFighter(availableFighters[currentlyViewedFighter]);
        }
        UpdateStoreTexts();
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
