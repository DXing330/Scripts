using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverworldManager : Map
{
    // Data.
    public int currentLevel;
    public int currentLocation;
    public string currentLevelData;
    public List<string> levelLocations;
    public List<string> locationSpecifics;
    public int currentLevelSpawnPoint = -1;
    // UI.
    // Could be a list later.
    public Sprite playerSprite;

    protected override void Start()
    {
        currentLevel = GameManager.instance.currentLevel;
        if (currentLevel < 0){currentLevel = 0;}
        currentLocation = GameManager.instance.currentLocation;
        LoadLevel();
    }

    protected void LoadLevel(bool newLevel = false)
    {
        currentLevelData = GameManager.instance.levelData.allLevelsList[currentLevel];
        string[] dataBlocks = currentLevelData.Split(",");
        allTiles = dataBlocks[0].Split("|").ToList();
        totalRows = int.Parse(dataBlocks[1]);
        totalColumns = int.Parse(dataBlocks[2]);
        levelLocations = dataBlocks[3].Split("|").ToList();
        locationSpecifics = dataBlocks[4].Split("|").ToList();
        currentLevelSpawnPoint = int.Parse(dataBlocks[6]);
        if (newLevel || currentLocation < 0){currentLocation = currentLevelSpawnPoint;}
        pathfinder.SetTotalRowsColumns(totalRows, totalColumns);
        pathfinder.SetAllTiles(allTiles);
        UpdateMap();
    }

    protected void UpdateMap()
    {
        UpdateCenterTile(currentLocation);
        // Always center the map around the player.
        // This looks bad half the time.
        //UpdateCenterTile(currentLocation);
        for (int i = 0; i < terrainTiles.Count; i++)
        {
            terrainTiles[i].ResetImage();
            terrainTiles[i].ResetLocationImage();
            terrainTiles[i].ResetHighlight();
            terrainTiles[i].ResetText();
            UpdateTile(i, currentTiles[i]);
            if (currentTiles[i] == currentLocation)
            {
                terrainTiles[i].UpdateImage(playerSprite);
            }
            // Need to draw the player and any special location features.
        }
    }

    protected override void UpdateTile(int imageIndex, int tileIndex)
    {
        // Undefined tiles are deep water.
        if (tileIndex < 0 || tileIndex >= (totalRows * totalColumns))
        {
            terrainTiles[imageIndex].UpdateColor(4);
            terrainTiles[imageIndex].UpdateTileImage(tileSprites[4]);
        }
        else
        {
            int tileType = int.Parse(allTiles[tileIndex]);
            if (tileType == 7)
            {
                terrainTiles[imageIndex].UpdateColor(0);
                terrainTiles[imageIndex].UpdateTileImage(tileSprites[0]);
                terrainTiles[imageIndex].UpdateLocationImage(tileSprites[tileType]);
                return;
            }
            terrainTiles[imageIndex].UpdateColor(tileType);
            terrainTiles[imageIndex].UpdateTileImage(tileSprites[tileType]);
        }
    }

    protected override void UpdateCenterTile(int tileNumber = -1)
    {
        // Determine the exact center of the map first, then adjust if the player is moving too far away from it.
        int exactCenter = 0;
        if (totalRows%2 == 1)
        {
            exactCenter = (totalRows*totalColumns/2);
        }
        else
        {
            exactCenter = (totalColumns/2)+(totalRows*totalColumns/2);
        }
        if (tileNumber < 0)
        {
            startIndex = exactCenter;
            DetermineCornerRowColumn();
            DetermineCurrentTiles();
            return;
        }
        // Find how far the player is from the center.
        int centerRow = GetRow(exactCenter);
        int centerColumn = GetColumn(exactCenter);
        int currentRow = GetRow(tileNumber);
        int currentColumn = GetColumn(tileNumber);
        // First deal with vertical distances.
        int rowDiff = centerRow - currentRow;
        while (Mathf.Abs(rowDiff) > 1)
        {
            // You are above the center row.
            if (rowDiff > 0)
            {
                rowDiff -= 2;
                exactCenter -= totalColumns * 2;
            }
            // You are below the center row.
            else
            {
                rowDiff += 2;
                exactCenter += totalColumns * 2;
            }
        }
        // Next deal with the column difference.
        int colDiff = centerColumn - currentColumn;
        while (Mathf.Abs(colDiff) > 1)
        {
            // You are left from center.
            if (colDiff > 0)
            {
                colDiff -= 2;
                exactCenter -= 2;
            }
            // You are right from center.
            else
            {
                colDiff += 2;
                exactCenter += 2;
            }
        }
        startIndex = exactCenter;
        DetermineCornerRowColumn();
        DetermineCurrentTiles();
    }

    public void MovePlayer(int direction)
    {
        currentLocation = pathfinder.GetDestination(currentLocation, direction);
        UpdateMap();
    }
}
