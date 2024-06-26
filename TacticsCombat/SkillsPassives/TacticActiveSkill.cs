using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticActiveSkill : MonoBehaviour
{
    public string skillName;
    // 0 = free range move as you wish, 1 = can only move between possible targets if there are none then the skill fails/can't be used
    public int lockOn = 0;
    // How far the center can be from the caster.
    public int range;
    // 0/1/2 implies total squares covered =
    // 1/5/13/etc. always diamond shaped
    public int span = 0;
    public string targetingShape = "none";
    // Ally/enemy/all/etc.
    public int skillTarget;
    // Energy cost.
    public int cost;
    // Damage/heal/status/buff/etc.
    public string effect;
    public string effectSpecifics;
    // Based on caster?
    public int basePower;
    public int currentPower;
    public string actionCost;
    public string flavorText;
    public List<string> ReturnStatList()
    {
        List<string> displayedStats = new List<string>();
        // Don't need name since's it on the side.
        displayedStats.Add(effect);
        displayedStats.Add(effectSpecifics);
        displayedStats.Add(lockOn.ToString());
        displayedStats.Add(skillTarget.ToString());
        displayedStats.Add(basePower.ToString());
        //displayedStats.Add(ReturnTargettingString(skillTarget));
        displayedStats.Add(cost.ToString());
        displayedStats.Add(actionCost);
        return displayedStats;
    }

    public List<string> ReturnRangeStats()
    {
        List<string> rangeStats = new List<string>();
        rangeStats.Add(range.ToString());
        rangeStats.Add(span.ToString());
        rangeStats.Add(targetingShape);
        return rangeStats;
    }

    protected string ReturnTargettingString()
    {
        int target = skillTarget;
        if (lockOn == 0){target *= -1;}
        switch (target)
        {
            case 0:
                if (lockOn == 0){return "enemies";}
                return "an enemy";
            case 1:
                return "an ally";
            case 2:
                return "all units";
            case 3:
                return "yourself";
            case 4:
                return "None";
            case 5:
                return "another unit";
            case 6:
                return "a Soul-Linked unit";
            case -1:
                return "allies";
            case -2:
                return "all units";
            case -5:
                return "other units";
            case -6:
                return "Soul-Linked units";
        }
        return "";
    }

    // Definitely don't need two of these.
    public void AffectActor(TacticActor actor, int power = 0, string newEffect = "")
    {
        string specificEffect = effect;
        if (newEffect.Length > 0){specificEffect = newEffect;}
        switch (specificEffect)
        {
            case "Damage":
                int damage = Random.Range(power/2, power*3/2);
                actor.ReceiveDamage(damage);
                break;
            case "Heal":
                actor.RegainHealth(power);
                break;
            case "Move":
                actor.movement += power;
                break;
            case "Support":
                AddBuffDebuff(actor, effectSpecifics, power);
                break;
            case "Act":
                actor.actionsLeft += power;
                break;
        }
    }

    public void AddBuffDebuff(TacticActor actor, string type, int duration)
    {
        // No duplicate buffs/debuffs
        int indexOf = actor.buffDebuffNames.IndexOf(type);
        if (indexOf < 0)
        {
            actor.buffDebuffNames.Add(type);
            actor.buffDebuffsDurations.Add(duration);
            actor.ApplyNewlyAddedBuffDebuffEffect();
        }
        else
        {
            actor.buffDebuffsDurations[indexOf] = Mathf.Max(duration, actor.buffDebuffsDurations[indexOf]);
        }
    }

    public int ReturnActionCost(TacticActor user)
    {
        if (actionCost == "Attack")
        {
            return user.actionsToAttack;
        }
        // Can make it consume all actions and increase power based on actions consumed.
        return int.Parse(actionCost);
    }

    public string ReturnActionCostString()
    {
        string cost = "";
        if (actionCost == "Attack")
        {
            cost = "o";
        }
        else
        {
            int costInt = int.Parse(actionCost);
            for (int i = 0; i < costInt; i++)
            {
                cost += "o";
            }
        }
        return cost;
    }

    public int ReturnActionCost()
    {
        if (actionCost == "Attack")
        {
            return 1;
        }
        else
        {
            return int.Parse(actionCost);
        }
    }

    public string ReturnEffectDescription(int part = 0)
    {
        string[] effects = effect.Split("+");
        string currentEffect = effects[part];
        string description = "";
        switch (currentEffect)
        {
            case ("Support"):
                return SupportEffectDescription();
            case ("Battle"):
                return BattleEffectDescription();
            case ("Damage"):
                return BattleEffectDescription();
            case ("Heal"):
                return HealEffectDescription();
            case ("Displace"):
                return DisplaceEffectDescription();
            case ("Move"):
                return MoveEffectDescription();
            case ("TempPassive"):
                return TempPassiveDescription();
            case ("TerrainChange"):
                return TerrainChangeDescription();
            case ("Act"):
                return ActEffectDescription();
        }
        return description;
    }

    private string SupportEffectDescription()
    {
        string description = "Applies "+effectSpecifics+" to "+ReturnTargettingString()+" for "+basePower.ToString()+" turns.";
        return description;
    }

    private string BattleEffectDescription()
    {
        string description = "Deals "+(basePower * 10).ToString()+"% damage to "+ReturnTargettingString()+".";
        return description;
    }

    private string HealEffectDescription()
    {
        string description = "Restores up to "+(basePower).ToString()+" HP for "+ReturnTargettingString()+".";
        return description;
    }

    private string DisplaceEffectDescription()
    {
        string description = effectSpecifics+" "+ReturnTargettingString()+".";
        return description;
    }

    private string TempPassiveDescription()
    {
        string description = "Grant "+effectSpecifics+" to "+ReturnTargettingString()+" for 1 turn.";
        return description;
    }

    private string TerrainChangeDescription()
    {
        string description = "";
        switch (effectSpecifics)
        {
            case "0":
                return "Creates fires on ignitable terrain.";
        }
        return description;
    }

    private string MoveEffectDescription()
    {
        string description = "Move "+((basePower * 10) - 100).ToString()+"% faster.";
        return description;
    }

    private string ActEffectDescription()
    {
        string description = "Gain "+(basePower - 1).ToString()+" extra actions.";
        return description;
    }
}
