using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentContainer : AllStats
{
    public Equipment weaponSlot;
    public Equipment armorSlot;
    public Equipment helmetSlot;
    public Equipment bootsSlot;
    public Equipment accessorySlot;
    public List<string> allPassives;
    public List<string> allActives;

    public void UpdateStats()
    {
        baseHealth = armorSlot.baseHealth+helmetSlot.baseHealth;
        baseAttack = weaponSlot.baseAttack;
        baseDefense = armorSlot.baseDefense;
        baseEnergy = 0;
        baseMovement = bootsSlot.baseMovement+armorSlot.baseMovement;
        attackRange = weaponSlot.attackRange;
        baseActions = 0;
        moveType = bootsSlot.moveType;
        size = armorSlot.size;
        baseInitiative = bootsSlot.baseInitiative+helmetSlot.baseInitiative;
    }
}
