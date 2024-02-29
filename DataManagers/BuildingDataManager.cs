using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDataManager : MonoBehaviour
{
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
    }

    public int ReturnBuildingMaxHealth(int buildingIndex, int level)
    {
        if (buildingIndex < 0 || buildingIndex >= names.Count){return 0;}
        return (int.Parse(baseHealths[buildingIndex])+(level-1)*int.Parse(hpPerLevel[buildingIndex]));
    }

    public string ReturnBuildingName(int buildingIndex)
    {
        if (buildingIndex < 0 || buildingIndex >= names.Count){return "";}
        // Assigned to home means not working.
        if (names[buildingIndex] == "Residential"){return "Rest";}
        // Assigned to center means ???
        if (names[buildingIndex] == "Center"){return "";}
        return names[buildingIndex];
    }

    public int ReturnWorkerLimit(int buildingIndex, int level)
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
}
