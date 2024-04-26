using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DataStringContainer", order = 1)]
public class DataStringContainer : ScriptableObject
{
    public string data;
    public string delimiter = "|";
}
