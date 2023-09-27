using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffectManager : MonoBehaviour
{
    public void ApplySkillEffect(TacticActor target, TacticActiveSkill skill, TacticActor user)
    {
        // Determine skill power.
        int power = skill.basePower;
        switch (skill.effect)
        {
            case "Damage":
                power += user.attackDamage;
                break;
            case "Heal":
                power *= user.level;
                break;
        }
        skill.AffectActor(target, power);
    }
}
