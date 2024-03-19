using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewSkillGUI : MonoBehaviour
{
    public SkillDataManager skillData;
    public TacticActiveSkill dummySkill;
    public PlayerActor actor;
    public void SetActor(PlayerActor nActor)
    {
        actor = nActor;
    }
    public TextList skillTextList;
    public void UpdateSkillTextList(bool active = true)
    {
        if (active)
        {
            // Grab the currently selected actor's active list.
            skillTextList.SetAllText(actor.ReturnActives());
        }
        else
        {
            skillTextList.SetAllText(actor.ReturnPassives());
        }
    }

    protected void UpdateSkill(string skillName)
    {
        skillData.LoadDataForSkill(dummySkill, skillName);
    }

    public void ClickOnSkill(int index)
    {
        skillTextList.HighlightText(index);
        UpdateSkill(skillTextList.ReturnText(index));
    }
}
