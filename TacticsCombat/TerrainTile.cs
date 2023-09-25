using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerrainTile : MonoBehaviour
{
    public int cType;
    public Image tileImage;
    public Image actorImage;
    public Image highlight;
    // Show the border when you're trying to select a tile.
    public Image border;

    public void SetType(int newType)
    {
        cType = newType;
    }

    public void ResetImage()
    {
        Color tempColor = Color.white;
        tempColor.a = 0f;
        actorImage.sprite = null;
        actorImage.color = tempColor;
    }

    public void UpdateImage(Sprite newActor)
    {
        Color tempColor = Color.white;
        tempColor.a = 0.6f;
        actorImage.sprite = newActor;
        actorImage.color = tempColor;
    }

    public void ResetHighlight()
    {
        Color tempColor = Color.white;
        tempColor.a = 0f;
        highlight.color = tempColor;
    }

    public void Highlight(bool cyan = true)
    {
        Color tempColor = Color.cyan;
        if (!cyan)
        {
            tempColor = Color.red;
        }
        tempColor.a = 0.66f;
        highlight.color = tempColor;
    }

    public void UpdateColor(int type)
    {
        Color tempColor = Color.white;
        tempColor.a = 0.5f;
        if (type < 0)
        {
            tempColor = Color.black;
            tempColor.a = 1f;
            tileImage.color = tempColor;
            return;
        }
        switch (type)
        {
            case 0:
                tempColor = Color.green;
                tempColor.a = 0.3f;
                break;
            case 1:
                tempColor = Color.green;
                tempColor.a = 0.8f;
                break;
            case 2:
                tempColor = Color.grey;
                break;
            case 3:
                tempColor = Color.blue;
                break;
            case 4:
                tempColor = Color.yellow;
                break;
        }
        tileImage.color = tempColor;
    }

    public int ReturnMoveCost(int type, int occupied = 0)
    {
        if (type < 0 || occupied > 0)
        {
            return 999;
        }
        switch (type)
        {
            case 0:
                return 1;
            case 1:
                return 2;
            case 2:
                return 3;
            case 3:
                return 3;
            case 4:
                return 1;
        }
        return 999;
    }

    // Maybe make it so fliers can share tiles with other units.
    public int ReturnFlyingMoveCost(int type, int occupied = 0)
    {
        switch (type)
        {
            case 0:
                return 1;
            case 1:
                return 1;
            case 2:
                return 1;
            case 3:
                return 1;
            case 4:
                return 1;
        }
        return 1;
    }
}
