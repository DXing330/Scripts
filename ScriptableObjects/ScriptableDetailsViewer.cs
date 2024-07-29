using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ViewDetails", menuName = "ScriptableObjects/ViewDetails", order = 1)]
public class ScriptableDetailsViewer : ScriptableObject
{
    public PassiveSkillDataManager passiveData;
    public ScriptableDictionary damageTypes;
    public TacticPassiveSkill dummyPassive;
    public string ReturnSkillDescription(string skillName, bool active = false)
    {
        passiveData.LoadDataForPassive(dummyPassive, skillName);
        return GeneratePassiveDescription("");
    }

    protected string GeneratePassiveDescription(string description)
    {
        description += DetermineTimingText();
        description += DetermineEffectText();
        description += DetermineConditionText();
        return description;
    }

    protected string DetermineTimingText()
    {
        switch (dummyPassive.timing)
        {
            case 0:
                return "At the start of your turn ";
            case 1:
                return "At the end of your turn ";
            case 2:
                return "When you deal damage ";
            case 3:
                return "When you are attacked ";
            case 4:
                return "When you take damage ";
            case 5:
                return "When moving ";
            case 6:
                return "When you attack ";
            case 7:
                return "When you attack ";
            case 8:
                return "When you die ";
            case 11:
                return "At the start of battle ";
        }
        return "";
    }

    protected string DetermineConditionText()
    {
        switch (dummyPassive.condition)
        {
            case "None":
                return ".";
            case "Direction":
                switch (dummyPassive.conditionSpecifics)
                {
                    case "Opposing":
                        return " if you are facing the enemy.";
                    case "Identical":
                        return " if you are facing the back of the enemy.";
                }
                break;
            case "Distance":
                return " if within "+dummyPassive.conditionSpecifics+" tile(s).";
            case "Type":
                return " if the damage is "+damageTypes.ReturnValue(dummyPassive.conditionSpecifics)+".";
        }
        return ".";
    }

    protected string DetermineEffectText()
    {
        int amount = int.Parse(dummyPassive.effectSpecifics);
        switch (dummyPassive.effect)
        {
            case "Health":
                if (amount > 0){return "regain "+amount+" health";}
                else{return "lose "+Math.Abs(amount)+" health";}
            case "Decrease":
                return "decrease the damage by "+(amount)+"";
            case "Increase%":
                return "increase the damage by "+amount+"0%";
            case "Damage":
                return "deal "+amount+" damage";
            case "Ignore%":
                return "ignore "+amount+"0% defense";
            case "BATK":
                return "increase your attack by "+amount;
            case "BDEF":
                return "increase your defense by "+amount;
            case "Attack":
                return "increase your attack by "+amount;
            case "Defense":
                return "increase your defense by "+amount;

        }
        return "";
    }
}
