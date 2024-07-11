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
    protected string saveDataPath;
    public TMP_Text indexText;
    public int mapIndex = -1;
    public int currentPage = 0;
    public List<string> mapToEdit;
    public List<string> possibleTerrains;
    public int currentlySelectedTerrain = -1;
    public List<GameObject> possibleTerrainButtons;
    public List<TerrainTile> possibleTerrainTiles;
    public string baseTerrain = "1";
    public string[] baseTerrains;

    protected virtual void UpdateIndexText()
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
        string[] loadedMapData = allMapsList[mapIndex].Split(",");
        if (loadedMapData.Length > 2)
        {
            LoadMap(loadedMapData[0].Split("|").ToList(), int.Parse(loadedMapData[1]), int.Parse(loadedMapData[2]));
        }
        else
        {
            LoadMap(loadedMapData[0].Split("|").ToList());
        }
        UpdateCenterTile();
        UpdateMap();
    }

    public void StopEditingMaps()
    {
        GameManager.instance.RefreshMaps();
    }

    protected override void Start()
    {
        UpdateBaseTerrains();
        DeterminePossibleTerrains();
        saveDataPath = Application.persistentDataPath;
        if (File.Exists(saveDataPath+"/Maps_"+baseTerrain+".txt"))
        {
            allMaps = File.ReadAllText(saveDataPath+"/Maps_"+baseTerrain+".txt");
        }
        UpdateAllMapsList();
        ChangeIndex();
        for (int i = 0; i < terrainTiles.Count; i++)
        {
            terrainTiles[i].SetTileNumber(i);
        }
    }

    protected void UpdateAllMapsList()
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
        string savedMapText = GameManager.instance.utility.ConvertListToString(mapToEdit)+","+totalRows+","+totalColumns;
        // If its a new map then add it to the rest.
        if (mapIndex < 0)
        {
            allMaps += "#"+savedMapText;
            UpdateAllMapsList();
            mapIndex = allMapsList.Count-1;
            UpdateIndexText();
        }
        // Otherwise just edit it.
        else
        {
            allMapsList[mapIndex] = savedMapText;
            allMaps = GameManager.instance.utility.ConvertListToString(allMapsList, "#");
        }
        File.WriteAllText(saveDataPath+"/Maps_"+baseTerrain+".txt", allMaps);
    }

    public void DeleteMap()
    {
        // Can't delete the last map.
        if (allMapsList.Count <= 1){return;}
        if (mapIndex < 0){return;}
        allMapsList.RemoveAt(mapIndex);
        allMaps = GameManager.instance.utility.ConvertListToString(allMapsList, "#");
        File.WriteAllText(saveDataPath+"/Maps_"+baseTerrain+".txt", allMaps);
        ChangeIndex();
    }

    public void NewMap()
    {
        mapIndex = -1;
        UpdateIndexText();
        if (fullSize < gridSize)
        {
            fullSize = gridSize;
            SetTotalRowsColumns();
        }
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
        if (mapToEdit.Count < terrainTiles.Count){return;}
        allTiles = new List<string>(mapToEdit);
        fullSize = (int) Mathf.Sqrt(allTiles.Count);
        SetTotalRowsColumns();
        UpdateCenterTile();
        UpdateMap();
    }

    protected void UpdateBaseTerrains()
    {
        baseTerrains = possibleTerrains[int.Parse(baseTerrain)].Split("|");
    }

    protected void DeterminePossibleTerrains()
    {
        int pageShift = (currentPage * possibleTerrainButtons.Count);
        for (int i = 0; i < possibleTerrainButtons.Count; i++)
        {
            if (i < baseTerrains.Length - pageShift)
            {
                possibleTerrainButtons[i].SetActive(true);
            }
            else
            {
                possibleTerrainButtons[i].SetActive(false);
            }
        }
        for (int j = 0; j < Mathf.Min(possibleTerrainTiles.Count, baseTerrains.Length - pageShift); j++)
        {
            int type = int.Parse(baseTerrains[j + pageShift]);
            possibleTerrainTiles[j].UpdateColor(type);
            possibleTerrainTiles[j].UpdateImage(tileSprites.allSprites[type]);
        }
    }

    public virtual void LoadMap(List<string> loadedMap, int rows = -1, int columns = -1, string terrain = "1")
    {
        baseTerrain = terrain;
        allTiles = loadedMap;
        totalRows = rows;
        totalColumns = columns;
        if (totalRows < gridSize){totalRows = gridSize;}
        if (totalColumns < gridSize){totalColumns = gridSize;}
        fullSize = (int) Mathf.Sqrt(allTiles.Count);
    }

    protected void UpdateHighlights()
    {
        for (int i = 0; i < Mathf.Min(possibleTerrainTiles.Count, baseTerrains.Length - (currentPage * possibleTerrainButtons.Count)); i++)
        {
            possibleTerrainTiles[i].ResetHighlight();
        }
        if (currentlySelectedTerrain >= 0)
        {
            possibleTerrainTiles[currentlySelectedTerrain%possibleTerrainButtons.Count].Highlight();
        }
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
    }

    public override void MoveMap(int direction)
    {
        base.MoveMap(direction);
        UpdateMap();
    }

    public override void ClickOnTile(int tileNumber)
    {
        if (currentlySelectedTerrain < 0){return;}
        if (currentTiles[tileNumber] < 0){return;}
        allTiles[currentTiles[tileNumber]] = baseTerrains[currentlySelectedTerrain];
        UpdateMap();
    }

    public void ChangeCurrentlySelected(int newType)
    {
        int type = newType + (currentPage * possibleTerrainButtons.Count);
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
        allTiles = new List<string>(tempTiles);
        UpdateCenterTile();
        UpdateMap();
    }

    protected void IncreaseMapSize()
    {
        if (fullSize + 1 >= gridSize*2){return;}
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
    }

    protected void DecreaseMapSize()
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
    }

    public override void AdjustRows(bool increase = true)
    {
        base.AdjustRows(increase);
        UpdateMap();
    }

    public override void AdjustColumns(bool increase = true)
    {
        base.AdjustColumns(increase);
        UpdateMap();
    }

    public void ChangePage(bool right = true)
    {
        if (!right)
        {
            if (currentPage > 0)
            {
                currentPage--;
            }
        }
        else
        {
            if ((currentPage+1)*possibleTerrainButtons.Count < baseTerrains.Length)
            {
                currentPage++;
            }   
        }
        DeterminePossibleTerrains();
    }
}
