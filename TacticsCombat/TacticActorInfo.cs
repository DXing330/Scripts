using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TacticActorInfo : MonoBehaviour
{
    public TMP_Text health;
    public TMP_Text attack;
    public TMP_Text defense;
    public TMP_Text movement;
    public TMP_Text energy;
    public TMP_Text actions;

    public void ResetInfo()
    {
        health.text = "";
        attack.text = "";
        defense.text = "";
        energy.text = "";
        //movement.text = "";
        //actions.text = "";
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
        //movement.text = actor.movement.ToString();
        //actions.text = actor.actionsLeft.ToString();
    }
}
