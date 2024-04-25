using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPatternLocationFinder : MapUtility
{
    // Opposing patterns is just pattern#+2%4
    public int ReturnOscillatingFromMiddle(int number)
    {
        int sign = 1;
        if (number%2 == 0){sign = -1;}
        return (((number+1)/2)*(sign));
    }
    public int SingleSideSpawnPattern(int rows, int columns, int number, int pattern = 0)
    {
        int row = 0;
        int column = 0;
        int adjustment = ReturnOscillatingFromMiddle(number);
        switch (pattern)
        {
            // Enemies coming from right.
            case 0:
                column = columns - 1;
                row = (rows/2) + adjustment;
                break;
            // Enemies coming from left.
            case 2:
                row = (rows/2) + adjustment;
                break;
            // Enemies coming from above.
            case 1:
                column = (columns/2) + adjustment;
                break;
            // Enemies coming from below.
            case 3:
                row = rows - 1;
                column = (columns/2) + adjustment;
                break;
        }
        return GetLocationIndex(row,column,columns);
    }

    public int SingleSideInnerSpawn(int rows, int columns, int number, int pattern = 0)
    {
        int row = 1;
        int column = 1;
        int adjustment = ReturnOscillatingFromMiddle(number);
        switch (pattern)
        {
            // Allies right.
            case 0:
                column = columns - 2;
                row = (rows/2) + adjustment;
                break;
            // Allies left.
            case 2:
                row = (rows/2) + adjustment;
                break;
            // Allies above.
            case 1:
                column = (columns/2) + adjustment;
                break;
            // Allies below.
            case 3:
                row = rows - 2;
                column = (columns/2) + adjustment;
                break;
        }
        return GetLocationIndex(row,column,columns);
    }
}
