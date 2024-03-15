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
    public List<string> villageTiles; // figure it out
    // Keep track of resources I guess.
        // money|food|wood|stone|woman
        // women can be married to increase loyalty and make it harder to for workers to leave.
    public List<string> resources; // 0|2|0|0|0
    public void UpdateResources(List<int> changes)
    {
        for (int i = 0; i < resources.Count; i++)
        {
            // Can't store more than 100x the center's level of any resource, not enough space?
            resources[i] = (Mathf.Min(int.Parse(buildingLevels[0])*100, int.Parse(resources[i])+changes[i])).ToString();
        }
    }
    // Have some workers than you can manage/automate.
        // Maybe give them names and other attributes?
    public List<string> workers; // alex|bob
    // skill format is skill=level,skill2=level2,etc.
    protected string GenerateNewWorker()
    {
        string worker = "";
        // Decide on name.
        // Decide on skills.
        // You get to pick from X workers which ones you want to keep?
        return worker;
    }
    public List<string> workerFamilySize; // 1|1 indicates size of family ie cost to feed
    public List<string> workerHealth; // 20|20 max is 20, 0 = death, deal with family somehow
    public List<string> workerSkills; // 1=100|1=100 indicates farming skill level 1 // skill levels = sqrt(value/100), very slow to increase skills later
    public List<string> workerLocations; // 39|31 indicates unassigned workers
    // village has buildings on top of terrain
    public List<string> buildings; // 0|1|2|2|5|4 = city center | farm | housing|house|quarry|lumberyard
    // Need to shift the buildings around whenever the village expands.
    public List<string> buildingLocations; // 40|30|39|31|38|37
    public List<string> buildingLevels; // 1|1|1|1|1|1
    public List<string> buildingHealths; // 200|40|40|40|40|40
    public List<string> buildingPhaseBuildings; // empty
    public List<string> buildingPhaseLocations; // empty
    public List<string> buildTimes; // empty
    public List<string> possibleBuildings; // House|Farm|Quarry|Lumberyard
    public List<string> buildingsOnTerrainTypes; // Farm,House|Lumberyard||||||||Quarry||||
    protected List<int> projectedOutputs = new List<int>();

    public void NewDay(bool resources = true)
    {
        // Get outputs every other day.
        if (resources){UpdateResources(ReturnOutputs());}
        // Update any building phase stuff.
        UpdateBuildTimes();
        // Decrease worker's health if they aren't resting.
    }

    protected void UpdateBuildTimes()
    {
        for (int i = 0; i < workerLocations.Count; i++)
        {
            Build(workerLocations[i], i);
        }
    }

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
        data += GameManager.instance.ConvertListToString(possibleBuildings)+"#";
        data += GameManager.instance.ConvertListToString(buildingsOnTerrainTypes)+"#";
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
        //buildingHealths = dataBlocks[12].Split("|").ToList();
        buildingPhaseBuildings = dataBlocks[13].Split("|").ToList();
        buildingPhaseLocations = dataBlocks[14].Split("|").ToList();
        buildTimes = dataBlocks[15].Split("|").ToList();
        possibleBuildings = dataBlocks[16].Split("|").ToList();
        buildingsOnTerrainTypes = dataBlocks[17].Split("|").ToList();
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

    public int DetermineFoodConsumption()
    {
        int consumption = 0;
        int familycons = 0;
        for (int i = 0; i < workerFamilySize.Count; i++)
        {
            if (workerFamilySize[i].Length <= 0){continue;}
            familycons = int.Parse(workerFamilySize[i]);
            if (familycons >= 2){familycons = 1 + (familycons/2);}
            consumption += familycons;
        }
        return consumption;
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

    public bool CheckIfNewBuilding(int location)
    {
        string loc = location.ToString();
        if (buildingPhaseLocations.Contains(loc)){return true;}
        return false;
    }

    public int ReturnNewBuildingType(int location)
    {
        string loc = location.ToString();
        for (int i = 0; i < buildingPhaseLocations.Count; i++)
        {
            if (buildingPhaseLocations[i] == loc){return int.Parse(buildingPhaseBuildings[i]);}
        }
        return -1;
    }

    public int ReturnNewBuildingIndex(string location)
    {
        for (int i = 0; i < buildingPhaseLocations.Count; i++)
        {
            if (buildingPhaseLocations[i] == location){return i;}
        }
        return -1;
    }

    public string ReturnNewBuildingTime(int location)
    {
        string loc = location.ToString();
        for (int i = 0; i < buildingPhaseLocations.Count; i++)
        {
            if (buildingPhaseLocations[i] == loc){return (buildTimes[i]);}
        }
        return "";
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

    public bool WorkerBuilding(int workerIndex)
    {
        return CheckIfNewBuilding(int.Parse(workerLocations[workerIndex]));
    }

    public int ReturnWorkerSkillLevel(int workerIndex, int buildingType)
    {
        IncreaseWorkerSkillLevel(workerIndex, buildingType);
        string[] allWorkerSkills = workerSkills[workerIndex].Split(",");
        string[] specificSkill = new string[2];
        for (int i = 0; i < allWorkerSkills.Length; i++)
        {
            if (allWorkerSkills[i].Length < 3){continue;}
            specificSkill = allWorkerSkills[i].Split("=");
            if (int.Parse(specificSkill[0]) == buildingType)
            {
                return (int)Mathf.Floor(Mathf.Sqrt(int.Parse(specificSkill[1])/100));
            }
        }
        return 0;
    }

    protected void IncreaseWorkerSkillLevel(int workerIndex, int buildingType)
    {
        List<string> allWorkerSkills = workerSkills[workerIndex].Split(",").ToList();
        string[] specificSkill = new string[2];
        for (int i = 0; i < allWorkerSkills.Count; i++)
        {
            if (allWorkerSkills[i].Length < 3){continue;}
            specificSkill = allWorkerSkills[i].Split("=");
            if (int.Parse(specificSkill[0]) == buildingType)
            {
                allWorkerSkills[i] = specificSkill[0]+"="+(int.Parse(specificSkill[1])+1);
                workerSkills[workerIndex] = GameManager.instance.ConvertListToString(allWorkerSkills, ",");
                return;
            }
        }
        workerSkills[workerIndex] += ","+buildingType+"=0";
    }

    public void SortWorkerSkills(int workerIndex)
    {
        // Implement quick sort later I guess.
        List<string> allWorkerSkills = workerSkills[workerIndex].Split(",").ToList();
        List<int> skillLevels = new List<int>();
        string[] specificSkill = new string[2];
        for (int i = 0; i < allWorkerSkills.Count; i++)
        {
            skillLevels.Add(int.Parse(allWorkerSkills[i].Split("=")[1]));
        }
        allWorkerSkills = GameManager.instance.utility.QuickSortListbyIntList(allWorkerSkills, skillLevels, 0, allWorkerSkills.Count - 1);
        workerSkills[workerIndex] = GameManager.instance.ConvertListToString(allWorkerSkills, ",");
    }
    
    public List<int> ReturnOutputs()
    {
        // At first projected outputs are all 0.
        projectedOutputs.Clear();
        projectedOutputs.Add(0);
        projectedOutputs.Add(-(DetermineFoodConsumption()));
        projectedOutputs.Add(0);
        projectedOutputs.Add(0);
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
            // If they're building then no outputs.
            // O(n) // n = length of new buildings.
            if (CheckIfNewBuilding(int.Parse(workerLocations[i]))){continue;}
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
                if (outputTypes[j].Length <= 2){continue;}
                // Output format is type=amount.
                string[] outputSpecifics = outputTypes[j].Split("=");
                projectedOutputs[int.Parse(outputSpecifics[0])] += int.Parse(outputSpecifics[1]) + skillLevel;
            }
        }
        return projectedOutputs;
    }

    public void StartBuilding(int tileNumber, int buildingType, int duration)
    {
        buildingPhaseLocations.Add(tileNumber.ToString());
        buildingPhaseBuildings.Add(buildingType.ToString());
        buildTimes.Add(duration.ToString());
    }

    protected void Build(string location, int workerIndex)
    {
        int index = ReturnNewBuildingIndex(location);
        if (index < 0){return;}
        int newTime = (int.Parse(buildTimes[index]) - 1);
        if (newTime <= 0)
        {
            FinishBuilding(index);
            return;
        }
        buildTimes[index] = newTime.ToString();
    }

    // Make an unbuilt building a real building or increase a building's level.
    protected void FinishBuilding(int index)
    {
        int indexOf = buildingLocations.IndexOf(buildingPhaseLocations[index]);
        if (indexOf < 0)
        {
            string newType = buildingPhaseBuildings[index];
            buildings.Add(newType);
            buildingLocations.Add(buildingPhaseLocations[index]);
            buildingLevels.Add("1");
            //buildingHealths.Add((buildingData.ReturnBuildingMaxHealth(int.Parse(newType))).ToString());
        }
        else
        {
            buildingLevels[indexOf] = (int.Parse(buildingLevels[indexOf])+1).ToString();
        }
        buildingPhaseBuildings.RemoveAt(index);
        buildingPhaseLocations.RemoveAt(index);
        buildTimes.RemoveAt(index);
    }

    public bool TryToConsumeResources(List<int> costs)
    {
        List<string> updatedResources = new List<string>(resources);
        int specificAmount = 0;
        // Check all the costs.
        // O(1), constant amount of different resource costs.
        for (int i = 0; i < resources.Count; i++)
        {
            specificAmount = int.Parse(resources[i]);
            if (specificAmount < costs[i])
            {
                // Fail a specific requirement.
                return false;
            }
            updatedResources[i] = (specificAmount - costs[i]).ToString();
        }
        // Meet all the requirements.
        resources = updatedResources;
        return true;
    }
}