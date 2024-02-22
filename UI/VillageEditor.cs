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
        UpdateMap();
    }

    protected void UpdateMap()
    {
        UpdateCenterTile();
        for (int i = 0; i < terrainTiles.Count; i++)
        {
            terrainTiles[i].ResetAllImages();
            UpdateTile(i, currentTiles[i]);
        }
    }
}
