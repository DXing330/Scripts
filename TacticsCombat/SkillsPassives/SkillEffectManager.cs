using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffectManager : MonoBehaviour
{
    private int powerDenominator = 10;
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
                power *= user.baseHealth/10;
                // Limit healing.
                power = Mathf.Min(power, skill.basePower*powerDenominator);
                break;
            case "Move":
                power *= user.baseMovement;
                break;
            case "Battle":
                return true;
        }
        power /= powerDenominator;
        skill.AffectActor(target, power);
        return false;
    }
}
