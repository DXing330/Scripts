using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMap : Map
{
    protected bool interactable = true;
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
        startIndex = currentLocation;
        // Find the center.
        totalColumns = dungeonGenerator.GetMinSize();
        totalRows = dungeonGenerator.GetMinSize();
        pathfinder.SetTotalRowsColumns(totalRows, totalColumns);
        pathfinder.SetAllTiles(allTiles);
        DetermineCornerRowColumn();
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

    protected override void UpdateTile(int imageIndex, int tileIndex)
    {
        // Undefined tiles are black as are unpassable tiles.
        if (tileIndex < 0 || tileIndex >= (totalRows * totalColumns) || allTiles[tileIndex] == "1")
        {
            terrainTiles[imageIndex].UpdateColor(-1);
        }
        else
        {
            int tileType = int.Parse(allTiles[tileIndex]);
            if (tileType >= 7)
            {
                terrainTiles[imageIndex].UpdateColor(0);
                terrainTiles[imageIndex].UpdateTileImage(tileSprites.allSprites[0]);
                terrainTiles[imageIndex].UpdateLocationImage(tileSprites.allSprites[tileType]);
                return;
            }
            terrainTiles[imageIndex].UpdateColor(tileType);
            terrainTiles[imageIndex].UpdateTileImage(tileSprites.allSprites[tileType]);
        }
    }

    public override void MoveMap(int direction)
    {
        if (!interactable){return;}
        int nextTile = pathfinder.GetDestination(currentLocation, direction);
        if (allTiles[nextTile] == "1"){return;}
        currentLocation = nextTile;
        UpdateMap();
    }

    protected override void UpdateCenterTile(int tileNumber = -1)
    {
        // Map starts centered around the player. The player can move some spaces without the map moving. The map moves if the player moves too far from the centered spot.
        int prevCol = GetColumn(startIndex);
        int currCol = GetColumn(tileNumber);
        bool left = true;
        // If the distance between the points is more than one then adjust the center.
        if (dungeonGenerator.utility.DistanceBetweenPoints(startIndex, tileNumber, totalRows) > 1)
        {
            startIndex = tileNumber;
            if (currCol > prevCol){left = false;}
            DetermineCornerRowColumn();
            AdjustCorner(left);
        }
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

    public override void ClickOnTile(int tileNumber)
    {
        if (!interactable){return;}
        if (currentLocation == currentTiles[tileNumber]){return;}
        if (currentTiles[tileNumber] < 0){return;}
        List<int> path = new List<int>(pathfinder.DestReachable(currentLocation, currentTiles[tileNumber], currentTiles, "1"));
        if (path.Count <= 0){return;}
        StartCoroutine(MoveAlongPath(path));
    }

    IEnumerator MoveAlongPath(List<int> path)
    {
        interactable = false;
        for (int i = path.Count - 1; i > -1; i--)
        {
            currentLocation = path[i];
            UpdateMap();
            yield return new WaitForSeconds(0.1f);
        }
        interactable = true;
    }
}
