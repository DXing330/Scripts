using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageEditor : Map
{
    protected override void Start()
    {
        villageData = GameManager.instance.villageData;
        LoadVillageData();
    }
    public VillageDataManager villageData;
    public VillageGUI GUI;
    public BuildingDataManager buildingData;
    public BasicSpriteManager buildingSprites;
    public List<int> allBuildings;
    protected void UpdateAllBuildings(List<string> buildings, List<string> locations)
    {
        allBuildings.Clear();
        for (int i = 0; i < totalRows * totalColumns; i++)
        {
            allBuildings.Add(-1);
        }
        if (locations.Count <= 0){return;}
        for (int i = 0; i < locations.Count; i++)
        {
            if (locations[i].Length <= 0){continue;}
            allBuildings[int.Parse(locations[i])] = int.Parse(buildings[i]);
        }
    }

    protected override void UpdateTile(int imageIndex, int tileIndex)
    {
        base.UpdateTile(imageIndex, tileIndex);
        if (tileIndex < 0 || tileIndex >= (totalRows * totalColumns))
        {
            return;
        }
        if (allBuildings[tileIndex] >= 0)
        {
            terrainTiles[imageIndex].UpdateLocationImage(buildingSprites.allSprites[allBuildings[tileIndex]]);
        }
    }

    protected void LoadVillageData()
    {
        totalRows = villageData.totalRows;
        totalColumns = villageData.totalColumns;
        allTiles = villageData.villageTiles;
        UpdateAllBuildings(villageData.buildings, villageData.buildingLocations);
        UpdateCenterTile();
        UpdateMap();
    }

    public int state = -1;
    public void SetState(int newState)
    {
        state = newState;
        HighlightBasedOnState();
    }
    protected void HighlightBasedOnState()
    {
        switch (state)
        {
            case -1:
                for (int i = 0; i < terrainTiles.Count; i++)
                {
                    terrainTiles[i].ResetHighlight();
                }
                break;
            case 0:
                HighlightSelectedWorker();
                break;
            case 1:
                HighlightSelectedBuilding();
                break;
        }
    }
    protected void UpdateMap()
    {
        for (int i = 0; i < terrainTiles.Count; i++)
        {
            terrainTiles[i].ResetAllImages();
            UpdateTile(i, currentTiles[i]);
        }
        HighlightBasedOnState();
    }

    // need to be able to highlight when selecting things
    int selectedWorker = -1;
    public void HighlightSelectedWorker(int newlySelectedWorker = -1)
    {
        if (newlySelectedWorker >= 0)
        {
            selectedWorker = newlySelectedWorker;
        }
        if (selectedWorker < 0){return;}
        for (int i = 0; i < terrainTiles.Count; i++)
        {
            terrainTiles[i].ResetHighlight();
        }
        int location = int.Parse(villageData.workerLocations[selectedWorker]);
        if (location < 0){return;}
        int indexOf = currentTiles.IndexOf(location);
        if (indexOf >= 0)
        {
            terrainTiles[indexOf].Highlight();
        }
    }
    int selectedBuilding = -1;
    public void HighlightSelectedBuilding(int newlySelectedBuilding = -1)
    {
        if (newlySelectedBuilding >= 0)
        {
            selectedBuilding = newlySelectedBuilding;
        }
        if (selectedBuilding < 0){return;}
        for (int i = 0; i < terrainTiles.Count; i++)
        {
            terrainTiles[i].ResetHighlight();
        }
        int location = int.Parse(villageData.buildingLocations[selectedBuilding]);
        int indexOf = currentTiles.IndexOf(location);
        if (indexOf >= 0)
        {
            terrainTiles[indexOf].Highlight();
        }
    }

    public override void MoveMap(int direction)
    {
        base.MoveMap(direction);
        UpdateMap();
    }

    public override void ClickOnTile(int index)
    {
        int tileNumber = currentTiles[index];
        if (tileNumber < 0){return;}
        switch (state)
        {
            case 0:
                AssignSelectedWorker(tileNumber);
                break;
            case 1:
                SelectBuilding(tileNumber);
                break;
        }
        GUI.UpdatePanels();
    }

    protected void AssignSelectedWorker(int tileNumber)
    {
        if (selectedWorker < 0){return;}
        // Check capacity of building.
            // First get the building index on the tile.
        int buildingIndex = villageData.ReturnBuildingIndexOnTile(tileNumber);
        if (buildingIndex < 0){return;}
            // Then compare the current capacity to max capacity.
        if (villageData.ReturnCurrentLocationCapacity(tileNumber) >= buildingData.ReturnWorkerLimit(int.Parse(villageData.buildings[buildingIndex]), int.Parse(villageData.buildingLevels[buildingIndex]))){return;}
        villageData.workerLocations[selectedWorker] = tileNumber.ToString();
        villageData.Save();
    }

    protected void SelectBuilding(int tileNumber)
    {
        int buildingIndex = villageData.ReturnBuildingIndexOnTile(tileNumber);
        if (buildingIndex < 0){return;}
        GUI.ChangeBuildingIndex(buildingIndex);
    }
}
