using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticPassiveSkill : MonoBehaviour
{
    public string passiveName;
    // 0 = start turn, 1 = end turn, 2 = battle, 3 = moving, etc.
    public int timing;
    public string condition;
    public string conditionSpecifics;
    public string effect;
    public string effectSpecifics;

    public void AffectActor(TacticActor passiveHolder)
    {

    }

    // Probably need a different one for each timing.
    protected bool CheckConditions()
    {
        if (condition == "None"){return true;}
        return false;
    }
}
