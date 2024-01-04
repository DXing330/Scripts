using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HexTerrainTile : TerrainTile
{
    public float XYRatio = 2f;
    public TerrainMap terrainMap;
    public Map otherMap;
    public TMP_Text tileNumberText;

    public override void UpdateColor(int type)
    {
        Color tempColor = Color.white;
        tempColor.a = 1f;
        tileImage.color = tempColor;
        if (type < 0)
        {
            tempColor = Color.black;
            tempColor.a = 1f;
            tileImage.color = tempColor;
            return;
        }
    }

    public override void SetTileNumber(int newNumber)
    {
        tileNumber = newNumber;
        if (tileNumberText == null){return;}
        tileNumberText.text = newNumber.ToString();
    }

    public void SetTileText(string newText)
    {
        if (tileNumberText == null){return;}
        tileNumberText.text = newText;
    }

    public override void UpdateSize(float newSize)
    {
        rectTransform.sizeDelta = new Vector2(newSize * XYRatio, newSize);
    }

    public void ClickOnTile()
    {
        // Send the tileNumber to the terrain map.
        if (tileNumber < 0){return;}
        if (terrainMap == null)
        {
            otherMap.ClickOnTile(tileNumber);
        }
        else
        {
            terrainMap.ClickOnTile(tileNumber);
        }
    }
}
