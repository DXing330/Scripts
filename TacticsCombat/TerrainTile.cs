using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerrainTile : MonoBehaviour
{
    public int tileNumber = -1;
    public virtual void SetTileNumber(int newNumber)
    {
        tileNumber = newNumber;
    }
    public virtual void SetTileText(string newText)
    {
        return;
    }
    public Color transColor;
    void Start()
    {
        transColor.a = 0f;
    }
    public int cType;
    //plains=0,forest=1,mountain=2,water=3,deepWater=4,desert=5
    public RectTransform rectTransform;
    public Image backgroundColor;
    // The terrain.
    public Image tileImage;
    // Any special features.
    public Image locationImage;
    // Any actors.
    public Image actorImage;
    public Image highlight;
    // Show the border when you're trying to select a tile.
    public Image AOEHighlight;
    public List<Image> directionalArrows;
    public int currentDirectionArrow = -1;

    public void SetType(int newType)
    {
        cType = newType;
    }

    public virtual void UpdateSize(float newSize)
    {
        rectTransform.localScale = new Vector3(newSize, newSize, 0);
    }

    public void UpdatePivot(float xPivot, float yPivot)
    {
        rectTransform.pivot = new Vector2(xPivot, yPivot);
    }

    public void ResetTileImage()
    {
        tileImage.sprite = null;
    }

    public void UpdateTileImage(Sprite newTile)
    {
        tileImage.sprite = newTile;
    }

    public void ResetLocationImage()
    {
        locationImage.sprite = null;
        locationImage.color = transColor;
    }

    public void UpdateLocationImage(Sprite newTile)
    {
        Color tempColor = Color.white;
        tempColor.a = 0.8f;
        locationImage.sprite = newTile;
        locationImage.color = tempColor;
    }

    public void ResetImage()
    {
        actorImage.sprite = null;
        actorImage.color = transColor;
    }

    public void UpdateImage(Sprite newActor)
    {
        Color tempColor = Color.white;
        tempColor.a = 1f;
        actorImage.sprite = newActor;
        actorImage.color = tempColor;
    }

    public void ResetHighlight()
    {
        highlight.color = transColor;
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
        AOEHighlight.color = transColor;
    }

    public void AoeHighlight(bool red = true)
    {
        Color tempColor = Color.red;
        if (!red)
        {
            tempColor = Color.green;
        }
        tempColor.a = 0.8f;
        AOEHighlight.color = tempColor;
    }

    public virtual void UpdateColor(int type)
    {
        Color tempColor = Color.white;
        tempColor.a = 0.3f;
        if (type < 0)
        {
            tempColor = Color.black;
            tempColor.a = 1f;
            backgroundColor.color = tempColor;
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
                tempColor.a = 0.3f;
                break;
            case 2:
                //tempColor = Color.grey;
                tempColor = Color.green;
                tempColor.a = 0.1f;
                break;
            case 3:
                tempColor = Color.blue;
                tempColor.a = 0.2f;
                break;
            case 4:
                tempColor = Color.blue;
                tempColor.a = 0.6f;
                break;
            case 5:
                tempColor = Color.yellow;
                tempColor.a = 0.3f;
                break;
            case 6:
                tempColor = Color.green;
                tempColor.a = 0.3f;
                break;
            case 7:
                tempColor = Color.green;
                tempColor.a = 0.3f;
                break;
        }
        backgroundColor.color = tempColor;
    }

    public void ResetDirectionalArrows()
    {
        if (currentDirectionArrow < 0){return;}
        directionalArrows[currentDirectionArrow].color = transColor;
        currentDirectionArrow = -1;
    }
    
    public void UpdateDirectionalArrow(int direction = -1)
    {
        if (direction >= 0 && direction < directionalArrows.Count)
        {
            currentDirectionArrow = direction;
            Color visibleColor = Color.white;
            visibleColor.a = 1f;
            directionalArrows[direction].color = visibleColor;
        }
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
            case 6:
                return 6;
        }
        return 999;
    }

    public int ReturnFlyingMoveCost(int type, int occupied = 0)
    {
        if (type < 0 || occupied > 0)
        {
            return 999;
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
                return 5;
            case 5:
                return 1;
            case 6:
                return 8;
        }
        return 999;
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
                return 3;
            case 1:
                return 4;
            case 2:
                return 5;
            case 3:
                return 1;
            case 4:
                return 1;
            case 5:
                return 6;
            case 6:
                return 8;
        }
        return 999;
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
                return 2;
            case 4:
                return 3;
            case 5:
                return 1;
            case 6:
                return 3;
        }
        return 999;
    }

    public int ReturnDefenseBonus(int tileType, int moveType)
    {
        switch (moveType)
        {
            case 0:
                return TerrainDefenseBonus(tileType);
            case 1:
                return FlyingDefenseBonus(tileType);
            case 2:
                return RidingDefenseBonus(tileType);
            case 3:
                return SwimmingDefenseBonus(tileType);
            case 4:
                return ScoutingDefenseBonus(tileType);
        }
        return 6;
    }

    private int TerrainDefenseBonus(int type)
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
                return 5;
            case 4:
                return 4;
            case 5:
                return 6;
        }
        return 6;
    }

    private int FlyingDefenseBonus(int type)
    {
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
                return 5;
        }
        return 6;
    }

    private int RidingDefenseBonus(int type)
    {
        switch (type)
        {
            case 0:
                return 6;
            case 1:
                return 6;
            case 2:
                return 7;
            case 3:
                return 4;
            case 4:
                return 3;
            case 5:
                return 6;
        }
        return 6;
    }

    private int SwimmingDefenseBonus(int type)
    {
        switch (type)
        {
            case 0:
                return 6;
            case 1:
                return 6;
            case 2:
                return 5;
            case 3:
                return 8;
            case 4:
                return 9;
            case 5:
                return 4;
        }
        return 6;
    }

    private int ScoutingDefenseBonus(int type)
    {
        switch (type)
        {
            case 0:
                return 6;
            case 1:
                return 8;
            case 2:
                return 9;
            case 3:
                return 6;
            case 4:
                return 5;
            case 5:
                return 6;
        }
        return 6;
    }
}
