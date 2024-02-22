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
    protected void LoadCurrentLevel()
    {
        currentLevelData = allDataList[levelIndex];
        LoadLevel();
    }
    public int levelIndex = -1;
    public TMP_Text indexText;
    protected void UpdateIndexText()
    {
        if (levelIndex < 0)
        {
            indexText.text = "New Level";
        }
        else
        {
            indexText.text = "Level "+(levelIndex+1).ToString();
        }
    }
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
        SwitchEditing(-1);
        UpdateIndexText();
        LoadCurrentLevel();
        UpdateCenterTile();
        UpdateMap();
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
        encounterEditor.SetActive(false);
        encounterSpecificEditor.SetActive(false);
        // Sometimes reset everything you've selected.
        if (newEditing < 0)
        {
            newEditing = -1;
            currentlySelectedTerrain = -1;
            currentlySelectedEncounter = -1;
            encounterToMove = -1;
            encounterSpecificToEdit = -1;
            return;
        }
        if (newEditing == currentlyEditing)
        {
            currentlyEditing = -1;
            return;
        }
        currentlyEditing = newEditing;
        editingButtons[currentlyEditing].color = selectColor;
        switch (currentlyEditing)
        {
            case 1:
                terrainEditor.SetActive(true);
                UpdateTerrainEditor();
                break;
            case 2:
                encounterEditor.SetActive(true);
                UpdateEncounterEditor();
                break;
            case 3:
                encounterSpecificEditor.SetActive(true);
                break;
        }
        UpdateMap();
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
        for (int i = 0; i < tileSprites.allSprites.Count; i++)
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
            terrainEditButtons[j].UpdateImage(tileSprites.allSprites[type]);
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
    public GameObject encounterEditor;
    public List<Image> encounterEditButtons;
    protected void HighlightSelectedEncounter()
    {
        for (int i = 0; i < encounterEditButtons.Count; i++)
        {
            encounterEditButtons[i].color = transColor;
        }
        if (currentlySelectedEncounter >= 0)
        {
            encounterEditButtons[currentlySelectedEncounter].color = selectColor;
        }
    }
    protected void UpdateEncounterEditor()
    {
        HighlightSelectedEncounter();
    }
    public void StartChangingEncounters()
    {
        SwitchEditing(2);
    }
    public int currentlySelectedEncounter = -1;
    public int encounterToMove = -1;
    protected void UpdateEncounterToMoveOnMap()
    {
        if (currentlyEditing != 2){return;}
        if (encounterToMove < 0){return;}
        int indexOfSpawn = currentTiles.IndexOf(encounterToMove);
        if (indexOfSpawn >= 0)
        {
            terrainTiles[indexOfSpawn].Highlight(false);
        }
    }
    public void SelectEncounterType(int newType)
    {
        encounterToMove = -1;
        if (currentlySelectedEncounter == newType){currentlySelectedEncounter = -1;}
        else{currentlySelectedEncounter = newType;}
        HighlightSelectedEncounter();
    }
    // Need specific info on what happens, types of battles/encounters/etc.
    public List<string> currentEncounterSpecifics;
    public void StartChangingEncounterSpecifics()
    {
        SwitchEditing(3);
    }
    public GameObject encounterSpecificEditor;
    public int encounterSpecificToEdit = -1;
    protected void UpdateEncounterToEditOnMap()
    {
        if (currentlyEditing != 3){return;}
        if (encounterSpecificToEdit < 0){return;}
        int indexOfSpawn = currentTiles.IndexOf(encounterSpecificToEdit);
        if (indexOfSpawn >= 0)
        {
            terrainTiles[indexOfSpawn].Highlight(false);
        }
    }
    public string encounterSpecificsText;
    public void UpdateSpecificsText(string newString)
    {
        if (encounterSpecificToEdit < 0){return;}
        if (newString.Length <= 0)
        {
            encounterSpecificsText = encounterSpecificsText.Remove(encounterSpecificsText.Length - 1, 1);
        }
        else
        {
            encounterSpecificsText += newString;
        }
        currentEncounterSpecifics[encounterSpecificToEdit] = encounterSpecificsText;
        UpdateMap();
    }
    // Encounter can be one time or respawning.
    public List<string> currentEncounterRespawnFreq;
    // Need to know where to spawn the player when they first enter the level.
    public int spawnPoint = -1;
    public void StartChangingSpawnPoint()
    {
        SwitchEditing(0);
    }
    protected void UpdateSpawnPointOnMap()
    {
        int indexOfSpawn = currentTiles.IndexOf(spawnPoint);
        if (indexOfSpawn >= 0)
        {
            terrainTiles[indexOfSpawn].Highlight();
        }
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
        UpdateSpawnPointOnMap();
        UpdateEncounterToMoveOnMap();
        UpdateEncounterToEditOnMap();
    }

    protected override void UpdateTile(int imageIndex, int tileIndex)
    {
        base.UpdateTile(imageIndex, tileIndex);
        if (tileIndex < 0 || tileIndex >= (totalRows * totalColumns))
        {
            return;
        }
        if (tileIndex >= currentEncounters.Count && tileIndex < allTiles.Count)
        {
            for (int i = currentEncounters.Count; i < allTiles.Count; i++)
            {
                currentEncounters.Add("");
                currentEncounterSpecifics.Add("");
            }
        }
        terrainTiles[imageIndex].SetTileText(EncounterText(currentEncounters[tileIndex])+"\n"+currentEncounterSpecifics[tileIndex]);
    }

    protected string EncounterText(string enctype)
    {
        if (enctype.Length <= 0){return "";}
        int intType = int.Parse(enctype);
        switch (intType)
        {
            case 0:
                return "Battle";
            case 1:
                return "Switch Level";
            case 2:
                return "Switch Scene";
            case 3:
                return "Treasure";
        }
        return "";
    }

    public override void AdjustRows(bool increase = true)
    {
        base.AdjustRows(increase);
        UpdateMap();
    }

    protected override void AddRow()
    {
        for (int i = 0; i < totalColumns; i++)
        {
            allTiles.Add("0");
            currentEncounters.Add("");
            currentEncounterSpecifics.Add("");
        }
        totalRows++;
    }

    protected override void RemoveRow()
    {
        if (totalRows <= gridSize){return;}
        // Remove the last row.
        for (int i = 0; i < totalColumns; i++)
        {
            allTiles.RemoveAt(allTiles.Count - 1);
            currentEncounters.RemoveAt(currentEncounters.Count - 1);
            currentEncounterSpecifics.RemoveAt(currentEncounterSpecifics.Count - 1);
        }
        totalRows--;
    }

    protected override void AddColumns()
    {
        // Add four columns at once to ensure balance.
        int index = 0;
        List<string> tempEnc = new List<string>();
        List<string> tempEncSpec = new List<string>();
        for (int i = 0; i < totalRows; i++)
        {
            for (int j = 0; j < totalColumns+4; j++)
            {
                if (j >= totalColumns)
                {
                    tempTiles.Add("0");
                    tempEnc.Add("");
                    tempEncSpec.Add("");
                    continue;
                }
                tempTiles.Add(allTiles[index]);
                tempEnc.Add(currentEncounters[index]);
                tempEncSpec.Add(currentEncounterSpecifics[index]);
                index++;
            }
        }
        totalColumns += 4;
        allTiles = new List<string>(tempTiles);
        currentEncounters = new List<string>(tempEnc);
        currentEncounterSpecifics = new List<string>(tempEncSpec);
    }

    protected override void RemoveColumns()
    {
        if (totalColumns <= gridSize){return;}
        int index = 0;
        List<string> tempEnc = new List<string>();
        List<string> tempEncSpec = new List<string>();
        for (int i = 0; i < totalRows; i++)
        {
            for (int j = 0; j < totalColumns; j++)
            {
                if (j >= totalColumns - 4)
                {
                    index++;
                    continue;
                }
                tempTiles.Add(allTiles[index]);
                tempEnc.Add(currentEncounters[index]);
                tempEncSpec.Add(currentEncounterSpecifics[index]);
                index++;
            }
        }
        totalColumns -= 4;
        allTiles = new List<string>(tempTiles);
        currentEncounters = new List<string>(tempEnc);
        currentEncounterSpecifics = new List<string>(tempEncSpec);
    }

    public override void AdjustColumns(bool increase = true)
    {
        base.AdjustColumns(increase);
        UpdateMap();
    }

    public override void MoveMap(int direction)
    {
        base.MoveMap(direction);
        UpdateMap();
    }

    public void Publish()
    {
        SaveCurrentLevel();
        // It's a new battle.
        if (levelIndex < 0)
        {
            if (allDataList.Count <= 0){allData = currentLevelData;}
            else{allData += "#"+currentLevelData;}
            UpdateAllDataList();
            levelIndex = allDataList.Count - 1;
            UpdateIndexText();
        }
        // It's an edit of another battle.
        else
        {
            allDataList[levelIndex] = currentLevelData;
            allData = GameManager.instance.ConvertListToString(allDataList, "#");
        }
        File.WriteAllText(saveDataPath+"/Levels.txt", allData);
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
        levelIndex = -1;
        UpdateIndexText();
        UpdateCenterTile();
        UpdateMap();
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

    public override void ClickOnTile(int tileNumber)
    {
        if (currentTiles[tileNumber] < 0){return;}
        switch (currentlyEditing)
        {
            case 0:
                SetSpawnPoint(tileNumber);
                break;
            case 1:
                SetTerrain(tileNumber);
                break;
            case 2:
                SetEncounter(tileNumber);
                break;
            case 3:
                SetEncounterSpecifics(tileNumber);
                break;
            case 4:
                break;
        }
    }

    protected void SetSpawnPoint(int tileNumber)
    {
        spawnPoint = currentTiles[tileNumber];
        UpdateMap();
    }

    protected void SetTerrain(int tileNumber)
    {
        if (currentlySelectedTerrain < 0){return;}
        allTiles[currentTiles[tileNumber]] = currentlySelectedTerrain.ToString();
        UpdateMap();
    }

    protected void SetEncounter(int tileNumber)
    {
        if (currentlySelectedEncounter >= 0)
        {
            currentEncounters[currentTiles[tileNumber]] = currentlySelectedEncounter.ToString();
        }
        else if (currentlySelectedEncounter < 0 && encounterToMove < 0)
        {
            encounterToMove = currentTiles[tileNumber];
        }
        else if (currentlySelectedEncounter < 0 && encounterToMove > 0)
        {
            if (encounterToMove == currentTiles[tileNumber])
            {
                currentEncounters[currentTiles[tileNumber]] = "";
                encounterToMove = -1;
            }
            else
            {
                SwitchEncounters(currentTiles[tileNumber], encounterToMove);
            }
        }
        UpdateMap();
    }

    protected void SwitchEncounters(int tileOne, int tileTwo)
    {
        string tempEnc = currentEncounters[tileOne];
        string tempEncSpec = currentEncounterSpecifics[tileOne];
        currentEncounters[tileOne] = currentEncounters[tileTwo];
        currentEncounterSpecifics[tileOne] = currentEncounterSpecifics[tileTwo];
        currentEncounters[tileTwo] = tempEnc;
        currentEncounterSpecifics[tileTwo] = tempEncSpec;
    }

    protected void SetEncounterSpecifics(int tileNumber)
    {
        int tileIndex = currentTiles[tileNumber];
        if (encounterSpecificToEdit != tileIndex)
        {
            encounterSpecificToEdit = tileIndex;
            encounterSpecificsText = currentEncounterSpecifics[tileIndex];
        }
        else if (encounterSpecificToEdit == tileIndex)
        {
            encounterSpecificToEdit = -1;
            encounterSpecificsText = "";
        }
        UpdateMap();
    }


}
