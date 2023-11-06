using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OverworldMap : Map
{
    public SceneManager sceneManager;
    public int playerLocation;
    public Sprite playerSprite;
    public List<Sprite> locationSprites;
    public GameObject interactButton;
    public TMP_Text interactText;
    private int currentColumn;
    private int currentRow;
    public string allTilesString;
    public string allLocationsString;
    public string allLocationSpecifics;
    public List<string> allLocations;
    public List<string> locationSpecifics;
    public List<string> exploredTiles;

    protected void Save()
    {}

    protected void Load()
    {}
    
    protected override void Start()
    {
        allTiles = allTilesString.Split("|").ToList();
        // Might need to be able to save and change these as the story progresses.
        // People move, cities fall, etc.
        allLocations = allLocationsString.Split("|").ToList();
        locationSpecifics = allLocationSpecifics.Split("|").ToList();
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
            UpdateLocationImage(i, currentTiles[i]);
        }
        terrainTiles[terrainTiles.Count/2].UpdateImage(playerSprite);
        if (UpdateLocationImage(terrainTiles.Count/2, currentTiles[terrainTiles.Count/2]))
        {
            terrainTiles[terrainTiles.Count/2].ResetImage();
        }
        UpdateInteractButton();
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
            return;
        }
        GameManager.instance.UpdateLocation(playerLocation);
        UpdateCenterTile();
        UpdateMap();
        MoveIntoTile();
    }

    protected bool CheckIfMoveable(int direction)
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

    protected void DetermineRowColumn()
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

    protected bool UpdateLocationImage(int imageIndex, int tileIndex)
    {
        if (tileIndex < 0)
        {
            return false;
        }
        string tileSpecifics = locationSpecifics[tileIndex];
        // Could just make a hashmap.
        int spriteIndex = -1;
        for (int i = 0; i < locationSprites.Count; i++)
        {
            if (locationSprites[i].name == tileSpecifics)
            {
                spriteIndex = i;
                break;
            }
        }
        if (spriteIndex < 0)
        {
            return false;
        }
        terrainTiles[imageIndex].UpdateLocationImage(locationSprites[spriteIndex]);
        return true;
    }

    protected void UpdateInteractButton()
    {
        interactButton.SetActive(true);
        interactText.text = "";
        switch (allLocations[playerLocation])
        {
            case "Shop":
                interactText.text = "Talk";
                break;
            case "Battle":
                interactText.text = "Attack";
                break;
        }
        if (interactText.text.Length < 3)
        {
            interactButton.SetActive(false);
        }
    }

    public void InteractWithLocation()
    {
        switch (allLocations[playerLocation])
        {
            case "Shop":
                TalkToMerchant(locationSpecifics[playerLocation]);
                break;
            case "Battle":
                EnterFixedBatte(locationSpecifics[playerLocation]);
                break;
        }
    }

    private void TalkToMerchant(string merchantType)
    {
        string shopName = merchantType+"Shop";
        sceneManager.MoveScenes(shopName);
    }

    private void MoveIntoTile()
    {
        switch (allLocations[playerLocation])
        {
            case "RandomBattle":
                EnterBattle(locationSpecifics[playerLocation]);
                break;
        }
    }

    private void EnterFixedBatte(string battleName)
    {
        GameManager.instance.GenerateFixedBattle(battleName);
    }

    private void UpdateWinCon(string winCon = "")
    {
        if (winCon.Length < 5)
        {
            GameManager.instance.ResetWinCon();
        }

    }

    private void EnterBattle(string locationDifficulty)
    {
        string[] locDiff = locationDifficulty.Split("=");
        GameManager.instance.SetRandomBattleLocationDifficulty(int.Parse(locDiff[0]), int.Parse(locDiff[1]));
    }
}
