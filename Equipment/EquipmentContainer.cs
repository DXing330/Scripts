using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentContainer : AllStats
{
    // Two hands, armor set, accessory
    public int totalEquipmentTypes = 4;
    public List<string> allEquipment;
    public List<string> bonusActives;
    public List<string> bonusPassives;

    public string ReturnEquippedString()
    {
        return GameManager.instance.ConvertListToString(allEquipment,"@");
    }

    public bool Equipable(string equipment, int slot)
    {
        if (slot == 1 && allEquipment[0].Length > 6)
        {
            string[] data = allEquipment[0].Split("|");
            int trueSlot = int.Parse(data[6]);
            if (trueSlot < 0)
            {
                return false;
            }
        }
        return true;
    }
    
    public void Equip(string equipment, int slot)
    {
        if (slot < totalEquipmentTypes)
        {
            if (slot > 1)
            {
                allEquipment[slot] = equipment;
            }
            else
            {
                // Need to check if its the special case.
                string[] data = equipment.Split("|");
                int trueSlot = int.Parse(data[6]);
                if (trueSlot < 0)
                {
                    allEquipment[0] = equipment;
                }
                else
                {
                    allEquipment[slot] = equipment;
                }
            }
        }
    }

    public string Unequip(int slot)
    {
        string equip = allEquipment[slot];
        allEquipment[slot] = "";
        return equip;
    }

    public string SlotStats(int slot)
    {
        string equip = allEquipment[slot];
        return equip;
    }

    public void UnequipAll()
    {
        for (int i = 0; i < allEquipment.Count; i++)
        {
            allEquipment[i] = "";
        }
    }

    public void LoadEquipSet(string newSet)
    {
        List<string> newEquips = newSet.Split("@").ToList();
        if (newEquips.Count < allEquipment.Count){return;}
        for (int i = 0; i < allEquipment.Count; i++)
        {
            allEquipment[i] = newEquips[i];
        }
    }

    protected void GetEquipStats()
    {
        NullAllStats();
        bonusActives.Clear();
        bonusPassives.Clear();
        for (int i = 0; i < allEquipment.Count; i++)
        {
            if (allEquipment[i].Length < 6){continue;}
            ParseEquipmentString(allEquipment[i]);
        }
    }

    protected void ParseEquipmentString(string equip)
    {
        // Get the base stats in order.
        // Equipment stats go like this.
        // hlth|atk|def|move|actives|passives|slot|species|size|rarity|type|name
        // Not all equipment have names though.
        if (equip.Length <= 0){return;}
        List<string> bonusStats = equip.Split("|").ToList();
        baseHealth += int.Parse(bonusStats[0]);
        baseAttack += int.Parse(bonusStats[1]);
        baseDefense += int.Parse(bonusStats[2]);
        baseMovement += int.Parse(bonusStats[3]);
        bonusActives.AddRange(bonusStats[4].Split(",").ToList());
        bonusPassives.AddRange(bonusStats[5].Split(",").ToList());
    }

    public void UpdateActorStats(TacticActor actor)
    {
        GetEquipStats();
        actor.baseHealth += baseHealth;
        actor.baseAttack += baseAttack;
        actor.baseDefense += baseDefense;
        actor.baseMovement += baseMovement;
        //UpdateActives(actor);
        //UpdatePassives(actor);
    }

    public void UpdateActives(TacticActor actor)
    {
        if (bonusActives.Count > 0)
        {
            for (int i = 0; i < bonusActives.Count; i++)
            {
                if (bonusActives[i].Length <= 1){continue;}
                actor.activeSkillNames.Add(bonusActives[i]);
            }
        }
    }

    public void UpdatePassives(TacticActor actor)
    {
        if (bonusPassives.Count > 0)
        {
            for (int i = 0; i < bonusPassives.Count; i++)
            {
                if (bonusPassives[i].Length <= 1){continue;}
                actor.passiveSkillNames.Add(bonusPassives[i]);
            }
        }
    }

    public override List<int> ReturnStatList(bool main = true)
    {
        GetEquipStats();
        return base.ReturnStatList();
    }

    public List<string> ReturnEquipPassives()
    {
        return bonusPassives;
    }

    public List<string> ReturnEquipActives()
    {
        return bonusActives;
    }
}
