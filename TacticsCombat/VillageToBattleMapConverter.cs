using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageToBattleMapConverter : MapUtility
{
    public VillageDataManager villageData;
    public List<string> battleTiles;
    public List<string> buildings;
    public List<string> buildingLocations;
    public List<string> buildingHealths;
    //public List<string> allies;
    public int pattern = 0;
    public List<string> enemies;
    public List<string> testEnemies;
    public List<string> testAllies;
    public int battleRows;
    public int battleColumns;

    void Start()
    {
        villageData = GameManager.instance.villageData;
    }

    public void StartConverting()
    {
        string allData = "";
        battleRows = villageData.totalRows + 4;
        battleColumns = villageData.totalColumns + 4;
        battleTiles.Clear();
        // First make sure that the layout is the same.
        int villageTileNumber = 0;
        for (int i = 0; i < battleRows; i++)
        {
            for (int j = 0; j < battleColumns; j++)
            {
                // Top rows and left side.
                //if (i*j <= 1){battleTiles.Add("1");}
                if (i == 0 || j == 0 || i == 1 || j == 1){battleTiles.Add("1");}
                // Bottom rows and right side.
                //else if (Mathf.Max(i,j) >= battleRows - 2){battleTiles.Add("1");}
                else if (i == battleRows - 1 || j == battleColumns - 1 || i == battleRows - 2 || j == battleColumns - 2){battleTiles.Add("1");}
                else
                {
                    battleTiles.Add(villageData.villageTiles[villageTileNumber]);
                    villageTileNumber++;
                }
            }
        }
        // Then make sure all the buildings are in place.
        // Convert locations to row,columns, then add a row and column to each then convert back.
        ConvertBuildingLocations();
        // Later spawn your garrison accordingly.
        allData += GameManager.instance.ConvertListToString(battleTiles)+"#";
        allData += GameManager.instance.ConvertListToString(buildings)+"#";
        allData += GameManager.instance.ConvertListToString(buildingLocations)+"#";
        allData += GameManager.instance.ConvertListToString(buildingHealths)+"#";
        allData += pattern+"#";
        allData += GameManager.instance.ConvertListToString(testEnemies)+"#";
        allData += GameManager.instance.ConvertListToString(testAllies)+"#";
        if (GameManager.instance.AtVillage())
        {
            // Player party will help fight.
            allData += "1";
        }
        else
        {
            // Player party will not help fight.
            allData += "0";
        }
        GameManager.instance.StartVillageBattle(allData);
    }

    protected void ConvertBuildingLocations()
    {
        buildings.Clear();
        buildingLocations.Clear();
        buildingHealths.Clear();
        List<string> allLocations = villageData.buildings.buildingLocations;
        List<string> allHealths = villageData.buildings.buildingHealths;
        int row = 1;
        int location = 0;
        for (int i = 0; i < allLocations.Count; i++)
        {
            buildings.Add(villageData.buildings.ReturnBuildingNameFromIndex(i));
            // Get the row that the building was originally on.
            location = int.Parse(allLocations[i]);
            row = GetRow(location, villageData.totalColumns);
            // Based on the row get the new location.
            buildingLocations.Add(ConvertToNewLocation(location, row).ToString());
            buildingHealths.Add(allHealths[i]);
        }
    }

    protected int ConvertToNewLocation(int location, int row, int addedRows = 4, int addedColumns = 4)
    {
        location += (addedRows/2)*battleColumns;
        // row + 1, since the first row is called row 0
        location += (addedColumns)*(row+1) - (addedColumns/2);
        return location;
    }
}
