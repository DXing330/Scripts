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
        desert = 4
    };

    enum moveTypes
    {
        march = 0,
        fly = 1,
        ride = 2,
        swim = 3
    };
}
