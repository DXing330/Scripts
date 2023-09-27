using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListStringUtilities : MonoBehaviour
{
    public List<string> IntListToStringList(List<int> listToConvert)
    {
        return listToConvert.ConvertAll<string>(x => x.ToString());
    }

    public List<int> StringListToIntList(List<string> listToConvert)
    {
        return listToConvert.ConvertAll<int>(x => int.Parse(x));
    }

    public List<int> StringArrayToIntList(string[] stringArray)
    {
        return stringArray.ToList().ConvertAll<int>(x => int.Parse(x));
    }
}
