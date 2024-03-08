using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDataManager : MonoBehaviour
{
    protected int resourceTypes = 6;
    public string configData;
    public List<string> names;
    public List<string> indexes;
    public List<string> baseHealths;
    public List<string> hpPerLevel;
    public List<string> baseWorkers;
    public List<string> workersPerLevel;
    public List<string> upgradeCosts;
    public List<string> buildableTerrains;
    public List<string> outputs;
    public List<string> flavorTexts;
    public List<string> buildTimes;
    protected List<string> dummyList;

    [ContextMenu("Load")]
    public void LoadAllData()
    {
        string[] configBlocks = configData.Split("#");
        names = configBlocks[0].Split("|").ToList();
        indexes = configBlocks[1].Split("|").ToList();
        baseHealths = configBlocks[2].Split("|").ToList();
        hpPerLevel = configBlocks[3].Split("|").ToList();
        baseWorkers = configBlocks[4].Split("|").ToList();
        workersPerLevel = configBlocks[5].Split("|").ToList();
        upgradeCosts = configBlocks[6].Split("|").ToList();
        buildableTerrains = configBlocks[7].Split("|").ToList();
        outputs = configBlocks[8].Split("|").ToList();
        flavorTexts = configBlocks[9].Split("|").ToList();
        buildTimes = configBlocks[10].Split("|").ToList();
    }

    public int ReturnBuildingMaxHealth(int buildingIndex, int level = 1)
    {
        if (buildingIndex < 0 || buildingIndex >= names.Count){return 0;}
        return (int.Parse(baseHealths[buildingIndex])+(level-1)*int.Parse(hpPerLevel[buildingIndex]));
    }

    public int ReturnHealthPerLevel(int buildingIndex)
    {
        if (buildingIndex < 0 || buildingIndex >= names.Count){return 0;}
        return (int.Parse(hpPerLevel[buildingIndex]));
    }

    public int ReturnWorkerPerLevel(int buildingIndex)
    {
        if (buildingIndex < 0 || buildingIndex >= names.Count){return 0;}
        return (int.Parse(workersPerLevel[buildingIndex]));
    }

    public string ReturnBuildingName(int buildingIndex)
    {
        if (buildingIndex < 0 || buildingIndex >= names.Count){return "";}
        return names[buildingIndex];
    }

    public int ReturnBuildingIndex(string buildingName)
    {
        return names.IndexOf(buildingName);
    }

    public string ReturnBuildingTask(int buildingIndex)
    {
        if (buildingIndex < 0 || buildingIndex >= names.Count){return "";}
        // Assigned to home means not working.
        if (names[buildingIndex] == "House"){return "Rest";}
        return names[buildingIndex];
    }

    public int ReturnWorkerLimit(int buildingIndex, int level = 1)
    {
        if (buildingIndex < 0 || buildingIndex >= names.Count){return 0;}
        return (int.Parse(baseWorkers[buildingIndex])+(level-1)*int.Parse(workersPerLevel[buildingIndex]));
    }

    public List<string> ReturnBuildableTerrainList(int buildingIndex)
    {
        dummyList.Clear();
        if (buildingIndex < 0 || buildingIndex >= names.Count){return dummyList;}
        dummyList = buildableTerrains[buildingIndex].Split(",").ToList();
        return dummyList;
    }

    public List<string> ReturnOutputList(int buildingIndex)
    {
        dummyList.Clear();
        if (buildingIndex < 0 || buildingIndex >= names.Count){return dummyList;}
        dummyList = outputs[buildingIndex].Split(",").ToList();
        return dummyList;
    }

    public string ReturnFlavorText(int buildingIndex)
    {
        if (buildingIndex < 0 || buildingIndex >= names.Count){return "";}
        return flavorTexts[buildingIndex];
    }

    public int ReturnBuildTime(int buildingIndex, int level = 0)
    {
        if (buildingIndex < 0 || buildingIndex >= names.Count){return -1;}
        int time = int.Parse(buildTimes[buildingIndex]);
        return time*(level+1);
    }

    public List<string> ReturnBuildCost(int buildingIndex)
    {
        dummyList.Clear();
        if (buildingIndex < 0 || buildingIndex >= names.Count){return dummyList;}
        dummyList = upgradeCosts[buildingIndex].Split(",").ToList();
        return dummyList;
    }

    public List<int> ReturnBuildCostInOrder(int buildingIndex)
    {
        dummyList.Clear();
        List<int> costs = new List<int>();
        if (buildingIndex < 0 || buildingIndex >= names.Count){return costs;}
        for (int i = 0; i < resourceTypes; i++)
        {
            costs.Add(0);
        }
        dummyList = upgradeCosts[buildingIndex].Split(",").ToList();
        for (int i = 0; i < dummyList.Count; i++)
        {
            if (dummyList[i].Length < 3){continue;}
            string[] specifics = dummyList[i].Split("=");
            costs[int.Parse(specifics[0])] = int.Parse(specifics[1]);
        }
        return costs;
    }
}
