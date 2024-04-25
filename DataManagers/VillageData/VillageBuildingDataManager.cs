using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageBuildingDataManager : BasicDataManager
{
    public VillageDataManager villageData;
    public string fileName = "/buildings.txt";
    public string newGameData;
    [ContextMenu("New Game")]
    public override void NewGame()
    {
        saveDataPath = Application.persistentDataPath;
        if (File.Exists(saveDataPath+fileName))
        {
            File.Delete (saveDataPath+fileName);
        }
        File.WriteAllText(saveDataPath+fileName, newGameData);
        Load();
    }
    public override void Save()
    {
        saveDataPath = Application.persistentDataPath;
        string data = "";
        data += GameManager.instance.ConvertListToString(buildings)+"#";
        data += GameManager.instance.ConvertListToString(buildingLocations)+"#";
        data += GameManager.instance.ConvertListToString(buildingLevels)+"#";
        data += GameManager.instance.ConvertListToString(buildingHealths)+"#";
        data += GameManager.instance.ConvertListToString(buildingPhaseBuildings)+"#";
        data += GameManager.instance.ConvertListToString(buildingPhaseLocations)+"#";
        data += GameManager.instance.ConvertListToString(buildTimes)+"#";
        File.WriteAllText(saveDataPath+fileName, data);
    }
    public override void Load()
    {
        saveDataPath = Application.persistentDataPath;
        if (File.Exists(saveDataPath+fileName))
        {
            loadedData = File.ReadAllText(saveDataPath+fileName);
            string[] blocks = loadedData.Split("#");
            buildings = blocks[0].Split("|").ToList();
            buildingLocations = blocks[1].Split("|").ToList();
            buildingLevels = blocks[2].Split("|").ToList();
            buildingHealths = blocks[3].Split("|").ToList();
            buildingPhaseBuildings = blocks[4].Split("|").ToList();
            buildingPhaseLocations = blocks[5].Split("|").ToList();
            buildTimes = blocks[6].Split("|").ToList();
            GameManager.instance.RemoveEmptyListItems(buildingPhaseBuildings,0);
            GameManager.instance.RemoveEmptyListItems(buildingPhaseLocations,0);
            GameManager.instance.RemoveEmptyListItems(buildTimes,0);
        }
        else
        {
            NewGame();
        }
    }
    public List<string> buildings;
    public string ReturnBuildingNameFromIndex(int index)
    {
        return villageData.buildingData.ReturnBuildingName(int.Parse(buildings[index]));
    }
    public List<string> buildingLocations;
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
    public List<string> buildingLevels;
    public int ReturnCenterLevel()
    {
        return int.Parse(buildingLevels[0]);
    }
    public List<string> buildingHealths;
    public void StartBuilding(int tileNumber, int buildingType, int duration)
    {
        buildingPhaseLocations.Add(tileNumber.ToString());
        buildingPhaseBuildings.Add(buildingType.ToString());
        buildTimes.Add(duration.ToString());
    }
    public void Build(string location, int workerIndex)
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
    public List<string> buildingPhaseBuildings;
    public List<string> buildingPhaseLocations;
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
    public List<string> buildTimes;
    public string ReturnNewBuildingTime(int location)
    {
        string loc = location.ToString();
        for (int i = 0; i < buildingPhaseLocations.Count; i++)
        {
            if (buildingPhaseLocations[i] == loc){return (buildTimes[i]);}
        }
        return "";
    }
    public void FinishBuilding(int index)
    {
        int indexOf = buildingLocations.IndexOf(buildingPhaseLocations[index]);
        if (indexOf < 0)
        {
            string newType = buildingPhaseBuildings[index];
            buildings.Add(newType);
            buildingLocations.Add(buildingPhaseLocations[index]);
            buildingLevels.Add("1");
            buildingHealths.Add((villageData.buildingData.ReturnBuildingMaxHealth(int.Parse(newType))).ToString());
        }
        else
        {
            buildingLevels[indexOf] = (int.Parse(buildingLevels[indexOf])+1).ToString();
        }
        buildingPhaseBuildings.RemoveAt(index);
        buildingPhaseLocations.RemoveAt(index);
        buildTimes.RemoveAt(index);
    }
}
