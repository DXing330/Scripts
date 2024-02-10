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
    public int changeEquipType = -1;
    public void ChangeEquipType(int newType)
    {
        currentInventoryPage = 0;
        selectedEquipIndex = -1;
        if (changeEquipType == newType)
        {
            changeEquipType = -1;
            inventoryObject.SetActive(false);
        }
        else
        {
            changeEquipType = newType;
            inventoryObject.SetActive(true);
            GetPossibleEquips(newType);
            if (possibleEquips.Count <= 0)
            {
                // No possible equips, just hide it.
                ChangeEquipType(newType);
                return;
            }
            UpdateInventoryTiles();
        }
    }
    public List<string> possibleEquips;
    public List<string> possibleEquipTypes;
    protected void GetPossibleEquips(int slot)
    {
        possibleEquips.Clear();
        possibleEquipTypes.Clear();
        switch (slot)
        {
            case 0:
                for (int i = 0; i < equipInventory.allTools.Count; i++)
                {
                    equipStats = equipInventory.allTools[i].Split("|").ToList();
                    if (equipStats.Count < 6){continue;}
                    if (CheckIfEquipable(equipStats[7], int.Parse(equipStats[8])))
                    {
                        possibleEquips.Add(equipInventory.allTools[i]);
                        // Keep track of the name for easier image lookup.
                        possibleEquipTypes.Add(equipStats[9]);
                    }
                }
                break;
            case 1:
                for (int i = 0; i < equipInventory.allTools.Count; i++)
                {
                    equipStats = equipInventory.allTools[i].Split("|").ToList();
                    if (equipStats.Count < 6){continue;}
                    if (CheckIfEquipable(equipStats[7], int.Parse(equipStats[8])))
                    {
                        possibleEquips.Add(equipInventory.allTools[i]);
                        possibleEquipTypes.Add(equipStats[9]);
                    }
                }
                break;
            case 2:
                for (int i = 0; i < equipInventory.allArmors.Count; i++)
                {
                    equipStats = equipInventory.allArmors[i].Split("|").ToList();
                    if (equipStats.Count < 6){continue;}
                    if (CheckIfEquipable(equipStats[7], int.Parse(equipStats[8])))
                    {
                        possibleEquips.Add(equipInventory.allArmors[i]);
                        possibleEquipTypes.Add(equipStats[9]);
                    }
                }
                break;
            case 3:
                for (int i = 0; i < equipInventory.allAccessories.Count; i++)
                {
                    equipStats = equipInventory.allAccessories[i].Split("|").ToList();
                    if (equipStats.Count < 6){continue;}
                    if (CheckIfEquipable(equipStats[7], int.Parse(equipStats[8])))
                    {
                        possibleEquips.Add(equipInventory.allAccessories[i]);
                        possibleEquipTypes.Add(equipStats[9]);
                    }
                }
                break;
        }
    }
    protected bool CheckIfEquipable(string bodyType, int size)
    {
        // Need to be the right size and body type to equip something.
        if (size != selectedActor.size){return false;}
        if (!selectedActor.species.Contains(bodyType)){return false;}
        return true;
    }
    public int currentInventoryPage = 0;
    public void ChangeInventoryPage(bool right = true)
    {
        int lastPage = (possibleEquips.Count/inventoryTiles.Count)-1;
        if (right)
        {
            if (currentInventoryPage < lastPage)
            {
                currentInventoryPage++;
            }
            else
            {
                currentInventoryPage = 0;
            }
        }
        else
        {
            if (currentInventoryPage > 0)
            {
                currentInventoryPage--;
            }
            else
            {
                currentInventoryPage = lastPage;
            }
        }
        selectedEquipIndex = -1;
        UpdateInventoryTiles();
    }
    public int selectedEquipIndex = -1;
    public void SelectEquipInInventory(int index)
    {
        selectedEquipIndex = index;
        UpdateSelectedHighlight();
        UpdateStatTexts(possibleEquips[index + currentInventoryPage*inventoryTiles.Count]);
    }
    public void EquipSelectedEquip()
    {
        if (selectedEquipIndex < 0){return;}
        string equip = possibleEquips[selectedEquipIndex+(currentInventoryPage*inventoryTiles.Count)];
        equipInventory.EquipToActor(equip, selectedActor, changeEquipType);
        statSheet.UpdateMoreStats();
        UpdateEquipSlots();
        selectedEquipIndex = -1;
        ResetInventoryHighlights();
    }
    public GameObject inventoryObject;
    public List<FormationTile> inventoryTiles;
    public List<GameObject> inventoryTileObjects;
    protected void ResetInventoryTileObjects()
    {
        for (int i = 0; i < inventoryTileObjects.Count; i++)
        {
            inventoryTileObjects[i].SetActive(false);
        }
    }
    protected void ResetInventoryHighlights()
    {
        for (int i = 0; i < inventoryTiles.Count; i++)
        {
            inventoryTiles[i].ResetHighlight();
        }
    }
    protected void UpdateSelectedHighlight()
    {
        ResetInventoryHighlights();
        if (selectedEquipIndex < 0){return;}
        inventoryTiles[selectedEquipIndex].Highlight();
    }
    protected void UpdateInventoryTiles()
    {
        ResetInventoryTileObjects();
        ResetInventoryHighlights();
        int startIndex = currentInventoryPage*inventoryTiles.Count;
        int endIndex = Mathf.Min(startIndex + inventoryTiles.Count, possibleEquips.Count);
        for (int i = startIndex; i < endIndex; i++)
        {
            inventoryTileObjects[i].SetActive(true);
            if (changeEquipType <= 1)
            {
                inventoryTiles[i - startIndex].UpdateActorSprite(equipSprites.SpriteDictionary(possibleEquipTypes[i]));
            }
            // Hardcoded for now until more/better sprites are obtained.
            else if (changeEquipType == 2)
            {
                inventoryTiles[i - startIndex].UpdateActorSprite(equipSprites.SpriteDictionary("ArmorIcon"));
            }
            else if (changeEquipType == 3)
            {
                inventoryTiles[i - startIndex].UpdateActorSprite(equipSprites.SpriteDictionary("RingIcon"));
            }
        }
    }
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
    public List<EquipmentSlot> equipSlots;
    protected void ResetEquipSlots()
    {
        for (int i = 0; i < equipSlots.Count; i++)
        {
            equipSlots[i].DisableImage();
        }
    }
    public void UnequipFromSlot(int slot)
    {
        equipInventory.UnequipFromActor(selectedActor, slot);
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
