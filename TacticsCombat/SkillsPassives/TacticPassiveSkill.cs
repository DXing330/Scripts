using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticPassiveSkill : MonoBehaviour
{
    public string passiveName;
    // 0 = start turn, 1 = end turn, 2 = battle, 5 = moving, etc.
    public int timing;
    public string condition;
    public string conditionSpecifics;
    public string effect;
    public string effectSpecifics;

    public void AffectActor(TacticActor actor)
    {
        int amount = int.Parse(effectSpecifics);
        switch (effect)
        {
            case "Health":
                actor.RegainHealth(amount);
                break;
        }
    }

    public int AffectDamage(int damage)
    {
        int amount = int.Parse(effectSpecifics);
        switch (effect)
        {
            case "Increase":
                damage += amount;
                break;
            case "Decrease":
                damage -= amount;
                break;
            case "Increase%":
                damage += damage * amount / 10;
                break;
            case "Decrease%":
                damage -= damage * amount / 10;
                break;
        }
        return damage;
    }

    // Probably need a different one for each timing.
    public bool StartTurnConditions(TacticActor passiveHolder)
    {
        if (condition == "None"){return true;}
        return false;
    }

    public bool EndturnConditions(TacticActor passiveHolder)
    {
        if (condition == "None"){return true;}
        return false;
    }

    public bool AttackingConditions(TacticActor passiveHolder, TacticActor defender)
    {
        if (condition == "None"){return true;}
        return false;
    }

    public bool DefendingConditions(TacticActor passiveHolder, TacticActor attacker)
    {
        if (condition == "None"){return true;}
        return false;
    }

    public bool DamagedConditions(TacticActor passiveHolder, int damageAmount, int damageDirection, int damageType)
    {
        if (condition == "None"){return true;}
        return false;
    }

    // Helper functions for directional conditions.
    protected bool SameDirection(TacticActor actor1, TacticActor actor2)
    {
        if (actor1.currentDirection == actor2.currentDirection){return true;}
        return false;
    }

    protected bool OppositeDirection(TacticActor actor1, TacticActor actor2)
    {
        if ((actor1.currentDirection+3)%6 == actor2.currentDirection){return true;}
        return false;
    }

}
