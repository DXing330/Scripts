using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticTileList : MonoBehaviour
{
    public List<TerrainTile> tacticTiles;

    public TerrainTile GetTacticTile(int index)
    {
        return tacticTiles[index];
    }
}
