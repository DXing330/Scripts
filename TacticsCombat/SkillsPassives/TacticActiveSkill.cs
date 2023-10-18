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
    // Ally/enemy/all/etc.
    public int skillTarget;
    // Energy cost.
    public int cost;
    // Damage/heal/status/buff/etc.
    public string effect;
    public string effectSpecifics;
    // Based on caster?
    public int basePower;
    public int actionCost;
    public string flavorText;

    // Definitely don't need two of these.
    public void AffectActor(TacticActor actor, int power = 0)
    {
        switch (effect)
        {
            case "Damage":
                actor.ReceiveDamage(power);
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
}
