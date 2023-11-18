using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditor : Map
{
    // This is the all the tiles.
    public List<string> mapToEdit;
    public List<string> possibleTerrains;
    public int currentlySelectedTerrain = -1;
    public List<GameObject> possibleTerrainButtons;
    public List<TerrainTile> possibleTerrainTiles;
    public string baseTerrain = "1";
    public string[] baseTerrains;

    public void NewMap()
    {
        Debug.Log("New Map");
        if (fullSize < gridSize){fullSize = gridSize;}
        allTiles.Clear();
        for (int i = 0; i < fullSize * fullSize; i++)
        {
            allTiles.Add(baseTerrain);
        }
        UpdateCenterTile();
        UpdateMap();
        DeterminePossibleTerrains();
    }

    public void SaveMap()
    {
        mapToEdit = new List<string>(allTiles);
    }

    public void UndoEdits()
    {
        allTiles = new List<string>(mapToEdit);
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

    public void LoadMap(List<string> loadedMap, string terrain = "2")
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
        if (tileNumber < 0){startIndex = (fullSize*fullSize/2);}
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
                startIndex=fullSize*fullSize/2;
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
}
