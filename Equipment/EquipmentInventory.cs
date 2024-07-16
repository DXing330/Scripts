using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentInventory : BasicDataManager
{
    public List<string> starterEquipment;
    public List<string> allEquipment;
    public string ReturnEquipment(int slot, int user = -1)
    {
        if (user == -1)
        {
            // Get it from all equipment.
            if (slot < allEquipment.Count)
            {
                return allEquipment[slot];
            }
        }
        else
        {
            string[] userEquips = allEquippedEquipment[user].Split("@");
            if (slot < userEquips.Length){return userEquips[slot];}
        }
        return "";
    }
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
    public void LostEquipment(string equipment)
    {
        allEquipment.Remove(equipment);
        SortEquipmentIntoLists();
    }
    public void EnchantEquipment(string enchantment, int slot, int user = -1)
    {
        if (user == -1 && slot < allEquipment.Count)
        {
            List<string> equipToEnchant = allEquipment[slot].Split("|").ToList();
            equipToEnchant[5] += ","+enchantment;
            allEquipment[slot] = GameManager.instance.utility.ConvertListToString(equipToEnchant);
        }
        else
        {
            List<string> usersEquips = allEquippedEquipment[user].Split("@").ToList();
            List<string> equipToEnchant = usersEquips[slot].Split("|").ToList();
            equipToEnchant[5] += ","+enchantment;
            usersEquips[slot] = GameManager.instance.utility.ConvertListToString(equipToEnchant);
            allEquippedEquipment[user] = GameManager.instance.utility.ConvertListToString(usersEquips, "@");
        }
    }
    public List<string> allEquippedEquipment;
    public void LoseEquipSets(int index)
    {
        if (index >= allEquippedEquipment.Count){return;}
        for (int i = 0; i < allEquippedEquipment.Count; i++)
        {
            if (i >= index){allEquippedEquipment.RemoveAt(i);}
        }
        Save();
    }
    public void LoseEquipSetsAtIndices(List<int> indices)
    {
        for (int i = 0; i < indices.Count; i++)
        {
            if (indices[i] >= allEquippedEquipment.Count){continue;}
            allEquippedEquipment.RemoveAt(indices[i]);
        }
        Save();
    }
    public List<string> allTools;
    public List<string> allArmors;
    public List<string> allAccessories;

    protected void SortEquipmentIntoLists()
    {
        allTools.Clear();
        allArmors.Clear();
        allAccessories.Clear();
        if (allEquipment.Count <= 0){return;}
        GameManager.instance.utility.RemoveEmptyListItems(allEquipment);
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

    [ContextMenu("New Game")]
    public override void NewGame()
    {
        saveDataPath = Application.persistentDataPath;
        if (File.Exists(saveDataPath+fileName))
        {
            File.Delete(saveDataPath+fileName);
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
        data += GameManager.instance.utility.ConvertListToString(allEquipment, "+")+"#";
        data += GameManager.instance.utility.ConvertListToString(allEquippedEquipment, "+");
        File.WriteAllText(saveDataPath+fileName, data);
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
        string[] data = equipment.Split("|");
        int trueSlot = int.Parse(data[6]);
        if (trueSlot < 0)
        {
            string equip = actor.allEquipment.Unequip(0);
            if (equip.Length > 6){GainEquipment(equip);}
            equip = actor.allEquipment.Unequip(1);
            if (equip.Length > 6){GainEquipment(equip);}
            actor.allEquipment.Equip(equipment, slot);
        }
        else
        {
            // First check if it's equipable.
            if (actor.allEquipment.Equipable(equipment,slot))
            {
                string equip = actor.allEquipment.Unequip(slot);
                if (equip.Length > 6){GainEquipment(equip);}
                actor.allEquipment.Equip(equipment, slot);
            }
            // If not then nothing changes.
            else{return;}
        }
        LostEquipment(equipment);
        SaveEquipSets();
        GameManager.instance.armyData.UpdatePartyStats();
    }

    public void UnequipFromActor(PlayerActor actor, int slot)
    {
        string equip = actor.allEquipment.Unequip(slot);
        if (equip.Length > 6){GainEquipment(equip);}
        SaveEquipSets();
        GameManager.instance.armyData.UpdatePartyStats();
    }

    public override void Load()
    {
        saveDataPath = Application.persistentDataPath;
        if (File.Exists(saveDataPath+fileName))
        {
            loadedData = File.ReadAllText(saveDataPath+fileName);
            string[] blocks = loadedData.Split("#");
            // Can't use minus sign as delimiter since -1 can be a slot.
            // Can't use | or # obviously.
            allEquipment = blocks[0].Split("+").ToList();
            allEquippedEquipment = blocks[1].Split("+").ToList();
            GameManager.instance.utility.RemoveEmptyListItems(allEquipment);
            GameManager.instance.utility.RemoveEmptyListItems(allEquippedEquipment);
        }
        else
        {
            allEquipment.Clear();
            allEquippedEquipment.Clear();
        }
        SortEquipmentIntoLists();
    }
}
