using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionLog : MonoBehaviour
{
    public bool testing = false;
    void Start()
    {
        if (testing)
        {
            for (int i = 0; i < 100; i++)
            {
                AddActionLog(i.ToString());
            }
        }
    }
    public static ActionLog instance;
    void Awake()
    {
        instance = this;
    }
    public List<string> actionLog;
    public List<TMP_Text> actionTexts;
    public int currentPage = 0;
    public Scrollbar scrollbar;

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
    private void UpdateActionTexts()
    {
        ResetTexts();
        // Update the text in reverse so the bottom is the newest.
        if (actionLog.Count >= actionTexts.Count)
        {
            for (int i = 0; i < actionTexts.Count; i++)
            {
                actionTexts[actionTexts.Count - i - 1].text = actionLog[i+currentPage];
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

    // Scrollbar resets whenever new actions are added.
    private void UpdateScrollBar()
    {
        scrollbar.value = 0;
        scrollbar.size = Mathf.Min(1, actionTexts.Count/actionLog.Count);
    }

    public void ChangePage()
    {
        if (actionTexts.Count >= actionLog.Count){return;}
        float scrollValue = scrollbar.value;
        // currentPage ranges in value from 0 to actionLog.Count - 10.
        currentPage = (int) (scrollValue * (actionLog.Count - 10));
        UpdateActionTexts();
    }

    public void AddActionLog(string newAction)
    {
        actionLog.Insert(0, newAction);
        currentPage = 0;
        UpdateActionTexts();
        UpdateScrollBar();
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
