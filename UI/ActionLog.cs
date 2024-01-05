using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ActionLog : MonoBehaviour
{
    public List<string> actionLog;
    public List<TMP_Text> actionTexts;
    // Later allow for scrolling up or down.
    public int currentPage = 0;

    private void ResetTexts()
    {
        for (int i = 0; i < actionTexts.Count; i++)
        {
            actionTexts[i].text = "";
        }
    }

    public void ClearActionLog()
    {
        actionLog.Clear();
        UpdateActionTexts();
    }

    // Update with the newest actions.
    public void UpdateActionTexts()
    {
        ResetTexts();
        // Update the text in reverse so the bottom is the newest.
        if (actionLog.Count >= actionTexts.Count)
        {
            for (int i = 0; i < actionTexts.Count; i++)
            {
                actionTexts[actionTexts.Count - i - 1].text = actionLog[i];
            }
        }
        // Ensure that the bottom text is the newest.
        else
        {
            for (int i = 0; i < actionLog.Count; i++)
            {
                actionTexts[actionLog.Count - i - 1].text = actionLog[i];
            }
        }
    }

    public void AddActionLog(string newAction)
    {
        actionLog.Insert(0, newAction);
        UpdateActionTexts();
    }

    public void AddSkillAction(TacticActor skillUser, TacticActor skillTarget)
    {
        string newAction = "";
        if (skillUser != skillTarget)
        {
            newAction = skillUser.typeName+" used "+skillUser.activeSkill.skillName+" on "+skillTarget.typeName+".";
        }
        else
        {
            newAction = skillUser.typeName+" used "+skillUser.activeSkill.skillName+".";
        }
        AddActionLog(newAction);
    }

    public void AddTerrainEffect(TacticActor actor, int terrainType)
    {
        string newAction = "";
        switch (terrainType)
        {
            case 7:
                newAction = actor.typeName+" is on top of a chasm.";
                break;
        }
        if (newAction.Length < 6){return;}
        AddActionLog(newAction);
    }
}
