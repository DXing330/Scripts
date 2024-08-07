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
    public List<string> possibleEnemies;
    public List<int> enemyCosts;
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
        currentBattleData = allBattlesList[currentBattleIndex];
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
    public List<Image> changeEditingButtons;
    public List<GameObject> changeEditingTabs;
    protected void SetCurrentlyEditing(int newEditing)
    {
        // Reset to defaults then change as needed.
        for (int i = 0; i < changeEditingButtons.Count; i++)
        {
            changeEditingButtons[i].color = transColor;
        }
        for (int i = 0; i < changeEditingTabs.Count; i++)
        {
            changeEditingTabs[i].SetActive(false);
        }
        currentlyEditing = newEditing;
        if (currentlyEditing < 0){return;}
        changeEditingButtons[currentlyEditing].color = highlightColor;
        switch (currentlyEditing)
        {
            case 0:
                changeEditingTabs[0].SetActive(true);
                UpdatePageOfEnemies();
                break;
            case 1:
                changeEditingTabs[1].SetActive(true);
                UpdatePageOfRandomEnemies();
                break;
            case 4:
                changeEditingTabs[2].SetActive(true);
                UpdatePageOfRewards();
                break;
        }
    }
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
    public string currentReward;
    public List<TMP_Text> rewardTexts;
    public void StartChangingRewards()
    {
        if (currentlyEditing != 4)
        {
            SetCurrentlyEditing(4);
        }
        else
        {
            SetCurrentlyEditing(-1);
        }
    }
    protected void UpdatePageOfRewards()
    {
        string[] allRewards = currentReward.Split("|");
        for (int i = 0; i < allRewards.Length; i++)
        {
            rewardTexts[i].text = allRewards[i];
        }
    }
    public void StartChangingEnemies()
    {
        if (currentlyEditing != 0)
        {
            SetCurrentlyEditing(0);
        }
        else
        {
            SetCurrentlyEditing(-1);
        }
        currentlySelectedEnemyType = -1;
        currentlySelectedEnemyLocation = -1;
    }
    public void IncreaseReward(int type)
    {
        List<string> allRewards = currentReward.Split("|").ToList();
        allRewards[type] = (int.Parse(allRewards[type]) + 1).ToString();
        currentReward = GameManager.instance.utility.ConvertListToString(allRewards);
        UpdatePageOfRewards();
    }
    public void DecreaseReward(int type)
    {
        List<string> allRewards = currentReward.Split("|").ToList();
        int currentRewardAmount = int.Parse(allRewards[type]);
        if (currentRewardAmount <= 0){return;}
        allRewards[type] = (currentRewardAmount - 1).ToString();
        currentReward = GameManager.instance.utility.ConvertListToString(allRewards);
        UpdatePageOfRewards();
    }
    public List<TerrainTile> possibleEnemyButtons;
    public List<GameObject> possibleEnemyObjects;
    public int currentPageOfEnemies = 0;
    public void ChangePageOfEnemies(bool right = true)
    {
        // Count how many pages there are.
        int lastPage = possibleEnemies.Count/possibleEnemyButtons.Count;
        // If it's an exact fit then there's one less page.
        if (possibleEnemies.Count%possibleEnemyButtons.Count == 0){lastPage--;}
        if (right)
        {
            if (currentPageOfEnemies+1<=lastPage){currentPageOfEnemies++;}
            else {currentPageOfEnemies = 0;}
        }
        else
        {
            if (currentPageOfEnemies > 0){currentPageOfEnemies--;}
            else {currentPageOfEnemies = lastPage;}
        }
        currentlySelectedEnemyType = -1;
        UpdatePageOfEnemies();
    }
    protected void UpdatePageOfEnemies()
    {
        int pageShift = (currentPageOfEnemies * possibleEnemyObjects.Count);
        for (int i = 0; i < possibleEnemyObjects.Count; i++)
        {
            // We can do the switch for different enemy types later for now just hardcode forest enemies.
            // Probably we'll just make this a base class and make a different scene for every different terrain type.
            if (i < possibleEnemies.Count - pageShift)
            {
                possibleEnemyObjects[i].SetActive(true);
            }
            else
            {
                possibleEnemyObjects[i].SetActive(false);
            }
        }
        for (int j = 0; j < Mathf.Min(possibleEnemyButtons.Count, possibleEnemies.Count - pageShift); j++)
        {
            string enemyType = possibleEnemies[j + pageShift];
            possibleEnemyButtons[j].UpdateImage(actorSprites.SpriteDictionary(enemyType));
        }
        HighlightSelectedEnemyType();
    }
    protected void HighlightSelectedEnemyType()
    {
        for (int i = 0; i < possibleEnemyButtons.Count; i++)
        {
            possibleEnemyButtons[i].ResetHighlight();
        }
        if (currentlySelectedEnemyType >= 0)
        {
            // Highlight the appropiate spot on the page.
            int newIndex = currentlySelectedEnemyType%possibleEnemyObjects.Count;
            possibleEnemyButtons[newIndex].Highlight(false);
        }
    }
    public int currentlySelectedEnemyType = -1;
    public void SelectEnemyType(int newType)
    {
        int selectedType = newType + (currentPageOfEnemies * possibleEnemyObjects.Count);
        if (currentlySelectedEnemyType != selectedType)
        {
            currentlySelectedEnemyType = selectedType;
        }
        else
        {
            currentlySelectedEnemyType = -1;
        }
        HighlightSelectedEnemyType();
    }
    public int currentlySelectedEnemyLocation = -1;
    public List<string> currentEnemies;
    public List<string> currentEnemyLocations;
    public int totalSpawnPoints = 9;
    public TMP_Text unusedSpawnPointsText;
    protected void UpdateUnusedSpawnPointsText()
    {
        int remainder = totalSpawnPoints - currentSpawnPoints.Count;
        unusedSpawnPointsText.text = remainder.ToString();
    }
    public List<string> currentSpawnPoints;
    public void StartChangingSpawnPoints()
    {
        if (currentlyEditing != 2)
        {
            SetCurrentlyEditing(2);
        }
        else
        {
            SetCurrentlyEditing(-1);
        }
        currentlySelectedSpawnPoint = -1;
    }
    public int currentlySelectedSpawnPoint = -1;
    public List<string> currentRandomEnemies;
    public void IncreaseRandomEnemyAmount(int index)
    {
        int pageShift = (currentPageOfRandomEnemies * randomEnemyAmountBoxes.Count);
        string enemyType = possibleEnemies[index + pageShift];
        currentRandomEnemies.Add(enemyType);
        UpdatePageOfRandomEnemies();
        UpdateUnusedEnemySpawnPointsText();
    }
    public void DecreaseRandomEnemyAmount(int index)
    {
        int pageShift = (currentPageOfRandomEnemies * randomEnemyAmountBoxes.Count);
        string enemyType = possibleEnemies[index + pageShift];
        int indexOf = currentRandomEnemies.IndexOf(enemyType);
        if (indexOf >= 0){currentRandomEnemies.RemoveAt(indexOf);}
        UpdatePageOfRandomEnemies();
        UpdateUnusedEnemySpawnPointsText();
    }
    public void StartChangingRandomEnemies()
    {
        if (currentlyEditing != 1)
        {
            SetCurrentlyEditing(1);
        }
        else
        {
            SetCurrentlyEditing(-1);
        }
    }
    public int currentPageOfRandomEnemies = 0;
    public void ChangePageOfRandomEnemies(bool right = true)
    {
        // Count how many pages there are.
        int lastPage = possibleEnemies.Count/possibleEnemyButtons.Count;
        // If it's an exact fit then there's one less page.
        if (possibleEnemies.Count%possibleEnemyButtons.Count == 0){lastPage--;}
        if (right)
        {
            if (currentPageOfRandomEnemies+1<=lastPage){currentPageOfRandomEnemies++;}
            else {currentPageOfRandomEnemies = 0;}
        }
        else
        {
            if (currentPageOfRandomEnemies > 0){currentPageOfRandomEnemies--;}
            else {currentPageOfRandomEnemies = lastPage;}
        }
        UpdatePageOfRandomEnemies();
    }
    protected void UpdatePageOfRandomEnemies()
    {
        int pageShift = (currentPageOfRandomEnemies * randomEnemyAmountBoxes.Count);
        for (int i = 0; i < randomEnemyAmountBoxes.Count; i++)
        {
            if (i < possibleEnemies.Count - pageShift)
            {
                randomEnemyAmountBoxes[i].SetActive(true);
            }
            else
            {
                randomEnemyAmountBoxes[i].SetActive(false);
            }
        }
        for (int j = 0; j < Mathf.Min(possibleRandomEnemies.Count, possibleEnemies.Count - pageShift); j++)
        {
            string enemyType = possibleEnemies[j + pageShift];
            possibleRandomEnemies[j].UpdateImage(actorSprites.SpriteDictionary(enemyType));
            int randomEnemyCount = currentRandomEnemies.Count(s => s == enemyType);
            randomEnemyAmountTexts[j].text = randomEnemyCount.ToString();
        }
    }
    public List<GameObject> randomEnemyAmountBoxes;
    public List<TerrainTile> possibleRandomEnemies;
    public List<TMP_Text> randomEnemyAmountTexts;
    public List<string> currentRandomEnemySpawnPoints;
    public void StartChangingRandomEnemySpawnPoints()
    {
        if (currentlyEditing != 3)
        {
            SetCurrentlyEditing(3);
        }
        else
        {
            SetCurrentlyEditing(-1);
        }
    }
    public int currentlySelectedEnemySpawnPoint = -1;
    public TMP_Text unusedEnemySpawnPointsText;
    protected void UpdateUnusedEnemySpawnPointsText()
    {
        // Can have more spawn points than enemies, but not less.
        int remainder = Mathf.Max(0, currentRandomEnemies.Count - currentRandomEnemySpawnPoints.Count);
        unusedEnemySpawnPointsText.text = remainder.ToString();
    }

    // Loading is done once at the start so a specific load function isn't needed.
    protected void Load()
    {

    }

    public void StopEditingBattles()
    {
        GameManager.instance.RefreshBattles();
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
            allBattles = GameManager.instance.utility.ConvertListToString(allBattlesList, "#");
        }
        File.WriteAllText(saveDataPath+"/BattleMaps_"+baseTerrain+".txt", allBattles);
    }

    public void NewBattle()
    {
        currentBattleIndex = -1;
        currentPageOfEnemies = 0;
        UpdateIndexText();
        SetCurrentlyEditing(-1);
        SetCurrentMap(0);
        currentEnemies.Clear();
        currentEnemyLocations.Clear();
        currentSpawnPoints.Clear();
        currentRandomEnemies.Clear();
        currentRandomEnemySpawnPoints.Clear();
        currentReward = "0|0|0";
        LoadBaseMap();
    }

    public void DeleteBattle()
    {
        // Can't delete the last battle.
        // You probably should be allowed to but whatever.
        if (allBattlesList.Count <= 1){return;}
        if (currentBattleIndex < 0){return;}
        allBattlesList.RemoveAt(currentBattleIndex);
        allBattles = GameManager.instance.utility.ConvertListToString(allBattlesList, "#");
        File.WriteAllText(saveDataPath+"/BattleMaps_"+baseTerrain+".txt", allBattles);
        ChangeIndex();
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
            int enemyLocation = int.Parse(currentEnemyLocations[i]);
            int indexOfActor = currentTiles.IndexOf(enemyLocation);
            if (indexOfActor >= 0)
            {
                UpdateActorImage(indexOfActor, currentEnemies[i]);
                if (currentlyEditing == 2)
                {
                    if (currentlySelectedEnemyLocation == enemyLocation)
                    {
                        terrainTiles[indexOfActor].Highlight(false);
                    }
                }
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
                terrainTiles[indexOfSpawn].Highlight();
                if (currentlyEditing == 3)
                {
                    if (spawnPoint == currentlySelectedEnemySpawnPoint)
                    {
                        terrainTiles[indexOfSpawn].Highlight(false);
                    }
                }
            }
        }
        // Update enemy spawn points.
        for (int i = 0; i < currentRandomEnemySpawnPoints.Count; i++)
        {
            if (currentRandomEnemySpawnPoints[i].Length <= 0){continue;}
            int spawnPoint = int.Parse(currentRandomEnemySpawnPoints[i]);
            int indexOfSpawn = currentTiles.IndexOf(spawnPoint);
            if (indexOfSpawn >= 0)
            {
                terrainTiles[indexOfSpawn].Highlight(false);
                if (currentlyEditing == 5)
                {
                    if (spawnPoint == currentlySelectedEnemySpawnPoint)
                    {
                        terrainTiles[indexOfSpawn].Highlight();
                    }
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
        currentBattleData = currentMap+","+currentReward+","+GameManager.instance.utility.ConvertListToString(currentEnemies)+","+GameManager.instance.utility.ConvertListToString(currentEnemyLocations)+","+GameManager.instance.utility.ConvertListToString(currentSpawnPoints)+","+GameManager.instance.utility.ConvertListToString(currentRandomEnemies)+","+GameManager.instance.utility.ConvertListToString(currentRandomEnemySpawnPoints);
        // Need to check some conditions to see if it's a valid battle.
        // Battle needs enemies.
        if (currentEnemies.Count <= 0 && currentRandomEnemies.Count <= 0){return false;}
        // Battle needs spawn points for both players and enemies.
        if (currentSpawnPoints.Count < totalSpawnPoints){return false;}
        if (currentRandomEnemySpawnPoints.Count < currentRandomEnemies.Count){return false;}
        return true;
    }

    public void SaveCurrentBattleVoid()
    {
        currentBattleData = currentMap+","+currentReward+","+GameManager.instance.utility.ConvertListToString(currentEnemies)+","+GameManager.instance.utility.ConvertListToString(currentEnemyLocations)+","+GameManager.instance.utility.ConvertListToString(currentSpawnPoints)+","+GameManager.instance.utility.ConvertListToString(currentRandomEnemies)+","+GameManager.instance.utility.ConvertListToString(currentRandomEnemySpawnPoints);
    }

    public void UndoEdits()
    {
        LoadCurrentBattleData();
        // Need to update the map.
    }

    protected void LoadCurrentBattleData()
    {
        string[] previousData = currentBattleData.Split(",");
        currentPageOfEnemies = 0;
        currentMap = previousData[0];
        currentReward = previousData[1];
        currentEnemies = previousData[2].Split("|").ToList();
        currentEnemyLocations = previousData[3].Split("|").ToList();
        GameManager.instance.utility.RemoveEmptyListItems(currentEnemies);
        GameManager.instance.utility.RemoveEmptyListItems(currentEnemyLocations);
        currentSpawnPoints = previousData[4].Split("|").ToList();
        if (previousData.Length > 5)
        {
            currentRandomEnemies = previousData[5].Split("|").ToList();
            currentRandomEnemySpawnPoints = previousData[6].Split("|").ToList();
            GameManager.instance.utility.RemoveEmptyListItems(currentRandomEnemies);
            GameManager.instance.utility.RemoveEmptyListItems(currentRandomEnemySpawnPoints);
        }
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
        if (currentEnemyLocations.Count > 0)
        {
            for (int i = 0; i < currentEnemyLocations.Count; i++)
            {
                if (int.Parse(currentEnemyLocations[i]) >= totalRows * totalColumns)
                {
                    currentEnemyLocations.RemoveAt(i);
                    currentEnemies.RemoveAt(i);
                }
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
        // Nothing happens if you click on darkness.
        if (currentTiles[tileNumber] < 0){return;}
        switch (currentlyEditing)
        {
            case 0:
                SetEnemy(tileNumber);
                HighlightSelectedEnemyType();
                UpdateMap();
                break;
            case 2:
                SetSpawnPoint(tileNumber);
                UpdateUnusedSpawnPointsText();
                UpdateMap();
                break;
            case 3:
                SetRandomEnemySpawnPoint(tileNumber);
                UpdateUnusedEnemySpawnPointsText();
                UpdateMap();
                break;
        }
    }

    protected void SetSpawnPoint(int tileNumber)
    {
        string spawnLocation = currentTiles[tileNumber].ToString();
        // Can't spawn on enemies.
        if (currentEnemyLocations.Contains(spawnLocation)){return;}
        if (currentRandomEnemySpawnPoints.Contains(spawnLocation)){return;}
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

    protected void SetEnemy(int tileNumber)
    {
        // Either place a new enemy or move an enemy around.
        //if (currentlySelectedEnemyType < 0 && currentlySelectedEnemyLocation < 0){return;}
        string spawnLocation = currentTiles[tileNumber].ToString();
        // Can't place an enemy on a spawn point.
        if (currentSpawnPoints.Contains(spawnLocation)){return;}
        if (currentRandomEnemySpawnPoints.Contains(spawnLocation)){return;}
        // Can't place an enemy on another enemy.
        int overlap = 0;
        if (currentEnemyLocations.Contains(spawnLocation)){overlap = 1;}
        // If you click on an empty tile.
        if (overlap == 0)
        {
            if (currentlySelectedEnemyType >= 0)
            {
                // Place them down at the location.
                currentEnemies.Add(possibleEnemies[currentlySelectedEnemyType]);
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
                int indexOf = currentEnemyLocations.IndexOf(currentlySelectedEnemyLocation.ToString());
                currentEnemyLocations.RemoveAt(indexOf);
                currentEnemies.RemoveAt(indexOf);
                currentlySelectedEnemyLocation = -1;
            }
            else
            {
                currentlySelectedEnemyLocation = currentTiles[tileNumber];
            }
            currentlySelectedEnemyType = -1;
        }
    }

    protected void SetRandomEnemySpawnPoint(int tileNumber)
    {
        string spawnLocation = currentTiles[tileNumber].ToString();
        if (currentEnemyLocations.Contains(spawnLocation)){return;}
        if (currentSpawnPoints.Contains(spawnLocation)){return;}
        int overlap = 0;
        if (currentRandomEnemySpawnPoints.Contains(spawnLocation)){overlap = 1;}
        // Can either place or remove random spawn points.
        if (overlap == 0)
        {
            currentRandomEnemySpawnPoints.Add(spawnLocation);
        }
        else
        {
            if (currentlySelectedEnemySpawnPoint == int.Parse(spawnLocation))
            {
                int indexOf = currentRandomEnemySpawnPoints.IndexOf(spawnLocation);
                currentRandomEnemySpawnPoints.RemoveAt(indexOf);
                currentlySelectedEnemySpawnPoint = -1;
            }
            else{currentlySelectedEnemySpawnPoint = int.Parse(spawnLocation);}
        }
    }
}
