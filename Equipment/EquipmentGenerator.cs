using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentGenerator : MonoBehaviour
{
    public int totalEquipTypes = 5;

    public string GenerateEquipment(int rarity)
    {
        string newEquip = "";
        // Decide what type of equipment to generate.
        return newEquip;
    }

    protected string GenerateWeapon()
    {
        // Need to get the stats for the specific equipment.
        string equip = "";
        return equip;
    }

    protected string GenerateAttack(int rarity)
    {
        string stat = "";
        return stat;
    }

    protected string GenerateHealth(int rarity)
    {
        string stat = "";
        return stat;
    }

    protected string GenerateDefense(int rarity)
    {
        string stat = "";
        return stat;
    }

    protected string GenerateMoveSpeed(int rarity)
    {
        string stat = "";
        return stat;
    }

    protected string GeneratePassives(int rarity)
    {
        string stat = "";
        return stat;
    }

    protected string GenerateActives(int rarity)
    {
        string stat = "";
        return stat;
    }
}