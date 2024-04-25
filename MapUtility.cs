using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUtility : MonoBehaviour
{
    public int GetRow(int location, int columns)
    {
        int row = 0;
        while (location >= columns)
        {
            location -= columns;
            row++;
        }
        return row;
    }

    public int GetColumn(int location, int columns)
    {
        return location%columns;
    }

    public int GetLocationIndex(int row, int column, int totalColumns)
    {
        return ((row*totalColumns)+column);
    }
}
