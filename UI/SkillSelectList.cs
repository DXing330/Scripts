using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkillSelectList : MonoBehaviour
{
    public List<GameObject> skillButtons;
    public List<TMP_Text> skillNames;
    public int currentPage = 0;
    public TacticActor currentActor;

    public void SetActor(TacticActor newActor)
    {
        currentActor = newActor;
    }

    public void SelectSkill(int skillIndex)
    {
        currentActor.terrainMap.SelectSkill(skillIndex + (currentPage * skillButtons.Count));
    }

    private void DisableButtons()
    {
        for (int i = 0; i < skillButtons.Count; i++)
        {
            skillButtons[i].SetActive(false);
        }
    }

    public void UpdateSkills()
    {
        DisableButtons();
        if (currentActor.activeSkillNames.Count <= 0){return;}
        int availableSkills = Mathf.Min(skillButtons.Count, currentActor.activeSkillNames.Count - (skillButtons.Count * currentPage));
        for (int i = 0; i < availableSkills; i++)
        {
            skillButtons[i].SetActive(true);
            skillNames[i].text = currentActor.activeSkillNames[i + (currentPage * skillButtons.Count)];
        }
    }

    public void ChangePage(bool right = true)
    {
        if (right)
        {
            if (currentActor.activeSkillNames.Count > skillButtons.Count * (currentPage + 1))
            {
                currentPage++;
            }
        }
        else
        {
            if (currentPage > 0)
            {
                currentPage--;
            }
        }
        UpdateSkills();
    }
}
