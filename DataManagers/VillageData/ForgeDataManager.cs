using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgeDataManager : BasicDataManager
{
    public List<string> craftableEquipment;
    public List<string> forgeWorkers;
    public List<string> levels;
    public List<string> experiences;
    public List<string> craftEquipment;
    public List<string> craftDay;
    public List<string> craftTime;

    public override void Save()
    {
        saveDataPath = Application.persistentDataPath;
        string data = "";
        data += GameManager.instance.ConvertListToString(craftableEquipment)+"#";
        data += GameManager.instance.ConvertListToString(forgeWorkers)+"#";
        data += GameManager.instance.ConvertListToString(levels)+"#";
        data += GameManager.instance.ConvertListToString(experiences)+"#";
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
            forgeWorkers = blocks[1].Split("|").ToList();
            levels = blocks[2].Split("|").ToList();
            experiences = blocks[3].Split("|").ToList();
            craftEquipment = blocks[4].Split(",").ToList();
            craftDay = blocks[5].Split("|").ToList();
            craftTime = blocks[6].Split("|").ToList();
        }
        else
        {
            NewGame();
        }
    }
}
