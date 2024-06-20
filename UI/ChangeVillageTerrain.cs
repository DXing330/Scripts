using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeVillageTerrain : MonoBehaviour
{
    // List<Tiles> possibleChanges
    public VillageDataManager villageData;
    public int selectedTile;
    protected int selectedTerrainType = -1;
    protected int selectedBuilding = -1;
    public List<GameObject> possibleObjects;
    public List<FormationTile> possibleTerrains;
}
