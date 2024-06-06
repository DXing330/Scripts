using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenMarketGUI : MarketPanelGUI
{
    void Start()
    {
        marketData = GameManager.instance.villageData.market;
        if (marketData.currentAvailable.Count > marketItems.Count)
        {
            GameManager.instance.utility.EnableAllObjects(changePageButtons);
        }
    }
    public override void ActivatePanel()
    {
        panel.SetActive(true);
        ChangePage(true);
        ChangePage(false);
    }
    public MarketGUIManager marketGUI;
    public MarketDataManager marketData;
    public SpriteContainer equipSprites;
    public List<GameObject> itemButtons;
    public List<GameObject> changePageButtons;
    int currentPage = 0;
    public void ChangePage(bool right = true)
    {
        currentPage = GameManager.instance.utility.ChangePage(currentPage, right, itemButtons, marketData.currentAvailable);
        List<int> newPageIndices = GameManager.instance.utility.GetNewPageIndices(currentPage, itemButtons, marketData.currentAvailable);
        GameManager.instance.utility.DisableAllObjects(itemButtons);
        for (int i = 0; i < newPageIndices.Count; i++)
        {
            string[] equipmentInfo = marketData.currentAvailable[newPageIndices[i]].Split("|");
            UpdateMarketItems(equipmentInfo, i, marketData.currentPrices[newPageIndices[i]], marketData.currentQuantity[newPageIndices[i]]);
            // Update the market item images.
            // Update things like stats/price/quantity.
        }
    }
    public List<HighlightableImage> marketItems;
    protected void UpdateMarketItems(string[] equipmentInfo, int i, string price, string quantity)
    {
        itemButtons[i].SetActive(true);
        marketItems[i].UpdateSprite(equipSprites.SpriteDictionary(equipmentInfo[^1]));
        // Update things like stats/price/quantity.
    }
}
