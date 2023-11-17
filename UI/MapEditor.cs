using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditor : Map
{
    // This is the all the tiles.
    public List<string> mapToEdit;
    public string baseTerrain = "1";

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
    }

    public void LoadMap(List<string> loadedMap)
    {
        allTiles = loadedMap;
        fullSize = (int) Mathf.Sqrt(allTiles.Count);
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
}
