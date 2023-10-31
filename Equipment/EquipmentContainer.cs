using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentContainer : MonoBehaviour
{
    public Equipment weaponSlot;
    public Equipment armorSlot;
    public Equipment helmetSlot;
    public Equipment bootsSlot;
    public Equipment accessorySlot;
    public int totalBonusHealth;
    public int totalBonusAttack;
    public int totalBonusDefense;
    public int totalBonusEnergy;
    public int totalBonusMovement;
    public List<string> allPassives;
    public List<string> allActives;

    public void UpdateStats()
    {
        totalBonusHealth = armorSlot.bonusHealth+helmetSlot.bonusHealth;
        totalBonusAttack = weaponSlot.bonusAttack;
        totalBonusDefense = armorSlot.bonusDefense+helmetSlot.bonusDefense;
        totalBonusEnergy = bootsSlot.bonusEnergy;
        totalBonusMovement = bootsSlot.bonusMovement;
    }
}
