using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dict", menuName = "ScriptableObjects/Dict", order = 1)]
public class ScriptableDictionary : ScriptableObject
{
    public List<int> intKeys;
    public List<string> keys;
    public List<string> values;

    public string ReturnValue(string key)
    {
        int indexOf = keys.IndexOf(key);
        if (indexOf >= 0){return values[indexOf];}
        return "";
    }

    public string ReturnValueByIndex(int index)
    {
        if (index >= 0 && index < values.Count){return values[index];}
        return "";
    }
}
