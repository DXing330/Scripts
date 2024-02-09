using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffectManager : MonoBehaviour
{
    public TerrainMap terrainMap;
    private int powerDenominator = 10;
    public bool ApplySkillEffect(TacticActor target, TacticActiveSkill skill, TacticActor user, string individualEffect = "")
    {
        string effect = skill.effect;
        if (individualEffect.Length > 0){effect = individualEffect;}
        // Determine skill power.
        int power = Mathf.Max(skill.basePower, skill.currentPower);
        switch (effect)
        {
            case "Damage":
                power *= user.attackDamage;
                break;
            case "Heal":
                power *= user.baseHealth/10;
                // Limit healing.
                power = Mathf.Min(power, Mathf.Max(skill.basePower, skill.currentPower)*powerDenominator);
                break;
            case "Move":
                power *= user.currentMovespeed;
                break;
            case "Support":
                power *= powerDenominator;
                break;
            case "Act":
                power *= powerDenominator;
                break;
            case "Active":
                // Add the skill to the temp active lists?
                return false;
            case "TempPassive":
                target.GainTempPassive(skill.effectSpecifics);
                return false;
            case "Passive":
                // Add the skill to the temp passive lists?
                return false;
            case "Battle":
                return true;
            case "Summon":
                return true;
            case "Displace":
                return true;
            case "Teleport":
                return true;
            case "Tame":
                return true;
            case "Battle+Status":
                skill.AddBuffDebuff(target, skill.effectSpecifics, power);
                return true;
        }
        if (skill.effect.StartsWith("Battle")){return true;}
        power /= powerDenominator;
        skill.AffectActor(target, power, effect);
        return false;
    }

    public void ApplySpecialSkillEffect(string specialEffect, TacticActor target, TacticActiveSkill skill, TacticActor user)
    {
        switch (specialEffect)
        {
            case "Tame":
                if (TryToTame(target, skill, user))
                {
                    Tame(target);
                }
                break;
        }
    }

    protected bool TryToTame(TacticActor tameTarget, TacticActiveSkill skill, TacticActor tamer)
    {
        // Check if you're using the right skill.
        if (!tameTarget.species.Contains(skill.effectSpecifics)){return false;}
        // int catchRate = some number, adjust based on conditions, compare to some number.
        // Check if you're winning.
        if (tameTarget.health >= tamer.health){return false;}
        // Check if you're strong enough.
        //if (tameTarget.attackDamage > tamer.attackDamage){return false;}
        // Do a roll to see if it works.
        // Roll function.
        return true;
    }

    protected void Tame(TacticActor tameTarget)
    {
        // Remove them from the battle.
        tameTarget.health = 0;
        // Add then to your party for later use.
        GameManager.instance.armyData.GainFighter(tameTarget.typeName);
    }
}
