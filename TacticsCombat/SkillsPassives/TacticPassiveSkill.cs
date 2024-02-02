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
        if (condition == "Direction")
        {
            int cDirection = passiveHolder.currentDirection;
            switch (conditionSpecifics)
            {
                case "Opposing":
                    return OpposingDirections(cDirection, damageDirection);
                case "Opposite":
                    return OppositeDirection(cDirection, damageDirection);
                case "Identical":
                    return SameDirection(cDirection, damageDirection);
            }
        }
        return false;
    }

    // Helper functions for directional conditions.
    protected bool ActorSameDirection(TacticActor actor1, TacticActor actor2)
    {
        return (SameDirection(actor1.currentDirection, actor2.currentDirection));
    }

    protected bool SameDirection(int direction1, int direction2)
    {
        if (direction1 == direction2){return true;}
        return false;
    }

    protected bool ActorOppositeDirection(TacticActor actor1, TacticActor actor2)
    {
        return (OppositeDirection(actor1.currentDirection, actor2.currentDirection));
    }

    protected bool OppositeDirection(int direction1, int direction2)
    {
        if ((direction1+3)%6 == direction2){return true;}
        return false;
    }

    protected bool OpposingDirections(int direction1, int direction2)
    {
        // Either opposite or adjacent to opposite directions.
        int option1 = direction1;
        int option2 = direction1+1;
        int option3 = direction1-1;
        if ((option1+3)%6 == direction2){return true;}
        if ((option2+3)%6 == direction2){return true;}
        if ((option3+3)%6 == direction2){return true;}
        return false;
    }
}
