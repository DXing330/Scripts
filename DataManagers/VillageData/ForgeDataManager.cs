using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgeDataManager : BasicDataManager
{
    public List<string> craftableEquipment;
    public List<string> craftEquipment;
    public List<string> craftDay;
    public List<string> craftTime;

    public override void Save()
    {
        saveDataPath = Application.persistentDataPath;
        string data = "";
        data += GameManager.instance.ConvertListToString(craftableEquipment)+"#";
        data += GameManager.instance.ConvertListToString(craftEquipment, ",")+"#";
        data += GameManager.instance.ConvertListToString(craftDay)+"#";
        data += GameManager.instance.ConvertListToString(craftTime)+"#";
        File.WriteAllText(saveDataPath+fileName, data);
    }

    public override void Load()
    {
        saveDataPath = Application.persistentDataPath;
        if (File.Exists(saveDataPath+fileName))
        {
            loadedData = File.ReadAllText(saveDataPath+fileName);
            string[] blocks = loadedData.Split("#");
            craftableEquipment = blocks[0].Split("|").ToList();
            craftEquipment = blocks[1].Split(",").ToList();
            craftDay = blocks[2].Split("|").ToList();
            craftTime = blocks[3].Split("|").ToList();
            GameManager.instance.RemoveEmptyListItems(craftEquipment,0);
            GameManager.instance.RemoveEmptyListItems(craftDay,0);
            GameManager.instance.RemoveEmptyListItems(craftTime,0);
        }
        else
        {
            NewGame();
        }
    }

    public void StartCrafting(string equip, string craftingTime = "30")
    {
        craftEquipment.Add(equip);
        craftDay.Add(GameManager.instance.time.ToString());
        craftTime.Add(craftingTime);
    }

    public bool FinishedCrafting(int index)
    {
        if (int.Parse(craftDay[index])+int.Parse(craftTime[index]) > GameManager.instance.time)
        {
            GameManager.instance.equipInventory.GainEquipment(craftEquipment[index]);
            craftEquipment.RemoveAt(index);
            craftDay.RemoveAt(index);
            craftTime.RemoveAt(index);
            return true;
        }
        return false;
    }
}
