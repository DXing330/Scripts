using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffectManager : MonoBehaviour
{
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
            case "Battle+Status":
                skill.AddBuffDebuff(target, skill.effectSpecifics, power);
                return true;
        }
        if (skill.effect.StartsWith("Battle")){return true;}
        power /= powerDenominator;
        skill.AffectActor(target, power, effect);
        return false;
    }
}
