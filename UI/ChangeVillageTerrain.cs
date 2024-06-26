using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeVillageTerrain : MonoBehaviour
{
    // Should cost mana and gold and time to change the terrain.
    void Start()
    {
        possibilityMatrix = fullPossibilityMatrix.Split("|").ToList();
        flavorMatrix = fullFlavorMatrix.Split("|").ToList();
    }
    public VillageDataManager villageData;
    public int selectedTile;
    public int baseTerrainType = -1;
    public int changedTerrainType = -1;
    public SpriteContainer terrainSprites;
    public string fullPossibilityMatrix;
    public List<string> possibilityMatrix;
    public List<string> possibleTerrainNames;
    public List<GameObject> possibleObjects;
    public List<FormationTile> possibleTerrains;
    public TMP_Text flavor;
    public string fullFlavorMatrix;
    public List<string> flavorMatrix;
    public List<string> possibleFlavors;
    protected int baseCost = 1;
    public TMP_Text cost;


    protected void UpdateHighlights(int selected = -1)
    {
        for (int i = 0; i < possibleTerrains.Count; i++)
        {
            possibleTerrains[i].ResetHighlight();
        }
        if (selected >= 0){possibleTerrains[selected].Highlight();}
    }

    protected void UpdatePossibleSprites()
    {
        for (int i = 0; i < possibleObjects.Count; i++)
        {
            possibleObjects[i].SetActive(false);
        }
        if (possibleTerrainNames.Count <= 0){return;}
        UpdateHighlights();
        for (int i = 0; i < possibleTerrainNames.Count; i++)
        {
            possibleObjects[i].SetActive(true);
            possibleTerrains[i].UpdateActorSprite(terrainSprites.SpriteDictionary(possibleTerrainNames[i]));
        }
    }

    public void SetTerrainType(int terrainType)
    {
        baseTerrainType = terrainType;
        flavor.text = "";
        changedTerrainType = -1;
        if (baseTerrainType < 0)
        {
            possibleTerrainNames.Clear();
            UpdatePossibleSprites();
            return;
        }
        possibleTerrainNames = possibilityMatrix[baseTerrainType].Split(",").ToList();
        possibleFlavors = flavorMatrix[baseTerrainType].Split(",").ToList();
        UpdatePossibleSprites();
    }

    protected int DetermineCost(int newType)
    {
        return baseCost;
    }

    public void ChangeTerrainType(int newType)
    {
        changedTerrainType = newType;
        flavor.text = possibleFlavors[newType];
        cost.text = DetermineCost(newType).ToString();
        UpdateHighlights(newType);
    }

    public int ChangedTerrainIndex()
    {
        if (changedTerrainType < 0){return -1;}
        return terrainSprites.IndexBySprite(possibleTerrainNames[changedTerrainType]);
    }
}
