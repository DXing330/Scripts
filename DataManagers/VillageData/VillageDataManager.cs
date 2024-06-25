using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageDataManager : BasicDataManager
{
    public BuildingDataManager buildingData;
    public int basePrice = 5;
    public string villageData;
    // village includes terrain info
        // terrain info needs dimensions and terrain types
    public int totalRows; // 9
    public int totalColumns; // 9
    public List<string> villageTiles; // figure it out
    // money|food|wood|stone|mana
    public List<string> resources; // 0|2|0|0|0
    public bool PayResource(int amount, int type = 0)
    {
        int currentAmount = int.Parse(resources[type]);
        if (amount > currentAmount){return false;}
        else
        {
            currentAmount -= amount;
            resources[type] = currentAmount.ToString();
            return true;
        }
    }
    public bool PayResources(List<int> costs, bool building = true)
    {
        List<string> updatedResources = new List<string>(resources);
        int specificAmount = 0;
        // Check all the costs.
        // O(1), constant amount of different resource costs.
        for (int i = 0; i < resources.Count; i++)
        {
            specificAmount = int.Parse(resources[i]);
            if (i == 0 && building)
            {
                // Building's cost more gold than normal.
                costs[i] *= basePrice;
            }
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
    public void UpdateResources(List<int> changes)
    {
        for (int i = 0; i < resources.Count; i++)
        {
            // Can't store more than 100x the center's level of any resource, not enough space?
            //resources[i] = (Mathf.Min(int.Parse(buildingLevels[0])*100, int.Parse(resources[i])+changes[i])).ToString();
            // No need for such limits.
            resources[i] = (int.Parse(resources[i])+changes[i]).ToString();
        }
    }
    protected void AdjustMorale()
    {
        // They need food and housing or else they'll quickly leave.
        if (int.Parse(resources[1]) < 0)
        {
            vassals.DecreaseVassalMorale();
        }
        // If there is a surplus of food they'll be content.
        else if (int.Parse(resources[1]) < DetermineFoodConsumption())
        {
            vassals.IncreaseVassalMorale(6);
        }
        if (DetermineHousingLimit() < vassals.vassals.Count)
        {
            vassals.DecreaseVassalMorale();
        }
        // Later add more ACKs stuff like festivals and garrisons.
        // If they're resting then increase their morale up to 6.
    }
    // village has buildings on top of terrain
    public int ReturnCenterLevel()
    {
        return buildings.ReturnCenterLevel();
    }
    public List<string> possibleBuildings; // House|Farm|Quarry|Lumberyard
    public List<string> buildingsOnTerrainTypes; // Farm,House|Lumberyard||||||||Quarry||||
    protected List<int> projectedOutputs = new List<int>();
    public List<BasicDataManager> dataManagers;
    public VassalDataManager vassals;
    public VassalHiringDataManager vassalHiring;
    public GeneralPopulationDataManager generalPopulation;
    public VillageBuildingDataManager buildings;
    public MarketDataManager market;

    public void NewDay(bool resources = true)
    {
        // Get outputs every week.
        if (resources)
        {
            UpdateResources(ReturnOutputs());
            AdjustMorale();
        }
        // Update any building phase stuff.
        UpdateBuildTimes();
        for (int i = 0; i < vassals.vassals.Count; i++)
        {
            IncreaseWorkerSkillLevel(i);
        }
        for (int i = 0; i < dataManagers.Count; i++)
        {
            dataManagers[i].NewDay();
        }
    }

    public void NewYear()
    {
        // People born.
        vassals.UpdateFamilySizes();
    }

    protected void UpdateBuildTimes()
    {
        for (int i = 0; i < vassals.locations.Count; i++)
        {
            Build(vassals.locations[i], i);
        }
    }

    [ContextMenu("New Game")]
    public override void NewGame()
    {
        saveDataPath = Application.persistentDataPath;
        if (File.Exists(saveDataPath+fileName))
        {
            File.Delete (saveDataPath+fileName);
        }
        villageData = newGameData;
        GameManager.instance.utility.DataManagerNewGame(dataManagers);
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
        data += GameManager.instance.ConvertListToString(possibleBuildings)+"#";
        data += GameManager.instance.ConvertListToString(buildingsOnTerrainTypes)+"#";
        File.WriteAllText(saveDataPath+fileName, data);
        GameManager.instance.utility.DataManagerSave(dataManagers);
    }

    protected void QuickSave()
    {
        saveDataPath = Application.persistentDataPath;
        string data = villageData;
        File.WriteAllText(saveDataPath+fileName, data);
    }

    public override void Load()
    {
        saveDataPath = Application.persistentDataPath;
        if (File.Exists(saveDataPath+fileName))
        {
            loadedData = File.ReadAllText(saveDataPath+fileName);
            villageData = loadedData;
        }
        else
        {
            villageData = newGameData;
        }
        SetData();
        GameManager.instance.utility.DataManagerLoad(dataManagers);
    }

    protected void SetData()
    {
        string[] dataBlocks = villageData.Split("#");
        totalRows = int.Parse(dataBlocks[0]);
        totalColumns = int.Parse(dataBlocks[1]);
        villageTiles = dataBlocks[2].Split("|").ToList();
        resources = dataBlocks[3].Split("|").ToList();
        possibleBuildings = dataBlocks[4].Split("|").ToList();
        buildingsOnTerrainTypes = dataBlocks[5].Split("|").ToList();
    }

    public int DetermineHousingLimit()
    {
        int limit = 0;
        // Count houses and levels.
        for (int i = 0; i < buildings.buildings.Count; i++)
        {
            if (buildings.buildings[i] != "2"){continue;}
            limit += int.Parse(buildings.buildingLevels[i]);
        }
        return limit;
    }

    public int DeterminePopulation()
    {
        int pop = 0;
        for (int i = 0; i < vassals.familySizes.Count; i++)
        {
            if (vassals.familySizes[i].Length <= 0){continue;}
            pop += int.Parse(vassals.familySizes[i]);
        }
        return pop;
    }

    public int DetermineFoodConsumption()
    {
        return vassals.DetermineFoodConsumption();
    }

    public int DetermineWorkerPopulation()
    {
        return vassals.vassals.Count;
    }

    public int ReturnBuildingOnTile(string location)
    {
        return buildings.ReturnBuildingOnTile(location);
    }

    public int ReturnBuildingIndexOnTile(int location)
    {
        return buildings.ReturnBuildingIndexOnTile(location);
    }

    public bool CheckIfNewBuilding(int location)
    {
        return buildings.CheckIfNewBuilding(location);
    }

    public int ReturnNewBuildingType(int location)
    {
        return buildings.ReturnNewBuildingType(location);
    }

    public int ReturnNewBuildingIndex(string location)
    {
        return buildings.ReturnNewBuildingIndex(location);
    }

    public string ReturnNewBuildingTime(int location)
    {
        return buildings.ReturnNewBuildingTime(location);
    }

    public int ReturnCurrentLocationCapacity(int location)
    {
        int c = 0;
        string loc = location.ToString();
        for (int i = 0; i < vassals.locations.Count; i++)
        {
            if (vassals.locations[i] == loc){c++;}
        }
        return c;
    }

    public int ReturnWorkersBuilding(int workerIndex)
    {
        return ReturnBuildingOnTile(vassals.locations[workerIndex]);
    }

    public bool WorkerBuilding(int workerIndex)
    {
        return CheckIfNewBuilding(int.Parse(vassals.locations[workerIndex]));
    }

    public int ReturnWorkerSkillLevel(int workerIndex, int buildingType)
    {
        string[] allWorkerSkills = vassals.skills[workerIndex].Split(",");
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

    protected void IncreaseWorkerSkillLevel(int workerIndex)
    {
        int buildingType = ReturnBuildingOnTile(vassals.locations[workerIndex]);
        // No such thing as skilled at resting at home.
        if (buildingType == 2){return;}
        List<string> allWorkerSkills = vassals.skills[workerIndex].Split(",").ToList();
        string[] specificSkill = new string[2];
        for (int i = 0; i < allWorkerSkills.Count; i++)
        {
            if (allWorkerSkills[i].Length < 3){continue;}
            specificSkill = allWorkerSkills[i].Split("=");
            if (int.Parse(specificSkill[0]) == buildingType)
            {
                allWorkerSkills[i] = specificSkill[0]+"="+(int.Parse(specificSkill[1])+1);
                vassals.skills[workerIndex] = GameManager.instance.ConvertListToString(allWorkerSkills, ",");
                return;
            }
        }
        vassals.skills[workerIndex] += ","+buildingType+"=0";
    }

    public void SortWorkerSkills(int workerIndex)
    {
        // Implement quick sort later I guess.
        List<string> allWorkerSkills = vassals.skills[workerIndex].Split(",").ToList();
        List<int> skillLevels = new List<int>();
        string[] specificSkill = new string[2];
        for (int i = 0; i < allWorkerSkills.Count; i++)
        {
            skillLevels.Add(int.Parse(allWorkerSkills[i].Split("=")[1]));
        }
        allWorkerSkills = GameManager.instance.utility.QuickSortListbyIntList(allWorkerSkills, skillLevels, 0, allWorkerSkills.Count - 1);
        vassals.skills[workerIndex] = GameManager.instance.ConvertListToString(allWorkerSkills, ",");
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
        // Gain taxes from you citizens.
        projectedOutputs[0] += generalPopulation.ReturnTaxIncome();
        // No projections without workers.
        if (vassals.vassals.Count <= 0){return projectedOutputs;}
        int buildingType = -1;
        int skillLevel = 0;
        List<string> outputTypes = new List<string>();
        // n = # workers <= m = # buildings
        // Get the outputs for each worker.
        // O(n)
        for (int i = 0; i < vassals.vassals.Count; i++)
        {
            if (vassals.vassals[i].Length <= 0){continue;}
            // If they're building then no outputs.
            // O(n) // n = length of new buildings.
            if (CheckIfNewBuilding(int.Parse(vassals.locations[i]))){continue;}
            // Find what building they're assigned to.
            // O(m)
            buildingType = ReturnBuildingOnTile(vassals.locations[i]);
            // Check if the worker is skilled at this type of work and at what level.
            // O(p) // p = # worker skills
            skillLevel = ReturnWorkerSkillLevel(i, buildingType);
            // Skill level will also start to generate gold.
            projectedOutputs[0] += (skillLevel/2);
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
                projectedOutputs[int.Parse(outputSpecifics[0])] += int.Parse(outputSpecifics[1]) + (skillLevel/2);
            }
        }
        return projectedOutputs;
    }

    public void StartBuilding(int tileNumber, int buildingType, int duration)
    {
        buildings.StartBuilding(tileNumber, buildingType, duration);
    }

    protected void Build(string location, int workerIndex)
    {
        buildings.Build(location, workerIndex);
    }

    public void ChangeTerrain(int tileNumber, int newTerrain)
    {
        villageTiles[tileNumber] = newTerrain.ToString();   
    }
}