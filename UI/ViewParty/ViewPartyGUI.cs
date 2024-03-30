using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ViewPartyGUI : BasicGUI
{
    public PartyMemberList partyMemberList;
    public int selectedMember = -1;
    public void SelectMember(int selected)
    {
        if (selected == selectedMember){selectedMember = -1;}
        else{selectedMember = selected;}
        GameManager.instance.armyData.SetViewStatsIndex(selected);
        HighlightSelectedMember();
        ActivatePanels();
    }
    protected void HighlightSelectedMember()
    {
        partyMemberList.HighlightSelectedMember(selectedMember);
    }
    public int state = -1;
    public List<TMP_Text> stateTexts;
    public MoreStatsSheet equipmentStats;
    public EquipmentSelectGUI equipmentSelect;
    public ViewSkillGUI skillView;
    public void SelectState(int selected)
    {
        if (selected == state){state = -1;}
        else {state = selected;}
        ActivatePanels();
        HighlightStateText();
    }
    public List<GameObject> panels;
    protected void ActivatePanels()
    {
        for (int i = 0; i < panels.Count; i++)
        {
            panels[i].SetActive(false);
        }
        if (selectedMember < 0){return;}
        if (state >= 0)
        {
            panels[state].SetActive(true);
        }
        UpdatePanels();
    }

    protected void UpdatePanels()
    {
        if (state < 0 || selectedMember < 0){return;}
        switch (state)
        {
            case 0:
                equipmentSelect.GetViewedActor();
                equipmentStats.ReUpdate();
                equipmentSelect.ResetEquipType();
                break;
            case 1:
                skillView.SetActor(partyMemberList.partyMembers[selectedMember]);
                skillView.UpdateSkillTextList();
                skillView.ResetSkillView();
                break;
        }
    }

    protected void BoldStateText()
    {
        for (int i = 0; i < stateTexts.Count; i++)
        {
            bool isSet = (stateTexts[i].fontStyle & FontStyles.Bold) != 0;
            if(isSet){stateTexts[i].fontStyle ^= FontStyles.Bold;}
        }
        if (state >= 0)
        {
            stateTexts[state].fontStyle |= FontStyles.Bold;
        }
    }

    protected void HighlightStateText()
    {
        for (int i = 0; i < stateTexts.Count; i++)
        {
            stateTexts[i].color = Color.black;
        }
        if (state >= 0)
        {
            stateTexts[state].color = Color.red;
        }
    }
}
