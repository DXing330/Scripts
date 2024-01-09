using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleMapEditor : Map
{
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
    // 0 changes map, 1 changes difficulty, 2 changes enemy and locations, 3 changes spawn points.
    public int currentlyEditing = 1;
    public string currentBattleData;
    public string currentMap;
    public string currentDifficulty;
    public List<string> currentEnemies;
    public List<string> currentEnemyLocations;
    public List<string> currentSpawnPoints;


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
        
    }

    protected override void Start()
    {
        // Load once at the start.
        saveDataPath = Application.persistentDataPath;
        if (File.Exists(saveDataPath+"/BattleMaps_"+baseTerrain+".txt"))
        {
            allBattles = File.ReadAllText(saveDataPath+"/BattleMaps_"+baseTerrain+".txt");
        }
        else
        {
            allBattles = "";
        }
        UpdateAllBattlesList();
        for (int i = 0; i < terrainTiles.Count; i++)
        {
            terrainTiles[i].SetTileNumber(i);
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
    }

    public bool SaveCurrentBattle()
    {
        currentBattleData = currentMap+","+currentDifficulty+","+GameManager.instance.ConvertListToString(currentEnemies)+","+GameManager.instance.ConvertListToString(currentEnemyLocations)+","+GameManager.instance.ConvertListToString(currentSpawnPoints);
        // Need to check some conditions to see if it's a valid battle.
        // Battle needs enemies.
        if (currentEnemies.Count <= 0){return false;}
        // Battle needs spawn points.
        if (currentSpawnPoints.Count < 9){return false;}
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

    private void ChangeBaseMap(int newMapIndex)
    {
        switch (baseTerrain)
        {
            case "1":
                int totalMaps = GameManager.instance.forestFixedTerrains.Count;
                if (newMapIndex < 0){newMapIndex += totalMaps;}
                if (newMapIndex >= GameManager.instance.forestFixedTerrains.Count){newMapIndex -= totalMaps;}
                break;
        }
        currentMap = newMapIndex.ToString();
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
}
