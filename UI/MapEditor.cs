using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapEditor : Map
{
    public string allMaps;
    public List<string> allMapsList;
    private string saveDataPath;
    public TMP_Text indexText;
    public int mapIndex = -1;
    public List<string> mapToEdit;
    public List<string> tempTiles;
    public List<string> possibleTerrains;
    public int currentlySelectedTerrain = -1;
    public List<GameObject> possibleTerrainButtons;
    public List<TerrainTile> possibleTerrainTiles;
    public string baseTerrain = "1";
    public string[] baseTerrains;

    private void UpdateIndexText()
    {
        if (mapIndex < 0)
        {
            indexText.text = "New Map";
        }
        else
        {
            indexText.text = "Map "+(mapIndex+1).ToString();
        }
    }

    public void ChangeIndex(bool right = true)
    {
        if (right)
        {
            if (mapIndex + 1 < allMapsList.Count)
            {
                mapIndex++;
            }
            else
            {
                mapIndex = 0;
            }
        }
        else
        {
            if (mapIndex > 0)
            {
                mapIndex--;
            }
            else
            {
                mapIndex = allMapsList.Count - 1;
            }
        }
        UpdateIndexText();
        LoadMap(allMapsList[mapIndex].Split("|").ToList());
        UpdateCenterTile();
        UpdateMap();
    }

    protected override void Start()
    {
        DeterminePossibleTerrains();
        saveDataPath = Application.persistentDataPath;
        allMaps = File.ReadAllText(saveDataPath+"/Maps_"+baseTerrain+".txt");
        UpdateAllMapsList();
        ChangeIndex();
    }

    private void UpdateAllMapsList()
    {
        allMapsList = allMaps.Split("#").ToList();
        for (int i = 0; i < allMapsList.Count; i++)
        {
            if (allMapsList[i].Length < gridSize)
            {
                allMapsList.RemoveAt(i);
            }
        }
    }

    public void PublishMap()
    {
        if (File.Exists(saveDataPath+"/Maps_"+baseTerrain+".txt"))
        {
            allMaps = File.ReadAllText(saveDataPath+"/Maps_"+baseTerrain+".txt");
        }
        else
        {
            allMaps = "";
        }
        SaveMap();
        // If its a new map then add it to the rest.
        if (mapIndex < 0)
        {
            allMaps += "#"+GameManager.instance.ConvertListToString(mapToEdit);
            UpdateAllMapsList();
            mapIndex = allMapsList.Count-1;
            UpdateIndexText();
        }
        // Otherwise just edit it.
        else
        {
            allMapsList[mapIndex] = GameManager.instance.ConvertListToString(mapToEdit);
            allMaps = GameManager.instance.ConvertListToString(allMapsList, "#");
        }
        File.WriteAllText(saveDataPath+"/Maps_"+baseTerrain+".txt", allMaps);
    }

    public void NewMap()
    {
        mapIndex = -1;
        UpdateIndexText();
        if (fullSize < gridSize){fullSize = gridSize;}
        allTiles.Clear();
        for (int i = 0; i < fullSize * fullSize; i++)
        {
            allTiles.Add(baseTerrain);
        }
        UpdateCenterTile();
        UpdateMap();
    }

    public void SaveMap()
    {
        mapToEdit = new List<string>(allTiles);
    }

    public void UndoEdits()
    {
        allTiles = new List<string>(mapToEdit);
        fullSize = (int) Mathf.Sqrt(allTiles.Count);
        UpdateCenterTile();
        UpdateMap();
    }

    private void DeterminePossibleTerrains()
    {
        baseTerrains = possibleTerrains[int.Parse(baseTerrain)].Split("|");
        for (int i = 0; i < possibleTerrainButtons.Count; i++)
        {
            if (i < baseTerrains.Length)
            {
                possibleTerrainButtons[i].SetActive(true);
            }
            else
            {
                possibleTerrainButtons[i].SetActive(false);
            }
        }
        for (int j = 0; j < baseTerrains.Length; j++)
        {
            int type = int.Parse(baseTerrains[j]);
            possibleTerrainTiles[j].UpdateColor(type);
            possibleTerrainTiles[j].UpdateImage(tileSprites[type]);
        }
    }

    public void LoadMap(List<string> loadedMap, string terrain = "1")
    {
        baseTerrain = terrain;
        allTiles = loadedMap;
        fullSize = (int) Mathf.Sqrt(allTiles.Count);
    }

    private void UpdateHighlights()
    {
        for (int i = 0; i < baseTerrains.Length; i++)
        {
            possibleTerrainTiles[i].ResetHighlight();
        }
        if (currentlySelectedTerrain >= 0)
        {
            possibleTerrainTiles[currentlySelectedTerrain].Highlight();
        }
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
    }

    private void UpdateCenterTile(int tileNumber = -1)
    {
        if (tileNumber < 0)
        {
            if (fullSize%2 == 1)
            {
                startIndex = (fullSize*fullSize/2);
            }
            else
            {
                startIndex = (fullSize/2)+(fullSize*fullSize/2);
            }
        }
        else{startIndex = tileNumber;}
        DetermineCornerRowColumn();
        DetermineCurrentTiles();
    }

    public void MoveMap(int direction)
    {
        int previousIndex = startIndex;
        switch (direction)
        {
            case -1:
                UpdateCenterTile();
                break;
            case 0:
                if (previousIndex < fullSize)
                {
                    break;
                }
                startIndex-=fullSize;
                break;
            case 1:
                if (previousIndex%fullSize==fullSize-1)
                {
                    break;
                }
                startIndex++;
                break;
            case 2:
                if (previousIndex>(fullSize*(fullSize-1))-1)
                {
                    break;
                }
                startIndex+=fullSize;
                break;
            case 3:
                if (previousIndex%fullSize==0)
                {
                    break;
                }
                startIndex--;
                break;
            case 4:
                MoveMap(0);
                MoveMap(1);
                break;
            case 5:
                MoveMap(2);
                MoveMap(1);
                break;
            case 6:
                MoveMap(2);
                MoveMap(3);
                break;
            case 7:
                MoveMap(0);
                MoveMap(3);
                break;
        }
        DetermineCornerRowColumn();
        DetermineCurrentTiles();
        UpdateMap();
    }

    public void ClickOnTile(int tileNumber)
    {
        if (currentlySelectedTerrain < 0){return;}
        allTiles[currentTiles[tileNumber]] = baseTerrains[currentlySelectedTerrain];
        UpdateMap();
    }

    public void ChangeCurrentlySelected(int type)
    {
        if (type >= baseTerrains.Length){return;}
        if (type == currentlySelectedTerrain){currentlySelectedTerrain = -1;}
        else{currentlySelectedTerrain = type;}
        UpdateHighlights();
    }

    public void AdjustSize(bool increase = true)
    {
        tempTiles.Clear();
        if (increase)
        {
            IncreaseMapSize();
        }
        else
        {
            DecreaseMapSize();
        }
    }

    private void IncreaseMapSize()
    {
        if (fullSize >= gridSize*2){return;}
        // Copy the list and add a row and column.
        int index = 0;
        for (int i = 0; i < fullSize + 1; i++)
        {
            for (int j = 0; j < fullSize + 1; j++)
            {
                if (i == fullSize || j == fullSize)
                {
                    tempTiles.Add(baseTerrain);
                    continue;
                }
                tempTiles.Add(allTiles[index]);
                index++;
            }
        }
        fullSize++;
        allTiles = new List<string>(tempTiles);
        UpdateCenterTile();
        UpdateMap();
    }

    private void DecreaseMapSize()
    {
        if (fullSize <= gridSize){return;}
        // Copy the list and remove a row and column;
        int index = 0;
        for (int i = 0; i < fullSize - 1; i++)
        {
            for (int j = 0; j < fullSize; j++)
            {
                if (j == fullSize - 1)
                {
                    index++;
                    continue;
                }
                tempTiles.Add(allTiles[index]);
                index++;
            }
        }
        fullSize--;
        allTiles = new List<string>(tempTiles);
        UpdateCenterTile();
        UpdateMap();
    }
}
