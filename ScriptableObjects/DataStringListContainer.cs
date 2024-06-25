using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StringList", menuName = "ScriptableObjects/DataStringListContainer", order = 1)]
public class DataStringListContainer : ScriptableObject
{
    public List<string> stringList;
}
