using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMaker : MonoBehaviour
{
    public List<int> terrain;
    private int rng;

    public List<int> GenerateTerrain(int type = 0, int size = 6)
    {
        terrain.Clear();
        for (int i = 0; i < size * size; i++)
        {
            terrain.Add(RandomizeTile(type));
        }
        return terrain;
    }

    protected int RandomizeTile(int type)
    {
        switch (type)
        {
            case 0:
                return RandomPlainsTile();
            case 1:
                return RandomForestTile();
            case 2:
                return RandomMountainTile();
            case 3:
                return RandomWaterTile();
            case 4:
                return RandomDesertTile();
        }
        return 0;
    }

    protected int RandomPlainsTile()
    {
        rng = Random.Range(0, 10);
        switch (rng)
        {
            case 0:
                return 0;
            case 1:
                return 0;
            case 2:
                return 0;
            case 3:
                return 1;
            case 4:
                return 1;
            case 5:
                return 0;
            case 6:
                return 0;
            case 7:
                return 0;
            case 8:
                return 1;
            case 9:
                return 3;
        }
        return 0;
    }

    protected int RandomForestTile()
    {
        rng = Random.Range(0, 10);
        switch (rng)
        {
            case 0:
                return 1;
            case 1:
                return 1;
            case 2:
                return 1;
            case 3:
                return 0;
            case 4:
                return 0;
            case 5:
                return 3;
            case 6:
                return 1;
            case 7:
                return 1;
            case 8:
                return 1;
            case 9:
                return 0;
        }
        return 1;
    }

    protected int RandomMountainTile()
    {
        rng = Random.Range(0, 10);
        switch (rng)
        {
            case 0:
                return 2;
            case 1:
                return 2;
            case 2:
                return 2;
            case 3:
                return 1;
            case 4:
                return 3;
            case 5:
                return 4;
            case 6:
                return 2;
            case 7:
                return 2;
            case 8:
                return 2;
            case 9:
                return 1;
        }
        return 2;
    }

    protected int RandomWaterTile()
    {
        rng = Random.Range(0, 6);
        switch (rng)
        {
            case 0:
                return 3;
            case 1:
                return 3;
            case 2:
                return 4;
            case 3:
                return 4;
            case 4:
                return 0;
            case 5:
                return 1;
            case 6:
                return 3;
            case 7:
                return 3;
            case 8:
                return 4;
            case 9:
                return 0;
        }
        return 3;
    }

    protected int RandomDesertTile()
    {
        rng = Random.Range(0, 6);
        switch (rng)
        {
            case 0:
                return 5;
            case 1:
                return 5;
            case 2:
                return 5;
            case 3:
                return 5;
            case 4:
                return 0;
            case 5:
                return 3;
        }
        return 4;
    }
}
