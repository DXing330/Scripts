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
    public TerrainMap terrainMap;

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
        string currentRound = "Round "+(terrainMap.roundIndex+1).ToString()+"-"+(terrainMap.turnIndex+1).ToString();
        newAction = currentRound+":"+newAction;
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
            newAction = skillUser.ReturnName()+" used "+skillUser.activeSkill.skillName+" on "+skillTarget.ReturnName()+".";
        }
        else
        {
            newAction = skillUser.ReturnName()+" used "+skillUser.activeSkill.skillName+".";
        }
        AddActionLog(newAction);
    }

    public void AddNonLockOnSkill(TacticActor skillUser)
    {
        string newAction = "";
        newAction = skillUser.ReturnName()+" used "+skillUser.activeSkill.skillName+".";
        AddActionLog(newAction);
    }
}
