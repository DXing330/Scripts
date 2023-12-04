using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticBuffsStatuses : MonoBehaviour
{
    // attack up/down, defense up/down, movement up/down, health up/down
    public string effect;
    public void LoadEffectName(string newEffect)
    {
        effect = newEffect;
    }

    // Don't do this in here, have a central one take in the effects and actors and do the thing there, don't want to make too many gameobjects.
    public void AffectActor(TacticActor actor)
    {
        switch (effect)
        {
            case "ATK+":
                actor.attackDamage += actor.baseAttack/5;
                break;
            case "ATK-":
                actor.attackDamage -= actor.baseAttack/5;
                break;
            case "DEF+":
                actor.defense += actor.baseDefense/5;
                break;
            case "DEF-":
                actor.defense -= actor.baseDefense/5;
                break;
            case "MOV+":
                actor.currentMovespeed++;
                break;
            case "MOV-":
                actor.currentMovespeed--;
                break;
            case "ENGY+":
                actor.GainEnergy(1);
                break;
            case "ENGY-":
                actor.LoseEnergy(1);
                break;
            case "DIZZY":
                actor.weight--;
                actor.attackDamage -= actor.baseAttack/5;
                break;
        }
    }
}
