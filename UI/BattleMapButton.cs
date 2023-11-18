using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMapButton : MonoBehaviour
{
    public TerrainMap terrainMap;
    public OverworldMap overworldMap;
    public MapEditor mapEditor;
    public RectTransform rectTransform;
    public int gridSize = 9;
    // Need to make this not hard coded.
    // Based on screen dimensions.
    public float xScale;
    public float yScale;
    public float xPivot;
    public float yPivot;
    public float xMinAnchor;
    public float xMaxAnchor;
    public float yMinAnchor;
    public float yMaxAnchor;
    public int minXPos;
    public int maxXPos;
    public int minYPos;
    public int maxYPos;
    private int Xrange;
    private int columnWidth;
    private int Yrange;
    private int rowWidth;

    void Start()
    {
        DetermineMinMaxPos();
        Xrange = maxXPos - minXPos;
        columnWidth = Xrange/gridSize;
        Yrange = maxYPos - minYPos;
        rowWidth = Yrange/gridSize;
    }

    private void DetermineMinMaxPos()
    {
        xPivot = rectTransform.pivot.x;
        yPivot = rectTransform.pivot.y;
        xScale = rectTransform.localScale.x;
        yScale = rectTransform.localScale.y;
        xMinAnchor = rectTransform.anchorMin.x;
        yMinAnchor = rectTransform.anchorMin.y;
        xMaxAnchor = rectTransform.anchorMax.x;
        yMaxAnchor = rectTransform.anchorMax.y;
        float width = Screen.width;
        float height = Screen.height;
        int xCenter = (int) (width * xPivot);
        int yCenter = (int) (height * yPivot);
        minXPos = xCenter - (int) ((xPivot - xMinAnchor) * width * xScale);
        maxXPos = (int) (width * xScale) + minXPos;
        minYPos = yCenter - (int) ((yPivot - yMinAnchor) * height * yScale);
        maxYPos = (int) (height * yScale) + minYPos;
    }

    public void MousePosEditor()
    {
        Vector3 mousePos = Input.mousePosition;
        int tileNumber = ClickedTile((int) mousePos.x, (int) mousePos.y);
        mapEditor.ClickOnTile(tileNumber);
    }

    public void MousePosWorldMap()
    {
        Vector3 mousePos = Input.mousePosition;
        int tileNumber = ClickedTile((int) mousePos.x, (int) mousePos.y);
        overworldMap.ClickOnTile(tileNumber);
    }

    public void MousePosOnClick()
    {
        if (Input.touches.Length > 1)
        {
            return;
        }
        Vector3 mousePos = Input.mousePosition;
        int tileNumber = ClickedTile((int) mousePos.x, (int) mousePos.y);
        terrainMap.ClickOnTile(tileNumber);
    }

    private int ClickedTile(int x, int y)
    {
        int row = DetermineRow(y);
        int column = DetermineColumn(x);
        return ((row * gridSize) + column);
    }

    private int DetermineRow(int y)
    {
        int invY = maxYPos - y;
        int row = 0;
        while (invY > rowWidth)
        {
            invY -= rowWidth;
            row++;
        }
        return row;
    }
    
    private int DetermineColumn(int x)
    {
        x -= minXPos;
        int column = 0;
        while (x > columnWidth)
        {
            x -= columnWidth;
            column++;
        }
        return column;
    }
}
