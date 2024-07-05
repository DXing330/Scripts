using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgeDataManager : BasicDataManager
{
    public EquipmentGenerator equipmentGenerator;
    public List<string> craftableEquipment;
    public List<string> baseCraftTime;
    public List<string> baseCraftCost;
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
        data += GameManager.instance.ConvertListToString(baseCraftTime)+"#";
        data += GameManager.instance.ConvertListToString(baseCraftCost)+"#";
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
            baseCraftTime = blocks[4].Split("|").ToList();
            baseCraftCost = blocks[5].Split("|").ToList();
            GameManager.instance.RemoveEmptyListItems(craftEquipment,0);
            GameManager.instance.RemoveEmptyListItems(craftDay,0);
            GameManager.instance.RemoveEmptyListItems(craftTime,0);
        }
        else
        {
            NewGame();
        }
    }

    public int ReturnCost(int index, int quality)
    {
        return int.Parse(baseCraftCost[index])*((quality+1)*(quality+1)*(quality+1));
    }

    public int ReturnTime(int index, int quality)
    {
        return int.Parse(baseCraftTime[index])*(int)(Mathf.Pow(2, quality));
    }
    
    public void StartCrafting(string equip, int index, int quality)
    {
        craftEquipment.Add(equip);
        craftDay.Add(GameManager.instance.time.ToString());
        craftTime.Add(ReturnTime(index, quality).ToString());
    }

    public bool FinishedCrafting(int index)
    {
        if (int.Parse(craftDay[index])+int.Parse(craftTime[index]) > GameManager.instance.time)
        {
            GameManager.instance.equipInventory.GainEquipment(craftEquipment[index]);
            craftEquipment.RemoveAt(index);
            craftDay.RemoveAt(index);
            craftTime.RemoveAt(index);
            // Maybe send a message or something that the equipment is done crafting.
            return true;
        }
        return false;
    }
}
