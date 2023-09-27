using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerrainTile : MonoBehaviour
{
    public int cType;
    //plains=0,forest=1,mountain=2,water=3,deepWater=4,desert=5
    public RectTransform rectTransform;
    public Image tileImage;
    public Image actorImage;
    public Image highlight;
    // Show the border when you're trying to select a tile.
    public Image AOEHighlight;

    public void SetType(int newType)
    {
        cType = newType;
    }

    public void UpdateSize(float newSize)
    {
        rectTransform.localScale = new Vector3(newSize, newSize, 0);
    }

    public void UpdatePivot(float xPivot, float yPivot)
    {
        rectTransform.pivot = new Vector2(xPivot, yPivot);
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
        tempColor.a = 0.66f;
        if (!cyan)
        {
            tempColor = Color.red;
            tempColor.a = 0.5f;
        }
        highlight.color = tempColor;
    }

    public void ResetAOEHighlight()
    {
        Color tempColor = Color.white;
        tempColor.a = 0f;
        AOEHighlight.color = tempColor;
    }

    public void AoeHighlight(bool red = true)
    {
        Color tempColor = Color.red;
        if (!red)
        {
            tempColor = Color.green;
        }
        tempColor.a = 0.9f;
        AOEHighlight.color = tempColor;
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
                tempColor = Color.blue;
                tempColor.a = 0.8f;
                break;
            case 5:
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
                return 2;
            case 4:
                return 4;
            case 5:
                return 1;
        }
        return 999;
    }

    // Maybe make it so fliers can share tiles with other units.
    public int ReturnFlyingMoveCost(int type, int occupied = 0)
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
                return 1;
            case 2:
                return 1;
            case 3:
                return 1;
            case 4:
                return 1;
            case 5:
                return 1;
        }
        return 1;
    }

    public int ReturnRidingMoveCost(int type, int occupied = 0)
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
                return 3;
            case 2:
                return 4;
            case 3:
                return 2;
            case 4:
                return 4;
            case 5:
                return 1;
        }
        return 1;
    }

    public int ReturnSwimmingMoveCost(int type, int occupied = 0)
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
                return 4;
            case 3:
                return 1;
            case 4:
                return 1;
            case 5:
                return 4;
        }
        return 1;
    }

    public int ReturnScoutingMoveCost(int type, int occupied = 0)
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
                return 1;
            case 2:
                return 2;
            case 3:
                return 3;
            case 4:
                return 4;
            case 5:
                return 1;
        }
        return 1;
    }

    public int TerrainDefenseBonus(int type)
    {
        // Divide the bonus by six and multiply it by defense to get the final defender defense.
        switch (type)
        {
            case 0:
                return 6;
            case 1:
                return 7;
            case 2:
                return 8;
            case 3:
                return 6;
            case 4:
                return 6;
            case 5:
                return 6;
        }
        return 6;
    }
}
