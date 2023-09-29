using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    // 0 = weapon, 1 = armor, 2 = helmet, 3 = boots, 4 = accessory
    public int equipType;
    public int ID;
    public int bonusHealth = 0;
    public int bonusAttack = 0;
    public int bonusDefense = 0;
    public int bonusMovement = 0;
    public int bonusEnergy = 0;
    public string passiveEffect = "";
    public string negativePassive = "";
}
