using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverworldManager : Map
{
    // bools
    protected bool interactable = true;
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
    public BasicSpriteManager locationSprites;
    public List<string> indexNameDictionary;

    protected override void Start()
    {
        currentLevel = GameManager.instance.currentLevel;
        if (currentLevel < 0){currentLevel = 0;}
        currentLocation = GameManager.instance.currentLocation;
        LoadLevel();
        interactable = true;
    }

    protected void LoadLevel(bool newLevel = false, int previousLevel = -1)
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
        if (previousLevel >= 0)
        {
            SpawnNearPreviousEntrance(previousLevel);
            GameManager.instance.UpdateLocation(currentLocation, currentLevel);
        }
        pathfinder.SetTotalRowsColumns(totalRows, totalColumns);
        pathfinder.SetAllTiles(allTiles);
        UpdateMap();
    }

    protected void SpawnNearPreviousEntrance(int previousLevel)
    {
        int previousEntranceLocation = -1;
        // Find an appropriate place to spawn the player, close to the previous level entrance.
        // Find the location of the previous level entrance.
        for (int i = 0; i < levelLocations.Count; i++)
        {
            if (levelLocations[i] == "1" && int.Parse(locationSpecifics[i]) == previousLevel)
            {
                previousEntranceLocation = i;
                break;
            }
        }
        if (previousEntranceLocation >= 0)
        {
            // Find an appropriate spawn point.
            pathfinder.RecursiveAdjacency(previousEntranceLocation);
            List<int> adjacentFromEntrance = pathfinder.adjacentTiles;
            for (int i = 0; i < adjacentFromEntrance.Count; i++)
            {
                if (allTiles[adjacentFromEntrance[i]] != "2")
                {
                    currentLocation = adjacentFromEntrance[i];
                    break;
                }
            }
        }
    }

    protected void UpdateMap()
    {
        UpdateCenterTile(currentLocation);
        // Always center the map around the player.
        // This looks bad half the time.
        //UpdateCenterTile(currentLocation);
        for (int i = 0; i < terrainTiles.Count; i++)
        {
            terrainTiles[i].ResetAllImages();
            UpdateTile(i, currentTiles[i]);
            if (currentTiles[i] == currentLocation)
            {
                terrainTiles[i].UpdateImage(playerSprite);
            }
            UpdateLocationFeatures(i, currentTiles[i]);
            // Need to draw the player and any special location features.
        }
    }

    protected override void UpdateTile(int imageIndex, int tileIndex)
    {
        // Undefined tiles are deep water.
        if (tileIndex < 0 || tileIndex >= (totalRows * totalColumns))
        {
            // Black tiles for out of bounds.
            terrainTiles[imageIndex].UpdateColor(-1);
        }
        else
        {
            int tileType = int.Parse(allTiles[tileIndex]);
            if (tileType >= 7)
            {
                terrainTiles[imageIndex].UpdateColor(0);
                terrainTiles[imageIndex].UpdateTileImage(tileSprites.allSprites[0]);
                terrainTiles[imageIndex].UpdateLocationImage(tileSprites.allSprites[tileType]);
                return;
            }
            terrainTiles[imageIndex].UpdateColor(tileType);
            terrainTiles[imageIndex].UpdateTileImage(tileSprites.allSprites[tileType]);
        }
    }

    protected void UpdateLocationFeatures(int imageIndex, int tileIndex)
    {
        if (tileIndex < 0 || tileIndex >= (totalRows * totalColumns)){return;}
        if (levelLocations[tileIndex].Length <= 0){return;}
        string featureName = indexNameDictionary[int.Parse(levelLocations[tileIndex])];
        if (featureName.Length <= 0){return;}
        terrainTiles[imageIndex].UpdateLocationImage(locationSprites.SpriteDictionary(featureName));
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
        while (Mathf.Abs(rowDiff) > 2)
        {
            // You are above the center row.
            if (rowDiff > 0)
            {
                rowDiff -= 1;
                exactCenter -= totalColumns * 1;
            }
            // You are below the center row.
            else
            {
                rowDiff += 1;
                exactCenter += totalColumns * 1;
            }
        }
        // Next deal with the column difference.
        int colDiff = centerColumn - currentColumn;
        while (Mathf.Abs(colDiff) > 3)
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

    public override void MoveMap(int direction)
    {
        if (!interactable){return;}
        int previousLocation = currentLocation;
        currentLocation = pathfinder.GetDestination(currentLocation, direction);
        // Can't move onto mountains.
        if (allTiles[currentLocation] == "2"){currentLocation = previousLocation;}
        UpdateMap();
        if (currentLocation != previousLocation)
        {
            MoveIntoTile(currentLocation);
        }
    }

    public override void ClickOnTile(int tileNumber)
    {
        if (!interactable){return;}
        if (currentLocation == currentTiles[tileNumber]){MoveIntoTile(currentLocation, false);}
        if (currentTiles[tileNumber] < 0){return;}
        List<int> path = new List<int>(pathfinder.DestReachable(currentLocation, currentTiles[tileNumber], currentTiles));
        if (path.Count <= 0){return;}
        StartCoroutine(MoveAlongPath(path));
    }

    IEnumerator MoveAlongPath(List<int> path)
    {
        interactable = false;
        for (int i = path.Count - 1; i > -1; i--)
        {
            currentLocation = path[i];
            UpdateMap();
            MoveIntoTile(currentLocation);
            yield return new WaitForSeconds(0.1f);
        }
        interactable = true;
    }

    protected void MoveIntoTile(int tileLocation, bool newDay = true)
    {
        GameManager.instance.UpdateLocation(currentLocation, currentLevel);
        if (newDay){GameManager.instance.NewDay();}
        switch (levelLocations[tileLocation])
        {
            case "0":
                StartBattle(tileLocation);
                break;
            case "1":
                SwitchLevel(tileLocation);
                break;
            case "2":
                // SwitchScenes(tileLocation);
                break;
            case "3":
                // FindTreasure(tileLocation);
                break;
            case "4":
                MoveToVillage(tileLocation);
                break;
        }
    }

    protected void StartBattle(int battleLocation)
    {
        int battleNumber = DetermineBattleNumber(battleLocation);
        GameManager.instance.StartRandomBattle(battleNumber);
    }

    protected int DetermineBattleNumber(int battleLocation)
    {
        string possibleBattles = locationSpecifics[battleLocation];
        if (possibleBattles.Length <= 0){return -1;}
        string[] ranges = possibleBattles.Split(",");
        List<int> battleIndexList = new List<int>();
        for (int i = 0; i < ranges.Length; i++)
        {
            // Go through each possible range.
            string[] innerRange = ranges[i].Split("-");
            // Some are actual ranges.
            if (innerRange.Length > 1)
            {
                int lowerBound = int.Parse(innerRange[0]);
                int upperBound = int.Parse(innerRange[1]);
                for (int j = lowerBound; j < upperBound + 1; j++)
                {
                    battleIndexList.Add(j);
                }
            }
            // Some are individual battles.
            else if (innerRange.Length == 1)
            {
                battleIndexList.Add(int.Parse(innerRange[0]));
            }
        }
        // Then pick a battle from the possible battles.
        int rng = Random.Range(0, battleIndexList.Count);
        return battleIndexList[rng];
    }

    protected void SwitchLevel(int levelLocation)
    {
        string newLevel = locationSpecifics[levelLocation];
        if (newLevel.Length <= 0){return;}
        int previousLevel = currentLevel;
        currentLevel = int.Parse(newLevel);
        LoadLevel(true, previousLevel);
    }

    protected void MoveToVillage(int tileLocation)
    {
        // Your village.
        if (locationSpecifics[tileLocation] == "0")
        {
            GameManager.instance.ReturnToVillage();
        }
    }
}
