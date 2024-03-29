using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentInventory : BasicDataManager
{
    public List<string> starterEquipment;
    public List<string> allEquipment;
    public void GainEquipment(string newEquip)
    {
        allEquipment.Add(newEquip);
        SortEquipmentIntoLists();
    }
    public void RemoveEquipment(int index)
    {
        if (index < allEquipment.Count && index >= 0)
        {
            allEquipment.RemoveAt(index);
        }
        SortEquipmentIntoLists();
    }
    public List<string> allEquippedEquipment;
    public List<string> allTools;
    public List<string> allArmors;
    public List<string> allAccessories;

    protected void SortEquipmentIntoLists()
    {
        allTools.Clear();
        allArmors.Clear();
        allAccessories.Clear();
        for (int i = 0; i < allEquipment.Count; i++)
        {
            string[] data = allEquipment[i].Split("|");
            SortEquip(allEquipment[i], int.Parse(data[6]));
        }
    }

    protected void SortEquip(string equip, int equipType)
    {
        // -1 = twohanded, 0 = one handed tool, 1 = armor, 2 = accessory
        switch (equipType)
        {
            case 1:
                allArmors.Add(equip);
                return;
            case 2:
                allAccessories.Add(equip);
                return;
        }
        allTools.Add(equip);
    }

    public override void NewGame()
    {
        saveDataPath = Application.persistentDataPath;
        if (File.Exists(saveDataPath+"/equipInventoryData.txt"))
        {
            File.Delete (saveDataPath+"/equipInventoryData.txt");
        }
        allEquipment = starterEquipment;
        allEquippedEquipment.Clear();
        Save();
        Load();
    }

    public override void Save()
    {
        saveDataPath = Application.persistentDataPath;
        string data = "";
        data += GameManager.instance.ConvertListToString(allEquipment, "+")+"#";
        data += GameManager.instance.ConvertListToString(allEquippedEquipment, "+");
        File.WriteAllText(saveDataPath+"/equipInventoryData.txt", data);
    }

    public void SaveEquipSets()
    {
        // Keep track of who is equipping it and what the equipment is.
        allEquippedEquipment.Clear();
        for (int i = 0; i < GameManager.instance.armyData.allPartyMembers.Count; i++)
        {
            allEquippedEquipment.Add(GameManager.instance.armyData.allPartyMembers[i].allEquipment.ReturnEquippedString());
        }
    }

    public void LoadEquipSets()
    {
        for (int i = 0; i < allEquippedEquipment.Count; i++)
        {
            GameManager.instance.armyData.allPartyMembers[i].allEquipment.LoadEquipSet(allEquippedEquipment[i]);
        }
    }

    public void EquipToActor(string equipment, PlayerActor actor, int slot)
    {
        // if slot = -1, remove any tools and add them back.
        actor.allEquipment.Equip(equipment, slot);
        SaveEquipSets();
        GameManager.instance.armyData.UpdatePartyStats();
    }

    public void UnequipFromActor(PlayerActor actor, int slot)
    {
        actor.allEquipment.Unequip(slot);
        SaveEquipSets();
        GameManager.instance.armyData.UpdatePartyStats();
    }

    public override void Load()
    {
        saveDataPath = Application.persistentDataPath;
        if (File.Exists(saveDataPath+"/equipInventoryData.txt"))
        {
            loadedData = File.ReadAllText(saveDataPath+"/equipInventoryData.txt");
            string[] blocks = loadedData.Split("#");
            // Can't use minus sign as delimiter since -1 can be a slot.
            // Can't use | or # obviously.
            allEquipment = blocks[0].Split("+").ToList();
            allEquippedEquipment = blocks[1].Split("+").ToList();
        }
        else
        {
            allEquipment.Clear();
            allEquippedEquipment.Clear();
        }
        SortEquipmentIntoLists();
    }
}
