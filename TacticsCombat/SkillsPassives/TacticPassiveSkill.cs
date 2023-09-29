using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticPassiveSkill : MonoBehaviour
{
    public string effect;
    // 0 = start turn, 1 = end turn, 2 = battle, 3 = moving, etc.
    public int timing;
    public string condition;
    public string conditionSpecifics;
}
