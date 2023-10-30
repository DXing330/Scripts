using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public List<int> adjacentTiles;
    public List<int> tempAdjTiles;
    public int fullSize = 13;

    void Start()
    {
        RecurviseAdjacency(0, 2);
        Debug.Log(string.Join(", ", adjacentTiles.Select(x => x.ToString())));
    }

    private List<int> AdjacentFromIndex(int location)
    {
        tempAdjTiles.Clear();
        if (location%fullSize > 0)
        {
            tempAdjTiles.Add(location-1);
        }
        if (location%fullSize < fullSize - 1)
        {
            tempAdjTiles.Add(location+1);
        }
        if (location < (fullSize - 1) * fullSize)
        {
            tempAdjTiles.Add(location+fullSize);
        }
        if (location > fullSize - 1)
        {
            tempAdjTiles.Add(location-fullSize);
        }
        return tempAdjTiles;
    }

    public void RecurviseAdjacency(int location, int range = 1)
    {
        adjacentTiles.Clear();
        if (range <= 0)
        {
            return;
        }
        adjacentTiles = new List<int>(AdjacentFromIndex(location));
        if (range == 1)
        {
            return;
        }
        for (int i = 0; i < range; i++)
        {
            for (int j = 0; j < adjacentTiles.Count; j++)
            {
                AdjacentFromIndex(adjacentTiles[j]);
                adjacentTiles.AddRange(tempAdjTiles.Except(adjacentTiles));
                Debug.Log(string.Join(", ", adjacentTiles.Select(x => x.ToString())));
            }
        }
    }
}
