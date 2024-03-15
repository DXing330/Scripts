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
    public void GetViewedActor()
    {
        selectedActor = GameManager.instance.armyData.viewStatsActor;
        SetEquipped(selectedActor.allEquipment);
        UpdateEquipSlots();
    }
    public void SetActor(PlayerActor actor)
    {
        selectedActor = actor;
        SetEquipped(selectedActor.allEquipment);
        UpdateEquipSlots();
    }
    public EquipmentContainer equipped;
    protected void SetEquipped(EquipmentContainer container)
    {
        equipped = container;
    }
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
        for (int i = 0; i < equipSlots.Count; i++)
        {
            equipSlots[i].ResetTitleColor();
            if (i == changeEquipType)
            {
                equipSlots[i].HighlightTitle();
            }
        }
    }
    public List<string> possibleEquips;
    public List<int> possibleEquipRarities;
    public List<string> possibleEquipTypes;
    protected void GetPossibleEquips(int slot)
    {
        possibleEquips.Clear();
        possibleEquipRarities.Clear();
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
                        possibleEquipRarities.Add(int.Parse(equipStats[equipStats.Count-2]));
                        // Keep track of the name for easier image lookup.
                        possibleEquipTypes.Add(equipStats[^1]);
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
                        possibleEquipRarities.Add(int.Parse(equipStats[equipStats.Count-2]));
                        possibleEquipTypes.Add(equipStats[^1]);
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
                        possibleEquipRarities.Add(int.Parse(equipStats[equipStats.Count-2]));
                        possibleEquipTypes.Add(equipStats[^1]);
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
                        possibleEquipRarities.Add(int.Parse(equipStats[equipStats.Count-2]));
                        possibleEquipTypes.Add(equipStats[^1]);
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
        int lastPage = (possibleEquips.Count/inventoryTiles.Count);
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
            inventoryTileObjects[i - startIndex].SetActive(true);
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
            Color tempColor = equipSlots[0].ReturnColorBasedOnRarity(possibleEquipRarities[i]);
            inventoryTiles[i - startIndex].UpdateActorColor(tempColor);
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
                // Pass the rarity to the equip slot and let them determine what color it should be.
                equipSlots[i].UpdateColorBasedOnRarity(int.Parse(equipStats[equipStats.Count - 2]));
            }
        }
    }
    void Start()
    {
        equipInventory = GameManager.instance.equipInventory;
        selectedActor = GameManager.instance.armyData.viewStatsActor;
        if (selectedActor != null)
        {
            equipped = selectedActor.allEquipment;
            UpdateEquipSlots();
        }
    }
}
