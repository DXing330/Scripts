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
    public List<int> newBuildings;
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
    protected void UpdateNewBuildings(List<string> buildings, List<string> locations)
    {
        newBuildings.Clear();
        for (int i = 0; i < totalRows * totalColumns; i++)
        {
            newBuildings.Add(-1);
        }
        if (locations.Count <= 0){return;}
        for (int i = 0; i < locations.Count; i++)
        {
            if (locations[i].Length <= 0){continue;}
            newBuildings[int.Parse(locations[i])] = int.Parse(buildings[i]);
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
            // Probably want to change this to refer to name later so that we can use the actor sprites for everything.
            terrainTiles[imageIndex].UpdateLocationImage(buildingSprites.allSprites[allBuildings[tileIndex]]);
        }
        if (newBuildings[tileIndex] >= 0)
        {
            terrainTiles[imageIndex].UpdateLocationImage(buildingSprites.allSprites[newBuildings[tileIndex]]);
            terrainTiles[imageIndex].BlackenLocationImage();
        }
    }
    protected void LoadVillageData()
    {
        totalRows = villageData.totalRows;
        totalColumns = villageData.totalColumns;
        allTiles = villageData.villageTiles;
        UpdateAllBuildings(villageData.buildings.buildings, villageData.buildings.buildingLocations);
        UpdateNewBuildings(villageData.buildings.buildingPhaseBuildings, villageData.buildings.buildingPhaseLocations);
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
            case 2:
                HighlightSelectedTile();
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
    public int selectedWorker = -1;
    public void HighlightSelectedWorker(int newlySelectedWorker = -1)
    {
        if (villageData.vassals.vassals.Count <= 0){return;}
        if (newlySelectedWorker >= 0)
        {
            selectedWorker = newlySelectedWorker;
        }
        if (selectedWorker < 0){return;}
        for (int i = 0; i < terrainTiles.Count; i++)
        {
            terrainTiles[i].ResetHighlight();
        }
        int location = int.Parse(villageData.vassals.locations[selectedWorker]);
        if (location < 0){return;}
        int indexOf = currentTiles.IndexOf(location);
        if (indexOf >= 0)
        {
            terrainTiles[indexOf].Highlight();
        }
    }
    public int selectedBuilding = -1;
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
        int location = int.Parse(villageData.buildings.buildingLocations[selectedBuilding]);
        int indexOf = currentTiles.IndexOf(location);
        if (indexOf >= 0)
        {
            terrainTiles[indexOf].Highlight();
        }
    }
    public int selectedTile = -1;
    public int ReturnSelectedTile(){return selectedTile;}
    public void HighlightSelectedTile(int newlySelectedTile = -1)
    {
        if (newlySelectedTile >= 0)
        {
            selectedTile = newlySelectedTile;
        }
        if (selectedTile < 0){return;}
        for (int i = 0; i < terrainTiles.Count; i++)
        {
            terrainTiles[i].ResetHighlight();
        }
        int indexOf = currentTiles.IndexOf(selectedTile);
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
            case -1:
                // Enter the building if possible.
                break;
            case 0:
                AssignSelectedWorker(tileNumber);
                break;
            case 1:
                SelectBuilding(tileNumber);
                break;
            case 2:
                SelectTerrain(tileNumber);
                break;
            case 4:
                SelectTerrainToChange(tileNumber);
                break;
        }
        GUI.UpdatePanels();
    }

    protected void AssignSelectedWorker(int tileNumber)
    {
        if (selectedWorker < 0){return;}
        // Check if your assigning them to a building project.
        if (villageData.CheckIfNewBuilding(tileNumber))
        {
            villageData.vassals.locations[selectedWorker] = tileNumber.ToString();
            villageData.Save();
            return;
        }
        // Check capacity of building.
            // First get the building index on the tile.
        int buildingIndex = villageData.ReturnBuildingIndexOnTile(tileNumber);
        if (buildingIndex < 0){return;}
            // Then compare the current capacity to max capacity.
        if (villageData.ReturnCurrentLocationCapacity(tileNumber) >= buildingData.ReturnWorkerLimit(int.Parse(villageData.buildings.buildings[buildingIndex]), int.Parse(villageData.buildings.buildingLevels[buildingIndex]))){return;}
        villageData.vassals.locations[selectedWorker] = tileNumber.ToString();
        villageData.Save();
    }

    protected void SelectBuilding(int tileNumber)
    {
        int buildingIndex = villageData.ReturnBuildingIndexOnTile(tileNumber);
        if (buildingIndex < 0){return;}
        GUI.ChangeBuildingIndex(buildingIndex);
    }

    protected void SelectTerrain(int tileNumber)
    {
        // Check to make sure there is no building there.
        if (villageData.buildings.buildingLocations.Contains(tileNumber.ToString())){return;}
        if (!villageData.buildings.buildingPhaseLocations.Contains(tileNumber.ToString()))
        {
            HighlightSelectedTile(tileNumber);
            // Pass the terrain type to the new building thing.
            GUI.ChangeTerrainType(int.Parse(allTiles[tileNumber]));
        }
        else
        {
            HighlightSelectedTile(tileNumber);
            GUI.CheckUnbuiltBuilding(tileNumber);
        }
    }

    protected void SelectTerrainToChange(int tileNumber)
    {
        // Can only change the terrain of empty tiles.
        if (villageData.buildings.buildingLocations.Contains(tileNumber.ToString())){return;}
        if (villageData.buildings.buildingPhaseLocations.Contains(tileNumber.ToString())){return;}
        HighlightSelectedTile(tileNumber);
        GUI.ChangeTerrainType(int.Parse(allTiles[tileNumber]));
    }

    public void TryToBuildNew(int buildingType)
    {
        List<int> allCosts = buildingData.ReturnBuildCostInOrder(buildingType);
        if (villageData.PayResources(allCosts))
        {
            int buildTime = buildingData.ReturnBuildTime(buildingType);
            villageData.StartBuilding(selectedTile, buildingType, buildTime);
            UpdateNewBuildings(villageData.buildings.buildingPhaseBuildings, villageData.buildings.buildingPhaseLocations);
            UpdateMap();
        }
    }

    public void TryToUpgrade(int buildingType, int buildingLevel = 1)
    {
        // Can't upgrade what is already being upgraded.
        string loc = villageData.buildings.buildingLocations[selectedBuilding];
        if (villageData.buildings.buildingPhaseLocations.Contains(loc)){return;}
        List<int> allCosts = buildingData.ReturnBuildCostInOrder(buildingType, buildingLevel);
        if (villageData.PayResources(allCosts))
        {
            // Try to upgrade that tile.
            int buildTime = buildingData.ReturnBuildTime(buildingType, buildingLevel);
            villageData.StartBuilding(int.Parse(villageData.buildings.buildingLocations[selectedBuilding]), buildingType, buildTime);
            UpdateNewBuildings(villageData.buildings.buildingPhaseBuildings, villageData.buildings.buildingPhaseLocations);
            UpdateMap();
        }
    }

    public bool TryChangeTerrain(int newTerrainType)
    {
        // Check the mana cost.
        if (villageData.PayResource(int.Parse(GUI.changeTerrainSheet.cost.text), 4))
        {
            villageData.ChangeTerrain(selectedTile, newTerrainType);
            UpdateMap();
            return true;
        }
        return false;
    }
}
