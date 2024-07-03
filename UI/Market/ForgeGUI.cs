using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ForgeGUI : MarketPanelGUI
{
    public ForgeDataManager forgeData;
    public SpriteContainer equipSprites;
    void Start()
    {
        forgeData = GameManager.instance.villageData.market.forgeData;
        UpdateViewed();
        UpdateOrderStats();
    }
    public GameObject viewCraftingObject;
    public EquipmentStatsUI viewedCrafting;
    public StatImageText viewedEquipIcon;
    public List<StatImageText> otherInfo;
    public StatImageText orderedEquipIcon;
    public int currentOrder = 0;
    public int currentQuality = 0;
    public TMP_Text qualityText;
    public ScriptableDictionary qualityTextDict;
    public TMP_Text viewedIndexText;
    public TMP_Text viewedTimeLeftText;
    public int currentIndex = 0;
    public void ChangeIndex(bool right = true)
    {
        int maxIndex = forgeData.craftEquipment.Count - 1;
        currentIndex = GameManager.instance.utility.ChangeIndex(currentIndex, right, maxIndex);
        UpdateViewed();
    }

    public void UpdateOrderStats()
    {
        // Generate the equipment based on what your ordering.
        string orderedEquip = forgeData.craftableEquipment[currentOrder];
        orderedEquipIcon.SetSprite(equipSprites.SpriteDictionary(orderedEquip));
    }
    
    public void ChangeOrder(bool right = true)
    {
        int maxIndex = forgeData.craftableEquipment.Count - 1;
        if (maxIndex == 0){return;}
        currentOrder = GameManager.instance.utility.ChangeIndex(currentOrder, right, maxIndex);
        currentQuality = 0;
        UpdateQualityText();
        UpdateOrderStats();
    }

    protected void UpdateQualityText()
    {
        qualityText.text = qualityTextDict.ReturnValueByIndex(currentQuality);
    }

    public void ChangeQuality(bool increase = true)
    {
        // Check if the quality is less than market level.
        currentQuality = GameManager.instance.utility.ChangeIndex(currentQuality, increase, qualityTextDict.values.Count-1);
        // Change the time and price.
        UpdateQualityText();
        UpdateOrderStats();
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