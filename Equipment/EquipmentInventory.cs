using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentInventory : BasicDataManager
{
    private string saveDataPath;
    private string loadedData;
    public List<string> allEquipment;

    public override void NewGame()
    {
        allEquipment.Clear();
        Save();
        Load();
    }

    public override void Save()
    {
        saveDataPath = Application.persistentDataPath;
        string data = "";
        data += GameManager.instance.ConvertListToString(allEquipment)+"#";
        File.WriteAllText(saveDataPath+"/equipInventoryData.txt", data);
    }

    public override void Load()
    {
        saveDataPath = Application.persistentDataPath;
        loadedData = File.ReadAllText(saveDataPath+"/upgradeData.txt");
        string[] blocks = loadedData.Split("#");
        allEquipment = blocks[0].Split("|").ToList();
    }

    public void GainEquipment(string newEquipment)
    {
        allEquipment.Add(newEquipment);
    }

    public void LoseEquipment(int index)
    {
        allEquipment.RemoveAt(index);
    }
}
