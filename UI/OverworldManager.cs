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
        UpdateMap();
    }

    protected void UpdateMap()
    {
        // Always center the map around the player.
        UpdateCenterTile(currentLocation);
        for (int i = 0; i < terrainTiles.Count; i++)
        {
            terrainTiles[i].ResetImage();
            terrainTiles[i].ResetLocationImage();
            terrainTiles[i].ResetHighlight();
            terrainTiles[i].ResetText();
            UpdateTile(i, currentTiles[i]);
            terrainTiles[GetCenterTile()].UpdateImage(playerSprite);
            // Need to draw the player and any special location features.
        }
    }

    protected int GetCenterTile()
    {
        if (totalRows%2 == 1)
        {
            return (totalRows*totalColumns/2);
        }
        return (totalColumns/2)+(totalRows*totalColumns/2);
    }
}
