using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelEditor : Map
{
    protected override void Start()
    {
        UpdateAllTerrains();
        saveDataPath = Application.persistentDataPath;
        if (File.Exists(saveDataPath+"/Levels.txt"))
        {
            allData = File.ReadAllText(saveDataPath+"/Levels.txt");
            UpdateAllDataList();
            ChangeIndex();
        }
        else
        {
            allData = "";
            allDataList.Clear();
            NewLevel();
        }
        for (int i = 0; i < terrainTiles.Count; i++)
        {
            terrainTiles[i].SetTileNumber(i);
        }
    }
    public string allData;
    public List<string> allDataList;
    protected void UpdateAllDataList()
    {
        allDataList = allData.Split("#").ToList();
        for (int i = 0; i < allDataList.Count; i++)
        {
            if (allDataList[i].Length < gridSize){allDataList.RemoveAt(i);}
        }
    }
    public int levelIndex = -1;
    public void ChangeIndex(bool right = true)
    {
        if (allDataList.Count <= 0){return;}
        if (right)
        {
            if (levelIndex+1 < allDataList.Count){levelIndex++;}
            else {levelIndex = 0;}
        }
        else
        {
            if (levelIndex > 0){levelIndex--;}
            else{levelIndex = allDataList.Count-1;}
        }
    }
    protected string saveDataPath;
    public Color transColor;
    public Color selectColor;
    public int currentlyEditing = -1;
    public List<Image> editingButtons;
    protected void SwitchEditing(int newEditing = -1)
    {
        for (int i = 0; i < editingButtons.Count; i++)
        {
            editingButtons[i].color = transColor;
        }
        terrainEditor.SetActive(false);
        if (newEditing == currentlyEditing)
        {
            currentlyEditing = -1;
            return;
        }
        currentlyEditing = newEditing;
        editingButtons[currentlyEditing].color = selectColor;
        if (currentlyEditing == 1)
        {
            terrainEditor.SetActive(true);
            UpdateTerrainEditor();
        }
    }
    // Levels need to store some information.
    public string currentLevelData;
    // Need to store the actual terrain that makes up the level.
    // This is already stored in allTiles in the base map class.
    //public List<string> currentTerrain;
    public List<int> allTerrains;
    protected void UpdateAllTerrains()
    {
        allTerrains.Clear();
        for (int i = 0; i < tileSprites.Count; i++)
        {
            allTerrains.Add(i);
        }
    }
    public int currentTerrainPage = 0;
    public void ChangeTerrainPage(bool right = true)
    {
        int lastPage = allTerrains.Count/terrainEditObjects.Count;
        if (allTerrains.Count%terrainEditObjects.Count == 0){lastPage--;}
        if (right)
        {
            if (currentTerrainPage+1<=lastPage){currentTerrainPage++;}
            else {currentTerrainPage=0;}
        }
        else
        {
            if (currentTerrainPage > 0){currentTerrainPage--;}
            else {currentTerrainPage=lastPage;}
        }
        currentlySelectedTerrain = -1;
        UpdateTerrainEditor();
    }
    public int currentlySelectedTerrain = -1;
    public void SelectTerrainType(int newType)
    {
        int selectedType = newType + (currentTerrainPage * terrainEditObjects.Count);
        if (currentlySelectedTerrain != selectedType)
        {
            currentlySelectedTerrain = selectedType;
        }
        else
        {
            currentlySelectedTerrain = -1;
        }
        HighlightSelectedTerrain();
    }
    protected void HighlightSelectedTerrain()
    {
        for (int i = 0; i < terrainEditButtons.Count; i++)
        {
            terrainEditButtons[i].ResetHighlight();
        }
        if (currentlySelectedTerrain >= 0)
        {
            // Highlight the appropiate spot on the page.
            int newIndex = currentlySelectedTerrain%terrainEditObjects.Count;
            terrainEditButtons[newIndex].Highlight(false);
        }
    }
    public GameObject terrainEditor;
    protected void UpdateTerrainEditor()
    {
        int pageShift = (currentTerrainPage * terrainEditObjects.Count);
        for (int i = 0; i < terrainEditObjects.Count; i++)
        {
            if (i < allTerrains.Count - pageShift)
            {
                terrainEditObjects[i].SetActive(true);
            }
            else
            {
                terrainEditObjects[i].SetActive(false);
            }
        }
        for (int j = 0; j < Mathf.Min(terrainEditButtons.Count, allTerrains.Count - pageShift); j++)
        {
            int type = allTerrains[j + pageShift];
            terrainEditButtons[j].UpdateColor(type);
            terrainEditButtons[j].UpdateImage(tileSprites[type]);
        }
        HighlightSelectedTerrain();
    }
    public List<GameObject> terrainEditObjects;
    public List<TerrainTile> terrainEditButtons;
    public void StartChangingTerrain()
    {
        SwitchEditing(1);
    }
    // Need to store events/battles/etc on each terrain.
    // Can move to other scenes as well, then specifics will decide the spawn point, if any.
    public List<string> currentEncounters;
    public List<string> encounterRowColumns;
    // Need specific info on what happens, types of battles/encounters/etc.
    public List<string> currentEncounterSpecifics;
    // Encounter can be one time or respawning.
    public List<string> currentEncounterRespawnFreq;
    // Need to know where to spawn the player when they first enter the level.
    public int spawnPoint = -1;
    public void StartChangingSpawnPoint()
    {
        SwitchEditing(0);
    }

    protected void UpdateMap()
    {
        for (int i = 0; i < terrainTiles.Count; i++)
        {
            terrainTiles[i].ResetImage();
            terrainTiles[i].ResetLocationImage();
            terrainTiles[i].ResetHighlight();
            terrainTiles[i].ResetText();
            UpdateTile(i, currentTiles[i]);
            // Also need to update the text.
        }
    }

    public override void MoveMap(int direction)
    {
        base.MoveMap(direction);
        UpdateMap();
    }

    public void NewLevel()
    {
        allTiles.Clear();
        currentEncounters.Clear();
        currentEncounterSpecifics.Clear();
        currentEncounterRespawnFreq.Clear();
        spawnPoint = -1;
        totalRows = gridSize;
        totalColumns = gridSize;
        for (int i = 0; i < totalRows * totalColumns; i++)
        {
            allTiles.Add("0");
            currentEncounters.Add("");
            currentEncounterSpecifics.Add("");
            currentEncounterRespawnFreq.Add("");
        }
    }

    protected void SaveCurrentLevel()
    {
        currentLevelData = "";
        currentLevelData += GameManager.instance.ConvertListToString(allTiles)+","+totalRows+","+totalColumns;
        currentLevelData += ","+GameManager.instance.ConvertListToString(currentEncounters);
        currentLevelData += ","+GameManager.instance.ConvertListToString(currentEncounterSpecifics);
        currentLevelData += ","+GameManager.instance.ConvertListToString(currentEncounterRespawnFreq);
        currentLevelData += ","+spawnPoint;
    }

    protected void LoadLevel()
    {
        string[] dataBlocks = currentLevelData.Split(",");
        allTiles = dataBlocks[0].Split("|").ToList();
        totalRows = int.Parse(dataBlocks[1]);
        totalColumns = int.Parse(dataBlocks[2]);
        currentEncounters = dataBlocks[3].Split("|").ToList();
        currentEncounterSpecifics = dataBlocks[4].Split("|").ToList();
        currentEncounterRespawnFreq = dataBlocks[5].Split("|").ToList();
        spawnPoint = int.Parse(dataBlocks[6]);
    }
}
