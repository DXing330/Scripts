using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentInventory : BasicDataManager
{

    public string loadedData;
    public List<string> allEquipment;
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
        allEquipment.Clear();
        allEquippedEquipment.Clear();
        Save();
        Load();
    }

    public override void Save()
    {
        saveDataPath = Application.persistentDataPath;
        string data = "";
        data += GameManager.instance.ConvertListToString(allEquipment, "-")+"#";
        data += GameManager.instance.ConvertListToString(allEquippedEquipment, "-");
        File.WriteAllText(saveDataPath+"/equipInventoryData.txt", data);
    }

    private void SaveEquipSets()
    {
        // Keep track of who is equipping it and what the equipment is.
    }

    public override void Load()
    {
        saveDataPath = Application.persistentDataPath;
        if (File.Exists(saveDataPath+"/equipInventoryData.txt"))
        {
            loadedData = File.ReadAllText(saveDataPath+"/equipInventoryData.txt");
            string[] blocks = loadedData.Split("#");
            allEquipment = blocks[0].Split("-").ToList();
            allEquippedEquipment = blocks[1].Split("-").ToList();
        }
        else
        {
            allEquipment.Clear();
            allEquippedEquipment.Clear();
        }
    }
}
