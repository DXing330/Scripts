using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EquipmentSelectGUI : MonoBehaviour
{
    public EquipmentInventory equipInventory;
    public EquipmentSprites equipSprites;
    public PlayerActor selectedActor;
    public EquipmentContainer equipped;
    public MoreStatsSheet statSheet;
    public List<EquipmentSlot> equipSlots;
    public List<string> equipStats;
    public List<TMP_Text> statTexts;
    protected void ResetStatTexts()
    {
        for (int i = 0; i < statTexts.Count; i++)
        {
            statTexts[i].text = "";
        }
    }
    protected void UpdateStatTexts(string selectedEquip)
    {
        ResetStatTexts();
        if (selectedEquip.Length < 6){return;}
        string[] blocks = selectedEquip.Split("|");
        for (int i = 0; i < 3; i++)
        {
            statTexts[i].text = blocks[i+1];
        }
        statTexts[3].text = blocks[0];
        equipStats = blocks[4].Split(",").ToList();
        for (int i = 0; i < equipStats.Count; i++)
        {
            if (equipStats[i].Length <= 1){continue;}
            statTexts[4].text += equipStats[i]+"\n";
        }
        equipStats = blocks[5].Split(",").ToList();
        for (int i = 0; i < equipStats.Count; i++)
        {
            if (equipStats[i].Length <= 1){continue;}
            statTexts[5].text += equipStats[i]+"\n";
        }
    }
    protected void ResetEquipSlots()
    {
        for (int i = 0; i < equipSlots.Count; i++)
        {
            equipSlots[i].DisableImage();
        }
    }
    public void UnequipFromSlot(int slot)
    {
        equipped.Unequip(slot);
        UpdateEquipSlots();
        statSheet.UpdateMoreStats();
    }
    public void ViewSlotStats(int slot)
    {
        UpdateStatTexts(equipped.SlotStats(slot));
    }
    public void UpdateEquipSlots()
    {
        ResetEquipSlots();
        for (int i = 0; i < equipped.allEquipment.Count; i++)
        {
            if (equipped.allEquipment[i].Length > 6)
            {
                equipSlots[i].EnableImage();
                // Only handheld things get unique images.
                if (i <= 1)
                {
                    equipStats = equipped.allEquipment[i].Split("|").ToList();
                    equipSlots[i].UpdateImage(equipSprites.SpriteDictionary(equipStats[^1]));
                }
            }
        }
    }
    void Start()
    {
        equipInventory = GameManager.instance.equipInventory;
        selectedActor = GameManager.instance.armyData.viewStatsActor;
        equipped = selectedActor.allEquipment;
        UpdateEquipSlots();
    }
    /*
    public EquipmentData equipData;
    public Equipment equippedEquipment;
    public Equipment selectedEquipment;
    public List<string> currentActorEquips;
    public List<FormationTile> equippedTiles;
    public int currentInventoryPage = 0;
    public List<string> currentInventory;
    public List<FormationTile> inventoryTiles;
    public int selectedActor = -1;
    public int selectedEquipType = -1;
    public int selectedInventoryIndex = -1;
    public List<TMP_Text> baseStatTexts;
    public List<TMP_Text> allEquipStatTexts;
    public List<TMP_Text> equippedStatTexts;
    public List<TMP_Text> inventoryEquipStatTexts;

    void Start()
    {
        playerActors = GameManager.instance.playerActors;
        equipData = GameManager.instance.equipData;
        equipInventory = GameManager.instance.equipInventory;
        UpdateActorPanels();
    }

    public void SelectSpot(int selectedIndex)
    {
        int index = selectedIndex+currentActorPage*actors.Count;
        if (selectedActor == index){selectedActor = -1;}
        else {selectedActor = index;}
        UpdateActorPanels();
        UpdateStatsPanel();
    }

    private void ResetActorPanels()
    {
        for (int i = 0; i < actors.Count; i++)
        {
            actors[i].ResetActorSprite();
            actors[i].ResetHighlight();
        }
    }

    // The actors that can have equipment.
    private void UpdateActorPanels()
    {
        ResetActorPanels();
        int startIndex = currentActorPage * actors.Count;
        int endIndex = Mathf.Min(playerActors.Count, startIndex + actors.Count);
        for (int i = startIndex; i < endIndex; i++)
        {
            actors[i - startIndex].UpdateActorSprite(actorSprites.SpriteDictionary(playerActors[i].typeName));
            if (selectedActor == i - startIndex)
            {
                actors[i].Highlight();
            }
        }
    }

    private void ResetStatsPanel()
    {
        for (int i = 0; i < baseStatTexts.Count; i++)
        {
            baseStatTexts[i].text = "";
            allEquipStatTexts[i].text = "";
        }
    }

    // The actors base and equipment stats.
    private void UpdateStatsPanel()
    {
        ResetStatsPanel();
        if (selectedActor < 0 || selectedActor >= playerActors.Count){return;}
        PlayerActor currentActor = playerActors[selectedActor];
        currentActor.allEquipment.UpdateStats();
        List<int> currentBaseStats = currentActor.ReturnStatList();
        List<int> currentEquipStats = currentActor.allEquipment.ReturnStatList();
        for (int i = 0; i < currentBaseStats.Count; i++)
        {
            baseStatTexts[i].text = currentBaseStats[i].ToString();
            allEquipStatTexts[i].text = currentEquipStats[i].ToString();
        }
    }

    // Images corresponding to the equipment an actor equipped.
    private void UpdateEquippedPanel()
    {
        if (selectedActor < 0 || selectedActor >= playerActors.Count){return;}
        currentActorEquips = playerActors[selectedActor].ReturnEquippedInList();
        for (int i = 0; i < equippedTiles.Count; i++)
        {
            equippedTiles[i].UpdateActorSprite(equipSprites.SpriteDictionary(currentActorEquips[i]));
        }
    }

    private void UpdateEquipPanelHighlights()
    {
        for (int i = 0; i < equippedTiles.Count; i++)
        {
            equippedTiles[i].ResetHighlight();
            if (i == selectedEquipType && currentActorEquips[i] != "none")
            {
                equippedTiles[i].Highlight();
            }
        }
        UpdateInventoryPanelHighlights();
    }

    private void UpdateInventoryPanelHighlights()
    {
        for (int i = 0; i < inventoryTiles.Count; i++)
        {
            inventoryTiles[i].ResetHighlight();
            // Need to check that there is actually an item there.
            if (i == selectedInventoryIndex)
            {
                inventoryTiles[i].Highlight();
            }
        }
    }

    private void ResetInventoryPanel()
    {
        for (int i = 0; i < inventoryTiles.Count; i++)
        {
            inventoryTiles[i].ResetHighlight();
            inventoryTiles[i].ResetActorSprite();
        }
    }
    
    // Images corresponding to the equipment of some type in the inventory.
    private void UpdateInventoryPanel()
    {
        ResetInventoryPanel();
        int startIndex = currentInventoryPage*inventoryTiles.Count;
        int endIndex = Mathf.Min(startIndex + inventoryTiles.Count, currentInventory.Count);
        for (int i = startIndex; i < endIndex; i++)
        {
            inventoryTiles[i - startIndex].UpdateActorSprite(equipSprites.SpriteDictionary(currentInventory[i]));
        }
    }

    public void SelectEquipType(int type)
    {
        if (selectedEquipType == type){selectedEquipType = -1;}
        else {selectedEquipType = type;}
        selectedInventoryIndex = -1;
        UpdateEquipPanelHighlights();
        UpdateEquipStatsPanel();
        if (selectedEquipType >= 0)
        {
            GetEquipsOfSelectedType();
            UpdateInventoryPanel();
        }
    }

    private void GetEquipsOfSelectedType()
    {
        switch (selectedEquipType)
        {
            case 0:
                currentInventory = equipInventory.allWeapons;
                break;
            case 1:
                currentInventory = equipInventory.allArmors;
                break;
            case 2:
                currentInventory = equipInventory.allHelmets;
                break;
            case 3:
                currentInventory = equipInventory.allBoots;
                break;
            case 4:
                currentInventory = equipInventory.allAccessories;
                break;
        }
    }

    private void ResetEquipStatsPanel()
    {
        for (int i = 0; i < baseStatTexts.Count; i++)
        {
            equippedStatTexts[i].text = "";
            inventoryEquipStatTexts[i].text = "";
        }
    }

    // Specific stats corresponding to a current and selected equipment.
    private void UpdateEquipStatsPanel()
    {
        ResetEquipStatsPanel();
        if (selectedEquipType < 0){return;}
        if (currentActorEquips[selectedEquipType] == "none"){return;}
        equipData.LoadEquipData(equippedEquipment, currentActorEquips[selectedEquipType]);
        List<int> currentBaseStats = equippedEquipment.ReturnStatList();
        for (int i = 0; i < currentBaseStats.Count; i++)
        {
            equippedStatTexts[i].text = currentBaseStats[i].ToString();
        }
        if (selectedInventoryIndex < 0){return;}
        UpdateInventoryStatsPanel();
    }

    private void ResetInventoryStatsPanel()
    {
        for (int i = 0; i < inventoryEquipStatTexts.Count; i++)
        {
            inventoryEquipStatTexts[i].text = "";
        }
    }

    private void UpdateInventoryStatsPanel()
    {
        ResetInventoryStatsPanel();
        int index = selectedInventoryIndex;
        // Ensure that you actually clicked on an equipment.
        if (index < 0 || index >= currentInventory.Count){return;}
        equipData.LoadEquipData(selectedEquipment, currentInventory[index]);
        List<int> currentBaseStats = selectedEquipment.ReturnStatList();
        for (int i = 0; i < currentBaseStats.Count; i++)
        {
            inventoryEquipStatTexts[i].text = currentBaseStats[i].ToString();
        }
    }

    public void SelectInventoryIndex(int index)
    {
        int selected = index + (currentInventoryPage*inventoryTiles.Count);
        if (selectedInventoryIndex == selected){selectedInventoryIndex = -1;}
        else {selectedInventoryIndex = selected;}
        UpdateInventoryPanelHighlights();
        UpdateInventoryStatsPanel();
    }

    public void EquipSelectedEquipment()
    {
        if (selectedEquipType < 0){return;}
        if (selectedInventoryIndex < 0 || selectedInventoryIndex >= currentInventory.Count){return;}
        equipInventory.EquipToActor(selectedActor, selectedEquipType, selectedInventoryIndex);
        UpdateAfterChangingEquipment();
    }

    public void UnequipSelectEquipType()
    {
        if (selectedEquipType < 0){return;}
        equipInventory.UnequipFromActor(selectedActor, selectedEquipType);
        UpdateAfterChangingEquipment();
    }

    private void UpdateAfterChangingEquipment()
    {
        UpdateEquippedPanel();
        UpdateEquipStatsPanel();
        GetEquipsOfSelectedType();
        UpdateInventoryPanel();
    }*/
}
