using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldMap : Map
{
    public int playerLocation;
    public Sprite playerSprite;
    private int currentColumn;
    private int currentRow;
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
        playerLocation = GameManager.instance.location;
        UpdateCenterTile();
        UpdateMap();
    }

    protected override void AddCurrentTile(int row, int column)
    {
        if (row < 0 || column < 0 || column >= fullSize || row >= fullSize)
        {
            currentTiles.Add(-1);
            return;
        }
        currentTiles.Add((row*fullSize)+column);
    }

    protected override void UpdateTile(int imageIndex, int tileIndex)
    {
        // Undefined tiles are black.
        if (tileIndex < 0 || tileIndex >= (fullSize * fullSize))
        {
            // Make the edges of the map deep water.
            terrainTiles[imageIndex].UpdateColor(4);
            terrainTiles[imageIndex].UpdateTileImage(tileSprites[4]);
        }
        else
        {
            int tileType = int.Parse(allTiles[tileIndex]);
            terrainTiles[imageIndex].UpdateColor(tileType);
            terrainTiles[imageIndex].UpdateTileImage(tileSprites[tileType]);
        }
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
            UpdateTile(i, currentTiles[i]);
        }
        terrainTiles[terrainTiles.Count/2].UpdateImage(playerSprite);
    }

    public void MovePlayer(int direction)
    {
        if (!CheckIfMoveable(direction))
        {
            return;
        }
        int previousLocation = playerLocation;
        switch (direction)
        {
            case 0:
                playerLocation -= fullSize;
                break;
            case 1:
                playerLocation++;
                break;
            case 2:
                playerLocation += fullSize;
                break;
            case 3:
                playerLocation--;
                break;
        }
        // Don't move through mountains.
        if (int.Parse(allTiles[playerLocation]) == 2)
        {
            playerLocation = previousLocation;
        }
        GameManager.instance.UpdateLocation(playerLocation);
        UpdateCenterTile();
        UpdateMap();
    }

    private bool CheckIfMoveable(int direction)
    {
        DetermineRowColumn();
        switch (direction)
        {
            case 0:
                if (currentRow <= 0)
                {
                    return false;
                }
                return true;
            case 1:
                if (currentColumn >= fullSize - 1)
                    {
                        return false;
                    }
                    return true;
            case 2:
                if (currentRow >= fullSize - 1)
                {
                    return false;
                }
                return true;
            case 3:
                if (currentColumn <= 0)
                {
                    return false;
                }
                return true;
        }
        return false;
    }

    private void DetermineRowColumn()
    {
        currentRow = 0;
        currentColumn = 0;
        int locationIndex = playerLocation;
        while (locationIndex >= fullSize)
        {
            locationIndex -= fullSize;
            currentRow++;
        }
        currentColumn += locationIndex;
    }
}
