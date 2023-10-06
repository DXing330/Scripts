using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffectManager : MonoBehaviour
{
    private int powerDenominator;
    public bool ApplySkillEffect(TacticActor target, TacticActiveSkill skill, TacticActor user)
    {
        // Determine skill power.
        int power = skill.basePower;
        switch (skill.effect)
        {
            case "Damage":
                power *= user.attackDamage;
                break;
            case "Heal":
                power *= user.level;
                break;
            case "Battle":
                return true;
        }
        power /= powerDenominator;
        skill.AffectActor(target, power);
        return false;
    }
}
