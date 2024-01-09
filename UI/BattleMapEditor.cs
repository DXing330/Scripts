using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleMapEditor : Map
{
    public Color transColor;
    public Color highlightColor;
    public string allBattles;
    public string baseTerrain = "1";
    protected string saveDataPath;
    public List<string> allBattlesList;
    public ActorSprites actorSprites;
    // Things that are fixed/set in the game settings/editor.
    // Can be List<int>.
    public List<string> possibleForestEnemies;
    public List<int> forestEnemyCosts;
    public List<int> maxDifficultyCosts;
    // The current map being edited needs a base map, difficulty, enemies, enemy spawn points, player spawn points, reward for victory.
    // These vary depending on the map being edited.
    // Should all be List<string> for easier saving/loading.
    public int currentBattleIndex = -1;
    public void ChangeIndex(bool right = true)
    {
        if (right)
        {
            if (currentBattleIndex + 1 < allBattlesList.Count)
            {
                currentBattleIndex++;
            }
            else
            {
                currentBattleIndex = 0;
            }
        }
        else
        {
            if (currentBattleIndex > 0)
            {
                currentBattleIndex--;
            }
            else
            {
                currentBattleIndex = allBattlesList.Count - 1;
            }
        }
        UpdateIndexText();
        LoadCurrentBattleData();
        UpdateCenterTile();
        UpdateMap();
    }
    public TMP_Text indexText;
    protected virtual void UpdateIndexText()
    {
        if (currentBattleIndex < 0)
        {
            indexText.text = "New Battle";
        }
        else
        {
            indexText.text = "Battle "+(currentBattleIndex+1).ToString();
        }
    }
    // -1 changes what to edit, 0 changes map, 1 changes difficulty, 2 changes enemy and locations, 3 changes spawn points.
    public int currentlyEditing = -1;
    public string currentBattleData;
    public string currentMap;
    protected void SetCurrentMap(int newMap)
    {
        currentMap = newMap.ToString();
        UpdateCurrentMapText();
    }
    public TMP_Text currentMapText;
    protected void UpdateCurrentMapText()
    {
        currentMapText.text = (int.Parse(currentMap)+1).ToString();
    }
    public string currentDifficulty;
    public Image editEnemiesBG;
    public void StartChangingEnemies()
    {
        if (currentlyEditing != 2)
        {
            currentlyEditing = 2;
            editEnemiesBG.color = highlightColor;
        }
        else
        {
            currentlyEditing = -1;
            editEnemiesBG.color = transColor;
        }
        currentlySelectedEnemyType = -1;
        currentlySelectedEnemyLocation = -1;
    }
    public int currentlySelectedEnemyType = -1;
    public int currentlySelectedEnemyLocation = -1;
    public List<string> currentEnemies;
    public List<string> currentEnemyLocations;
    public int totalSpawnPoints = 9;
    public Image editSpawnBG;
    public TMP_Text unusedSpawnPointsText;
    protected void UpdateUnusedSpawnPointsText()
    {
        int remainder = totalSpawnPoints - currentSpawnPoints.Count;
        unusedSpawnPointsText.text = remainder.ToString();
    }
    public List<string> currentSpawnPoints;
    public void StartChangingSpawnPoints()
    {
        if (currentlyEditing != 3)
        {
            currentlyEditing = 3;
            editSpawnBG.color = highlightColor;
        }
        else
        {
            currentlyEditing = -1;
            editSpawnBG.color = transColor;
        }
        currentlySelectedSpawnPoint = -1;
    }
    public int currentlySelectedSpawnPoint = -1;

    // Loading is done once at the start so a specific load function isn't needed.
    protected void Load()
    {

    }

    // At the start check for valid maps.
    protected void UpdateAllBattlesList()
    {
        allBattlesList = allBattles.Split("#").ToList();
        for (int i = 0; i < allBattlesList.Count; i++)
        {
            if (allBattlesList[i].Length < 9)
            {
                allBattlesList.RemoveAt(i);
            }
        }
    }

    // Saves the maps to text files for persistance.
    public void PublishBattle()
    {
        // Save the current battle and then write its data if it's valid.
        if (!SaveCurrentBattle()){return;}
        // It's a new battle.
        if (currentBattleIndex < 0)
        {
            allBattles += "#"+currentBattleData;
            UpdateAllBattlesList();
            currentBattleIndex = allBattlesList.Count - 1;
            UpdateIndexText();
        }
        // It's an edit of another battle.
        else
        {
            allBattlesList[currentBattleIndex] = currentBattleData;
            allBattles = GameManager.instance.ConvertListToString(allBattlesList, "#");
        }
        File.WriteAllText(saveDataPath+"/BattleMaps_"+baseTerrain+".txt", allBattles);
    }

    public void NewBattle()
    {
        currentBattleIndex = -1;
        UpdateIndexText();
        currentlyEditing = -1;
        SetCurrentMap(0);
        currentDifficulty = "0";
        LoadBaseMap();
    }

    protected override void Start()
    {
        for (int i = 0; i < terrainTiles.Count; i++)
        {
            terrainTiles[i].SetTileNumber(i);
        }
        // Load once at the start.
        saveDataPath = Application.persistentDataPath;
        if (File.Exists(saveDataPath+"/BattleMaps_"+baseTerrain+".txt"))
        {
            allBattles = File.ReadAllText(saveDataPath+"/BattleMaps_"+baseTerrain+".txt");
            UpdateAllBattlesList();
            ChangeIndex();
        }
        else
        {
            allBattles = "";
            NewBattle();
        }
    }

    private void UpdateActorImage(int imageIndex, string actorName)
    {
        terrainTiles[imageIndex].UpdateImage(actorSprites.SpriteDictionary(actorName));
    }

    protected void UpdateMap()
    {
        for (int i = 0; i < terrainTiles.Count; i++)
        {
            terrainTiles[i].ResetImage();
            terrainTiles[i].ResetLocationImage();
            terrainTiles[i].ResetHighlight();
            UpdateTile(i, currentTiles[i]);
        }
        // For each enemy.
        for (int i = 0; i < currentEnemies.Count; i++)
        {
            // See if (and where) they should appear.
            int indexOfActor = currentTiles.IndexOf(int.Parse(currentEnemyLocations[i]));
            if (indexOfActor >= 0)
            {
                UpdateActorImage(indexOfActor, currentEnemies[i]);
            }
        }
        // Update player spawn points.
        for (int i = 0; i < currentSpawnPoints.Count; i++)
        {
            // See if they should appear.
            int spawnPoint = int.Parse(currentSpawnPoints[i]);
            int indexOfSpawn = currentTiles.IndexOf(spawnPoint);
            if (indexOfSpawn >= 0)
            {
                if (spawnPoint == currentlySelectedSpawnPoint)
                {
                    terrainTiles[indexOfSpawn].Highlight(false);
                }
                else
                {
                    terrainTiles[indexOfSpawn].Highlight();
                }
            }
        }
    }

    public override void MoveMap(int direction)
    {
        base.MoveMap(direction);
        UpdateMap();
    }

    public bool SaveCurrentBattle()
    {
        currentBattleData = currentMap+","+currentDifficulty+","+GameManager.instance.ConvertListToString(currentEnemies)+","+GameManager.instance.ConvertListToString(currentEnemyLocations)+","+GameManager.instance.ConvertListToString(currentSpawnPoints);
        // Need to check some conditions to see if it's a valid battle.
        // Battle needs enemies.
        if (currentEnemies.Count <= 0){return false;}
        // Battle needs spawn points.
        if (currentSpawnPoints.Count < totalSpawnPoints){return false;}
        return true;
    }

    public void SaveCurrentBattleVoid()
    {
        currentBattleData = currentMap+","+currentDifficulty+","+GameManager.instance.ConvertListToString(currentEnemies)+","+GameManager.instance.ConvertListToString(currentEnemyLocations)+","+GameManager.instance.ConvertListToString(currentSpawnPoints);
    }

    public void UndoEdits()
    {
        LoadCurrentBattleData();
        // Need to update the map.
    }

    protected void LoadCurrentBattleData()
    {
        string[] previousData = currentBattleData.Split(",");
        currentMap = previousData[0];
        currentDifficulty = previousData[1];
        currentEnemies = previousData[2].Split("|").ToList();
        currentEnemyLocations = previousData[3].Split("|").ToList();
        currentSpawnPoints = previousData[4].Split("|").ToList();
        LoadBaseMap();
    }

    public void LoadBaseMap()
    {
        string loadedData = "";
        // Changes all tiles, as well as row/column count.
        switch (baseTerrain)
        {
            case "1":
                loadedData = GameManager.instance.forestFixedTerrains[int.Parse(currentMap)];
                break;
        }
        string[] loadedDataBlocks = loadedData.Split(",");
        if (loadedDataBlocks.Length <= 2){return;}
        allTiles = loadedDataBlocks[0].Split("|").ToList();
        totalRows = int.Parse(loadedDataBlocks[1]);
        totalColumns = int.Parse(loadedDataBlocks[2]);
        // Remove out of bounds enemies and spawn points.
        for (int i = 0; i < currentEnemyLocations.Count; i++)
        {
            if (int.Parse(currentEnemyLocations[i]) >= totalRows * totalColumns)
            {
                currentEnemyLocations.RemoveAt(i);
                currentEnemies.RemoveAt(i);
            }
        }
        for (int i = 0; i < currentSpawnPoints.Count; i++)
        {
            if (int.Parse(currentSpawnPoints[i]) >= totalRows * totalColumns)
            {
                currentSpawnPoints.RemoveAt(i);
            }
        }
        // Auto center when changing the base map.
        UpdateCenterTile();
        UpdateMap();
    }

    protected void ChangeBaseMap(int newMapIndex)
    {
        switch (baseTerrain)
        {
            case "1":
                int totalMaps = GameManager.instance.forestFixedTerrains.Count;
                if (newMapIndex < 0){newMapIndex += totalMaps;}
                if (newMapIndex >= GameManager.instance.forestFixedTerrains.Count){newMapIndex -= totalMaps;}
                break;
        }
        SetCurrentMap(newMapIndex);
        LoadBaseMap();
    }

    public void SwitchBaseMap(bool right = true)
    {
        int currentMapIndex = int.Parse(currentMap);
        if (right)
        {
            ChangeBaseMap(currentMapIndex+1);
        }
        else
        {
            ChangeBaseMap(currentMapIndex-1);
        }
    }

    public override void ClickOnTile(int tileNumber)
    {
        switch (currentlyEditing)
        {
            case 3:
                SetSpawnPoint(tileNumber);
                UpdateUnusedSpawnPointsText();
                UpdateMap();
                break;
        }
    }

    protected void SetSpawnPoint(int tileNumber)
    {
        string spawnLocation = currentTiles[tileNumber].ToString();
        // Can't spawn on enemies.
        if (currentEnemyLocations.Contains(spawnLocation)){return;}
        // Check if this overlaps with a tile already in the spawn zone.
        int overlap = 0;
        if (currentSpawnPoints.Contains(spawnLocation)){overlap = 1;}
        // If you're selecting a new tile.
        if (currentlySelectedSpawnPoint < 0)
        {
            // If not enough spawn points then make a new one.
            if (overlap == 0 && currentSpawnPoints.Count < totalSpawnPoints)
            {
                currentSpawnPoints.Add(spawnLocation);
                return;
            }
            // Can pick a current spawn point.
            else if (overlap == 1)
            {
                currentlySelectedSpawnPoint = int.Parse(spawnLocation);
                return;
            }
        }
        // Can move spawn points around.
        else if (currentlySelectedSpawnPoint > 0)
        {
            // Don't move it to an overlapping region.
            // Replace it with the newly selected or drop it.
            if (overlap == 1)
            {
                if (currentlySelectedSpawnPoint == currentTiles[tileNumber])
                {
                    currentlySelectedSpawnPoint = -1;
                }
                else
                {
                    currentlySelectedSpawnPoint = currentTiles[tileNumber];
                }
                return;
            }
            // If possible then switch the spawn point to the new location.
            else if (overlap == 0)
            {
                int indexOf = currentSpawnPoints.IndexOf(currentlySelectedSpawnPoint.ToString());
                currentSpawnPoints[indexOf] = spawnLocation;
                currentlySelectedSpawnPoint = -1;
            }
        }
    }

    public void SelectEnemyType(int typeIndex)
    {
        if (currentlySelectedEnemyType == typeIndex)
        {
            currentlySelectedEnemyType = -1;
            return;
        }
        currentlySelectedEnemyType = typeIndex;
    }

    protected void SetEnemy(int tileNumber)
    {
        // Either place a new enemy or move an enemy around.
        if (currentlySelectedEnemyType < 0 && currentlySelectedEnemyLocation < 0){return;}
        string spawnLocation = currentTiles[tileNumber].ToString();
        // Can't place an enemy on a spawn point.
        if (currentSpawnPoints.Contains(spawnLocation)){return;}
        // Can't place an enemy on another enemy.
        int overlap = 0;
        if (currentEnemyLocations.Contains(spawnLocation)){overlap = 1;}
        // If you click on an empty tile.
        if (overlap == 0)
        {
            if (currentlySelectedEnemyType >= 0)
            {
                // Place them down at the location.
                currentEnemies.Add(possibleForestEnemies[currentlySelectedEnemyType]);
                currentEnemyLocations.Add(spawnLocation);
                currentlySelectedEnemyLocation = -1;
                return;
            }
            else if (currentlySelectedEnemyLocation >= 0)
            {
                // Move the enemy to the new location.
                int indexOf = currentEnemyLocations.IndexOf(currentlySelectedEnemyLocation.ToString());
                currentEnemyLocations[indexOf] = spawnLocation;
                currentlySelectedEnemyLocation = -1;

            }
        }
        // If you click on an enemy already on the map.
        else if (overlap == 1)
        {
            // We can do double clicks to delete enemies.
            if (currentlySelectedEnemyLocation == currentTiles[tileNumber])
            {
                // Delete the enemy.
                currentlySelectedEnemyLocation = -1;
            }
            else
            {
                currentlySelectedEnemyLocation = currentTiles[tileNumber];
            }
            currentlySelectedEnemyType = -1;
        }
    }
}
