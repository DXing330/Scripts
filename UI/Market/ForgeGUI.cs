using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ForgeGUI : MarketPanelGUI
{
    public ForgeDataManager forgeData;
    void Start()
    {
        forgeData = GameManager.instance.villageData.market.forgeData;
        UpdateViewed();
        if (forgeData.craftEquipment.Count > 1){GameManager.instance.utility.EnableAllObjects(changePageButtons);}
    }
    public GameObject viewCraftingObject;
    public EquipmentStatsUI viewedCrafting;
    public List<StatImageText> otherInfo;
    public int currentQuality = 1;
    public TMP_Text qualityText;
    public TMP_Text viewedIndexText;
    public TMP_Text viewedTimeLeftText;
    public List<GameObject> changePageButtons;
    public int currentIndex = 0;
    public void ChangeIndex(bool right = true)
    {
        int maxIndex = forgeData.craftEquipment.Count - 1;
        if (right)
        {
            if (currentIndex < maxIndex){currentIndex++;}
            else{currentIndex = 0;}
        }
        else
        {
            if (currentIndex > 0){currentIndex--;}
            else{currentIndex = maxIndex;}
        }
        UpdateViewed();
    }

    public void ChangeQuality(bool increase = true)
    {
        // Check if the quality is less than market level.
        // Change the time and price.
        // Change the quality text.
    }

    protected void ResetViewed()
    {
        viewCraftingObject.SetActive(false);
        viewedIndexText.text = "";
        viewedTimeLeftText.text = "";
    }

    protected void UpdateViewed()
    {
        if (forgeData.craftEquipment.Count <= 0)
        {
            ResetViewed();
            return;
        }
        viewedCrafting.UpdateStatTextsFromString(forgeData.craftEquipment[currentIndex]);
        viewedIndexText.text = (currentIndex+1)+"/"+forgeData.craftEquipment.Count;
        int timePassed = GameManager.instance.timeDifference(int.Parse(forgeData.craftDay[currentIndex]));
        viewedTimeLeftText.text = Mathf.Max(0, int.Parse(forgeData.craftTime[currentIndex]) - timePassed)+" Day(s) Left";
    }
}