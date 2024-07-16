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
    public ScriptableDictionary damageTypes;
    public ScriptableDetailsViewer passiveDescription;
    public TMP_Text passiveFlavorText;

    public void UpdatePassiveTextList()
    {
        skillTextList.SetAllText(actor.ReturnPassives());
    }

    protected override void UpdateSkill(string skillName)
    {
        skillDescription.text = passiveDescription.ReturnSkillDescription(skillName);
    }
}
