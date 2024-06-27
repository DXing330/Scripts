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
        currentPage = 0;
        selectedItem = -1;
        itemStats.ResetStatTexts();
        UpdateCurrentPage();
    }
    public MarketGUIManager marketGUI;
    public MarketDataManager marketData;
    public SpriteContainer equipSprites;
    public List<GameObject> itemButtons;
    public int selectedItem = -1;
    protected void ResetSelectedItem()
    {
        selectedItem = -1;
        itemStats.ResetStatTexts();
    }
    public void SelectItem(int index)
    {
        if (index == selectedItem){ResetSelectedItem();}
        else
        {
            selectedItem = index;
            UpdateStatTexts(marketData.currentAvailable[selectedItem+(currentPage*itemButtons.Count)]);
        }
    }
    public List<GameObject> changePageButtons;
    int currentPage = 0;
    public void ChangePage(bool right = true)
    {
        ResetSelectedItem();
        currentPage = GameManager.instance.utility.ChangePage(currentPage, right, itemButtons, marketData.currentAvailable);
        UpdateCurrentPage();
    }
    protected void UpdateCurrentPage()
    {
        List<int> newPageIndices = GameManager.instance.utility.GetNewPageIndices(currentPage, itemButtons, marketData.currentAvailable);
        GameManager.instance.utility.DisableAllObjects(itemButtons);
        for (int i = 0; i < newPageIndices.Count; i++)
        {
            string[] equipmentInfo = marketData.currentAvailable[newPageIndices[i]].Split("|");
            UpdateMarketItems(equipmentInfo, i, marketData.currentPrices[newPageIndices[i]], marketData.currentQuantity[newPageIndices[i]]);
        }
    }
    public List<HighlightableImage> marketItems;
    public EquipmentStatsUI itemStats;
    public List<StatImageText> marketStats;
    protected void UpdateMarketItems(string[] equipmentInfo, int i, string price, string quantity)
    {
        itemButtons[i].SetActive(true);
        marketItems[i].UpdateSprite(equipSprites.SpriteDictionary(equipmentInfo[^1]));
    }

    protected void UpdateStatTexts(string equipmentInfo)
    {
        itemStats.UpdateStatTextsFromString(equipmentInfo);
        marketStats[0].SetText(marketData.currentPrices[selectedItem+(currentPage*itemButtons.Count)]);
        marketStats[1].SetText(marketData.currentQuantity[selectedItem+(currentPage*itemButtons.Count)]);
    }

    public void BuyButton()
    {
        if (selectedItem < 0){return;}
        int currentQuantity = int.Parse(marketData.currentQuantity[selectedItem+(currentPage*itemButtons.Count)]);
        // Try to buy.
        if (marketData.BuyEquipment(selectedItem+(currentPage*itemButtons.Count)))
        {
            if (currentQuantity == 1){ActivatePanel();}
            else
            {
                marketStats[1].SetText(marketData.currentQuantity[selectedItem+(currentPage*itemButtons.Count)]);
            }
        }
        else
        {
            // Error message about lack of funds?
        }
    }
}
