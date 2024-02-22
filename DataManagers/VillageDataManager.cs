using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageDataManager : BasicDataManager
{
    public string starterVillage;
    public string villageData;
    // village includes terrain info
        // terrain info needs dimensions and terrain types
    public int totalRows; // 9
    public int totalColumns; // 9
    public List<string> villageTiles; // figure it out
    // Keep track of resources I guess.
        // money|food|materials|etc.
    public List<string> resources; // 0|2|0
    // Have some workers than you can manage/automate.
        // Maybe give them names and other attributes?
    public List<string> workers; // alex|bob
    // skill format is skill=level,skill2=level2,etc.
    public List<string> workerSkills; // 1=1|1=1 indicates farming skill level 1
    public List<string> workerLocations; // -1|-1 indicates unassigned workers
    // village has buildings on top of terrain
    public List<string> buildings; // 0|1|2 = city center | farm | housing
    // Need to shift the buildings around whenever the village expands.
    public List<string> buildingLocations; // 40|30|39
    public List<string> buildingLevels; // 1|1|1
    public List<string> buildingHealths; // 100|20|30
    public List<string> buildingPhaseBuildings; // empty
    public List<string> buildTimes; // empty

    public override void NewGame()
    {
        saveDataPath = Application.persistentDataPath;
        if (File.Exists(saveDataPath+"/village.txt"))
        {
            File.Delete (saveDataPath+"/village.txt");
        }
        villageData = starterVillage;
        QuickSave();
        Load();
    }

    public override void Save()
    {
        saveDataPath = Application.persistentDataPath;
        string data = "";
        data += totalRows+"#"+totalColumns+"#";
        data += GameManager.instance.ConvertListToString(villageTiles)+"#";
        data += GameManager.instance.ConvertListToString(resources)+"#";
        data += GameManager.instance.ConvertListToString(workers)+"#";
        data += GameManager.instance.ConvertListToString(workerSkills)+"#";
        data += GameManager.instance.ConvertListToString(workerLocations)+"#";
        data += GameManager.instance.ConvertListToString(buildings)+"#";
        data += GameManager.instance.ConvertListToString(buildingLocations)+"#";
        data += GameManager.instance.ConvertListToString(buildingLevels)+"#";
        data += GameManager.instance.ConvertListToString(buildingHealths)+"#";
        data += GameManager.instance.ConvertListToString(buildingPhaseBuildings)+"#";
        data += GameManager.instance.ConvertListToString(buildTimes)+"#";
        File.WriteAllText(saveDataPath+"/village.txt", data);
    }

    protected void QuickSave()
    {
        saveDataPath = Application.persistentDataPath;
        string data = villageData;
        File.WriteAllText(saveDataPath+"/village.txt", data);
    }

    public override void Load()
    {
        saveDataPath = Application.persistentDataPath;
        if (File.Exists(saveDataPath+"/village.txt"))
        {
            loadedData = File.ReadAllText(saveDataPath+"/village.txt");
            villageData = loadedData;
        }
        else
        {
            villageData = starterVillage;
        }
        SetData();
    }

    protected void SetData()
    {
        string[] dataBlocks = villageData.Split("#");
        totalRows = int.Parse(dataBlocks[0]);
        totalColumns = int.Parse(dataBlocks[1]);
        villageTiles = dataBlocks[2].Split("|").ToList();
        resources = dataBlocks[3].Split("|").ToList();
        workers = dataBlocks[4].Split("|").ToList();
        workerSkills = dataBlocks[5].Split("|").ToList();
        workerLocations = dataBlocks[6].Split("|").ToList();
        buildings = dataBlocks[7].Split("|").ToList();
        buildingLocations = dataBlocks[8].Split("|").ToList();
        buildingLevels = dataBlocks[9].Split("|").ToList();
        buildingHealths = dataBlocks[10].Split("|").ToList();
        buildingPhaseBuildings = dataBlocks[11].Split("|").ToList();
        buildTimes = dataBlocks[12].Split("|").ToList();
    }
}