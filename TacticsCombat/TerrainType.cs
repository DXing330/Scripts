using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainType : MonoBehaviour
{
    enum types 
    {
        plains = 0,
        forest = 1,
        mountain = 2,
        water = 3,
        deepWater = 4,
        desert = 5,
        rock = 6,
        chasm = 7
    };

    enum moveTypes
    {
        march = 0,
        fly = 1,
        ride = 2,
        swim = 3,
        scout = 4
    };
}
