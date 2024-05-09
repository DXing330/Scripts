using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ViewPassiveGUI : ViewSkillGUI
{
    public PassiveSkillDataManager passiveData;
    public TacticPassiveSkill dummyPassive;
    public TMP_Text passiveFlavorText;

    public void UpdatePassiveTextList()
    {
        skillTextList.SetAllText(actor.ReturnPassives());
    }

    protected override void UpdateSkill(string skillName)
    {
        passiveData.LoadDataForPassive(dummyPassive, skillName);
        passiveFlavorText.text = dummyPassive.flavor;
        UpdateDescription();
    }

    protected override void UpdateDescription()
    {
        string description = "";
        description = GeneratePassiveDescription(description);
        skillDescription.text = description;
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
        }
        return "";
    }

    protected string DetermineConditionText()
    {
        switch (dummyPassive.condition)
        {
            case "None":
                return "";
            case "Direction":
                switch (dummyPassive.conditionSpecifics)
                {
                    case "Opposing":
                        return "if you are facing the enemy.";
                    case "Identical":
                        return "if you are facing the back of the enemy.";
                }
                break;
        }
        return "";
    }

    protected string DetermineEffectText()
    {
        int amount = int.Parse(dummyPassive.effectSpecifics);
        switch (dummyPassive.effect)
        {
            case "Health":
                if (amount > 0){return "regain "+amount+" health.";}
                else{return "lose "+Math.Abs(amount)+" health.";}
            case "Decrease":
                return "decrease the damage by "+(amount)+" ";
            case "Increase%":
                return "increase the damage by "+(amount*10)+"% ";
        }
        return "";
    }
}
