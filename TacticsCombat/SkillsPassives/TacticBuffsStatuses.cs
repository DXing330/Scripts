using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticBuffsStatuses : MonoBehaviour
{
    // attack up/down, defense up/down, movement up/down, health up/down
    public string effect;
    public int duration;

    // Don't do this in here, have a central one take in the effects and actors and do the thing there, don't want to make too many gameobjects.
    public void AffectActor(TacticActor actor)
    {
        switch (effect)
        {
            case "ATK+":
                actor.attackDamage += actor.baseAttack/2;
                break;
            case "ATK-":
                actor.attackDamage -= actor.baseAttack/2;
                break;
            case "DEF+":
                actor.defense += actor.baseDefense/2;
                break;
            case "DEF-":
                actor.defense -= actor.baseDefense/2;
                break;
            case "MOV+":
                actor.movement += actor.baseMovement/2;
                break;
            case "MOV-":
                actor.movement -= actor.baseMovement/2;
                break;
        }
    }
}
