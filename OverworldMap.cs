using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldMap : Map
{
    public int playerLocation;
    public string allTilesString;
    public string allLocationsString;
    public List<string> allLocations;
    public List<string> exploredTiles;
    
    void Start()
    {
        allTiles = allTilesString.Split("|").ToList();
        // Might need to be able to save and change these as the story progresses.
        // People move, cities fall, etc.
        allLocations = allLocationsString.Split("|").ToList();
        fullSize = (int) Mathf.Sqrt(allTiles.Count);
        // Get the player's location from the GameManager.
        //playerLocation = (fullSize*fullSize)/2 + fullSize/2;
        UpdateCenterTile();
        UpdateMap();
    }

    private void UpdateCenterTile()
    {
        startIndex = playerLocation;
        DetermineCornerRowColumn();
        DetermineCurrentTiles();
    }

    private void UpdateMap()
    {
        for (int i = 0; i < terrainTiles.Count; i++)
        {
            terrainTiles[i].ResetImage();
            terrainTiles[i].ResetLocationImage();
            terrainTiles[i].ResetHighlight();
            terrainTiles[i].ResetAOEHighlight();
            UpdateTile(i, currentTiles[i]);
        }
    }
}
