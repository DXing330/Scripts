using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GeneralPopulationDataManager : BasicDataManager
{
    // All the village data stuff will keep track of the main village.
    public VillageDataManager villageData;
    public string fileName = "/generalpop.txt";
    public string population;
    public string morale;
    public override void NewGame()
    {
        saveDataPath = Application.persistentDataPath;
        if (File.Exists(saveDataPath+fileName))
        {
            File.Delete (saveDataPath+fileName);
        }
        Save();
        Load();
    }

    public override void Save()
    {
        saveDataPath = Application.persistentDataPath;
        string data = "";
        File.WriteAllText(saveDataPath+fileName, data);
    }

    public override void Load()
    {
        saveDataPath = Application.persistentDataPath;
        if (File.Exists(saveDataPath+fileName))
        {
            loadedData = File.ReadAllText(saveDataPath+fileName);
        }
        else
        {
            NewGame();
        }
    }
}
