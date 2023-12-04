using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : AllStats
{
    // 0 = weapon, 1 = armor, 2 = helmet, 3 = boots, 4 = accessory
    public int equipType;
    public int ID;
    public string equipName;
    public List<string> possibleUsers;
    public string flavorText;
    public string passiveSkills = "";
    public string activeSkills = "";
}
