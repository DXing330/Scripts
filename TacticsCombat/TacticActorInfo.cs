using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TacticActorInfo : MonoBehaviour
{
    public Text health;
    public Text attack;
    public Text defense;
    public Text movement;
    public Text energy;
    public Text actions;

    public void ResetInfo()
    {
        health.text = "";
        attack.text = "";
        defense.text = "";
        energy.text = "";
        movement.text = "";
        actions.text = "";
    }

    public void UpdateInfo(TacticActor actor)
    {
        // Maybe make this a ratio bar?
        if (actor == null)
        {
            ResetInfo();
            return;
        }
        health.text = actor.health.ToString();
        attack.text = actor.attackDamage.ToString();
        defense.text = actor.defense.ToString();
        energy.text = actor.energy.ToString();
        movement.text = actor.movement.ToString();
        actions.text = actor.actionsLeft.ToString();
    }
}
