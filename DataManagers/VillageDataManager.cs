using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageDataManager : BasicDataManager
{
    public BuildingDataManager buildingData;
    public string starterVillage;
    public string villageData;
    // village includes terrain info
        // terrain info needs dimensions and terrain types
    public int totalRows; // 9
    public int totalColumns; // 9
    public List<List<string>> allStats = new List<List<string>>();
    protected void SetAllStats()
    {
        allStats.Clear();
        allStats.Add(villageTiles);
        allStats.Add(resources);
        allStats.Add(workers);
        allStats.Add(workerFamilySize);
        allStats.Add(workerHealth);
        allStats.Add(workerSkills);
        allStats.Add(workerLocations);
        allStats.Add(buildings);
        allStats.Add(buildingLocations);
        allStats.Add(buildingLevels);
        allStats.Add(buildingHealths);
        allStats.Add(buildingPhaseBuildings);
        allStats.Add(buildTimes);
    }
    public List<string> villageTiles; // figure it out
    // Keep track of resources I guess.
        // money|food|materials|woman
        // women can be married to increase loyalty and make it harder to for workers to leave.
    public List<string> resources; // 0|2|0|0
    // Have some workers than you can manage/automate.
        // Maybe give them names and other attributes?
    public List<string> workers; // alex|bob
    // skill format is skill=level,skill2=level2,etc.
    public List<string> workerFamilySize; // 1|1 indicates size of family ie cost to feed
    public List<string> workerHealth; // 20|20 max is 20, 0 = death, deal with family somehow
    public List<string> workerSkills; // 1=100|1=100 indicates farming skill level 1 // skill levels = sqrt(value/100), very slow to increase skills later
    public List<string> workerLocations; // 39|31 indicates unassigned workers
    // village has buildings on top of terrain
    public List<string> buildings; // 0|1|2|2 = city center | farm | housing
    // Need to shift the buildings around whenever the village expands.
    public List<string> buildingLocations; // 40|30|39|31
    public List<string> buildingLevels; // 1|1|1|1
    public List<string> buildingHealths; // 200|40|60|60
    public List<string> buildingPhaseBuildings; // empty
    public List<string> buildingPhaseLocations; // empty
    public List<string> buildTimes; // empty
    protected List<int> projectedOutputs = new List<int>();

    [ContextMenu("New Game")]
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
        data += GameManager.instance.ConvertListToString(workerFamilySize)+"#";
        data += GameManager.instance.ConvertListToString(workerHealth)+"#";
        data += GameManager.instance.ConvertListToString(workerSkills)+"#";
        data += GameManager.instance.ConvertListToString(workerLocations)+"#";
        data += GameManager.instance.ConvertListToString(buildings)+"#";
        data += GameManager.instance.ConvertListToString(buildingLocations)+"#";
        data += GameManager.instance.ConvertListToString(buildingLevels)+"#";
        data += GameManager.instance.ConvertListToString(buildingHealths)+"#";
        data += GameManager.instance.ConvertListToString(buildingPhaseBuildings)+"#";
        data += GameManager.instance.ConvertListToString(buildingPhaseLocations)+"#";
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
        workerFamilySize = dataBlocks[5].Split("|").ToList();
        workerHealth = dataBlocks[6].Split("|").ToList();
        workerSkills = dataBlocks[7].Split("|").ToList();
        workerLocations = dataBlocks[8].Split("|").ToList();
        buildings = dataBlocks[9].Split("|").ToList();
        buildingLocations = dataBlocks[10].Split("|").ToList();
        buildingLevels = dataBlocks[11].Split("|").ToList();
        buildingHealths = dataBlocks[12].Split("|").ToList();
        buildingPhaseBuildings = dataBlocks[13].Split("|").ToList();
        buildingPhaseLocations = dataBlocks[14].Split("|").ToList();
        buildTimes = dataBlocks[15].Split("|").ToList();
    }

    public int DetermineHousingLimit()
    {
        int limit = 0;
        // Count houses and levels.
        for (int i = 0; i < buildings.Count; i++)
        {
            if (buildings[i] != "2"){continue;}
            limit += int.Parse(buildingLevels[i]);
        }
        return limit;
    }

    public int DeterminePopulation()
    {
        int pop = 0;
        for (int i = 0; i < workerFamilySize.Count; i++)
        {
            if (workerFamilySize[i].Length <= 0){continue;}
            pop += int.Parse(workerFamilySize[i]);
        }
        return pop;
    }

    public int DetermineWorkerPopulation()
    {
        return workers.Count;
    }

    public int ReturnBuildingOnTile(string location)
    {
        for (int i = 0; i < buildingLocations.Count; i++)
        {
            if (buildingLocations[i] == location){return int.Parse(buildings[i]);}
        }
        return -1;
    }

    public int ReturnBuildingIndexOnTile(int location)
    {
        string loc = location.ToString();
        for (int i = 0; i < buildingLocations.Count; i++)
        {
            if (buildingLocations[i] == loc){return i;}
        }
        return -1;
    }

    public int ReturnCurrentLocationCapacity(int location)
    {
        int c = 0;
        string loc = location.ToString();
        for (int i = 0; i < workerLocations.Count; i++)
        {
            if (workerLocations[i] == loc){c++;}
        }
        return c;
    }

    public int ReturnWorkersBuilding(int workerIndex)
    {
        return ReturnBuildingOnTile(workerLocations[workerIndex]);
    }

    public int ReturnWorkerSkillLevel(int workerIndex, int buildingType)
    {
        string[] allWorkerSkills = workerSkills[workerIndex].Split(",");
        string[] specificSkill = new string[2];
        for (int i = 0; i < allWorkerSkills.Length; i++)
        {
            if (allWorkerSkills[i].Length < 3){continue;}
            specificSkill = allWorkerSkills[i].Split("=");
            if (int.Parse(specificSkill[0]) == buildingType){return int.Parse(specificSkill[1])/100;}
        }
        return 0;
    }
    
    public List<int> ReturnOutputs()
    {
        // At first projected outputs are all 0.
        projectedOutputs.Clear();
        projectedOutputs.Add(0);
        projectedOutputs.Add(-(DeterminePopulation()));
        projectedOutputs.Add(0);
        // No projections without workers.
        if (workerLocations.Count <= 0){return projectedOutputs;}
        int buildingType = -1;
        int skillLevel = 0;
        List<string> outputTypes = new List<string>();
        // n = # workers <= m = # buildings
        // Get the outputs for each worker.
        // O(n)
        for (int i = 0; i < workerLocations.Count; i++)
        {
            // Find what building they're assigned to.
            // O(m)
            buildingType = ReturnBuildingOnTile(workerLocations[i]);
            // Check if the worker is skilled at this type of work and at what level.
            // O(p) // p = # worker skills
            skillLevel = ReturnWorkerSkillLevel(i, buildingType);
            // Find out what kind of produce they make.
            // O(q) // q = # buildingTypes
            outputTypes = buildingData.ReturnOutputList(buildingType);
            // O(1) // Constant about of output types.
            // Loop through the output types.
            for (int j = 0; j < outputTypes.Count; j++)
            {
                if (int.Parse(outputTypes[j]) < 0){continue;}
                if (outputTypes[j].Length <= 0){continue;}
                projectedOutputs[int.Parse(outputTypes[j])] += 1 + skillLevel;
            }
        }
        return projectedOutputs;
    }
}