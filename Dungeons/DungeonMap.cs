using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMap : DungeonGenTester
{
    protected bool interactable = true;
    public SpriteContainer dungeonSprites;
    public int stairsLocation;

    protected override void Start()
    {
        MakeDungeon();
        stairsLocation = int.Parse(dungeonData[2]);
        UpdateMap();
    }

    public override void MoveMap(int direction)
    {
        if (!interactable){return;}
        int nextTile = pathfinder.GetDestination(currentLocation, direction);
        if (allTiles[nextTile] == "1"){return;}
        currentLocation = nextTile;
        UpdateMap();
    }

    public override void UpdateMap()
    {
        UpdateCenterTile(currentLocation);
        for (int i = 0; i < terrainTiles.Count; i++)
        {
            terrainTiles[i].ResetAllImages();
            UpdateTile(i, currentTiles[i]);
            if (currentTiles[i] == currentLocation)
            {
                terrainTiles[i].UpdateImage(dungeonSprites.allSprites[0]);
            }
            else if (currentTiles[i] == stairsLocation)
            {
                terrainTiles[i].UpdateImage(dungeonSprites.allSprites[2]);
            }
            // Draw stairs.
            // Draw treasure.
        }
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

    protected void MoveIntoTile()
    {
        // Stairs, treasure, enemies, etc.
    }
}
