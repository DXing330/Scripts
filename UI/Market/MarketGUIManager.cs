using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MarketGUIManager : MonoBehaviour
{
    public SpriteContainer equipSprites;
    public MarketDataManager marketData;
    void Start()
    {
        marketData = GameManager.instance.villageData.market;
        UpdateGUI();
    }
    protected void UpdateGUI()
    {
        UpdateResources();
        HighlightSelectedMarket();
    }
    public List<StatImageText> resources;
    protected void UpdateResources()
    {
        for (int i = 0; i < GameManager.instance.villageData.resources.Count; i++)
        {
            resources[i].SetText(GameManager.instance.villageData.resources[i]);
        }
    }
    public List<TMP_Text> marketButtonTexts;
    public int selectedMarket = 0;
    protected void HighlightSelectedMarket()
    {
        for (int i = 0; i < marketButtonTexts.Count; i++)
        {
            if (i == selectedMarket)
            {
                marketButtonTexts[i].color = Color.red;
                continue;
            }
            marketButtonTexts[i].color = Color.black;
        }
    }
    public List<MarketPanelGUI> marketPanels;
    protected void ActivateSelectedPanel()
    {
        for (int i = 0; i < marketPanels.Count; i++)
        {
            marketPanels[i].DeactivatePanel();
            if (i == selectedMarket){marketPanels[i].ActivatePanel();}
        }
    }
}
