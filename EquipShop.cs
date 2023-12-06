using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EquipShop : MonoBehaviour
{
    public EquipmentSprites equipSprites;
    public List<string> equips;
    public List<int> prices;
    public int currentViewedPage = 0;
    public int currentlySelectedEquip = -1;
    public List<FormationTile> availableEquips;
    public TMP_Text currencyText;
    public TMP_Text selectedEquip;
    public TMP_Text selectedPrice;

    void Start()
    {
        UpdateEquipImages();
        UpdateInfo();
    }

    public void SelectEquip(int index)
    {
        int selected = currentViewedPage*availableEquips.Count+index;
        if (currentlySelectedEquip == selected){currentlySelectedEquip = -1;}
        else{currentlySelectedEquip = selected;}
        UpdateInfo();
        UpdateHighlights();
    }

    private void ResetInfo()
    {
        selectedEquip.text = "";
        selectedPrice.text = "";
    }

    private void UpdateInfo()
    {
        currencyText.text = GameManager.instance.goldCoins.ToString();
        ResetInfo();
        if (currentlySelectedEquip < 0 || currentlySelectedEquip >= equips.Count){return;}
        selectedEquip.text = equips[currentlySelectedEquip];
        selectedPrice.text = prices[currentlySelectedEquip].ToString();
    }

    private void ResetEquipImages()
    {
        for (int i = 0; i < availableEquips.Count; i++)
        {
            availableEquips[i].ResetHighlight();
            availableEquips[i].ResetActorSprite();
        }
    }

    private void UpdateEquipImages()
    {
        ResetEquipImages();
        for (int i = 0; i < availableEquips.Count; i++)
        {
            availableEquips[i].UpdateActorSprite(equipSprites.SpriteDictionary(equips[i]));
        }
    }

    private void ResetHighlights()
    {
        for (int i = 0; i < availableEquips.Count; i++)
        {
            availableEquips[i].ResetHighlight();
        }
    }

    private void UpdateHighlights()
    {
        ResetHighlights();
        for (int i = 0; i < availableEquips.Count; i++)
        {
            if (i == currentlySelectedEquip)
            {
                availableEquips[i].Highlight();
            }
        }
    }

    public void BuyEquip()
    {
        if (currentlySelectedEquip < 0 || currentlySelectedEquip >= equips.Count){return;}
        // Check price.
        if (GameManager.instance.goldCoins >= prices[currentlySelectedEquip])
        {
            GameManager.instance.goldCoins -= prices[currentlySelectedEquip];
            UpdateInfo();
        }
        else{return;}
        // Obtain equipment.
        GameManager.instance.equipInventory.GainEquipment(equips[currentlySelectedEquip]);
    }
}
