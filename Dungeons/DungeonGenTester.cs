using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenTester : Map
{
    protected override void Start()
    {
        MakeDungeon();
        UpdateMap();
    }
    public DungeonGenerator dungeonGenerator;
    public List<string> dungeonData = new List<string>();
    public int currentLocation;
    public int centeredSpot;

    [ContextMenu("Make Dungeon")]
    public void MakeDungeon()
    {
        dungeonData = dungeonGenerator.GenerateDungeon();
        allTiles = dungeonData[0].Split("|").ToList();
        currentLocation = int.Parse(dungeonData[1]);
        // Find the center.
        totalColumns = dungeonGenerator.GetMinSize();
        totalRows = dungeonGenerator.GetMinSize();
        pathfinder.SetTotalRowsColumns(totalRows, totalColumns);
        pathfinder.SetAllTiles(allTiles);
    }

    public void UpdateMap()
    {
        UpdateCenterTile(currentLocation);
        for (int i = 0; i < terrainTiles.Count; i++)
        {
            terrainTiles[i].ResetAllImages();
            UpdateTile(i, currentTiles[i]);
            if (currentTiles[i] == currentLocation)
            {
                terrainTiles[i].Highlight();
            }
        }
    }

    public override void MoveMap(int direction)
    {
        currentLocation = pathfinder.GetDestination(currentLocation, direction);
        UpdateMap();
    }

    protected override void UpdateCenterTile(int tileNumber = -1)
    {
        // Map starts centered around the player. The player can move some spaces without the map moving. The map moves if the player moves too far from the centered spot.
        int prevCol = GetColumn(startIndex);
        int currCol = GetColumn(tileNumber);
        int prevRow = GetRow(startIndex);
        int currRow = GetRow(tileNumber);
        bool left = true;
        if (Mathf.Abs(currCol - prevCol) > 1 || Mathf.Abs(currRow - prevRow) > 1)
        {
            startIndex = tileNumber;
            if (currCol > prevCol){left = false;}
        }
        DetermineCornerRowColumn();
        AdjustCorner(left);
        DetermineCurrentTiles();
    }

    protected void AdjustCorner(bool left = true)
    {
        if (Mathf.Abs(cornerColumn)%2 == 1)
        {
            if (left){cornerColumn--;}
            else {cornerColumn++;}
        }
    }

    protected override void DetermineCornerRowColumn()
    {
        int start = startIndex;
        int startRow = GetRow(start);
        int startColumn = GetColumn(start);
        cornerRow = startRow - (gridSize/2);
        cornerColumn = startColumn - (gridSize/2);
    }
}
