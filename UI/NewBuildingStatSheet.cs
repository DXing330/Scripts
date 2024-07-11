using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBuildingStatSheet : MonoBehaviour
{
    public VillageDataManager villageData;
    public int selectedTerrainType = -1;
    public string selectedBuildingName;
    public int selectedBuildingType = -1;
    public int ReturnSelectedBuilding(){return selectedBuildingType;}
    public BasicSpriteManager buildingSprites;
    public BuildingStatSheet statSheet;
    public List<string> possibleBuildingTypes;
    public int currentPage = 0;
    public void ChangePage(bool right = true)
    {
        currentPage = GameManager.instance.utility.ChangePage(currentPage, right, possibleObjects, possibleBuildingTypes);
        statSheet.ResetBasicStats();
        UpdatePossibleSprites();
    }
    public List<GameObject> possibleObjects;
    public List<FormationTile> possibleBuildings;

    protected void UpdatePossibleSprites()
    {
        int start = possibleObjects.Count * currentPage;
        int end = Mathf.Min(start+possibleObjects.Count, possibleBuildingTypes.Count - start);
        for (int i = 0; i < possibleObjects.Count; i++)
        {
            possibleObjects[i].SetActive(false);
        }
        if (start >= end)
        {
            statSheet.ResetBasicStats();
            return;
        }
        UpdateHighlights();
        for (int i = start; i < end; i++)
        {
            possibleObjects[i-start].SetActive(true);
            possibleBuildings[i-start].UpdateActorSprite(buildingSprites.SpriteDictionary(possibleBuildingTypes[i]));
        }
    }

    protected void UpdateHighlights(int selected = -1)
    {
        for (int i = 0; i < possibleBuildings.Count; i++)
        {
            possibleBuildings[i].ResetHighlight();
        }
        if (selected >= 0){possibleBuildings[selected].Highlight();}
    }

    public void SetTerrainType(int newType)
    {
        selectedTerrainType = newType;
        currentPage = 0;
        if (newType < 0)
        {
            possibleBuildingTypes.Clear();
            UpdatePossibleSprites();
            return;
        }
        // Get the buildable buildings for this terrain type.
        possibleBuildingTypes = villageData.buildingsOnTerrainTypes[selectedTerrainType].Split(",").ToList();
        statSheet.ResetBasicStats();
        UpdatePossibleSprites();
    }

    public void SetBuildingType(int newType)
    {
        selectedBuildingName = possibleBuildingTypes[newType + (currentPage * possibleObjects.Count)];
        UpdateHighlights(newType);
        selectedBuildingType = statSheet.buildingData.ReturnBuildingIndex(selectedBuildingName);
        UpdateStats();
    }

    protected void UpdateStats()
    {
        if (selectedBuildingType < 0){return;}
        statSheet.villageData = villageData;
        statSheet.UpdateBasicStats(selectedBuildingType);
    }

    public void CheckOnUnBuilt(int tileNumber)
    {
        UpdateUnbuiltStats(tileNumber);
    }

    protected void UpdateUnbuiltStats(int tileNumber)
    {
        // Get the building type.
        int buildingType = villageData.ReturnNewBuildingType(tileNumber);
        if (buildingType < 0){return;}
        statSheet.UpdateUnbuiltStats(buildingType);
        statSheet.upgradeTime.text = villageData.ReturnNewBuildingTime(tileNumber);
    }
}
