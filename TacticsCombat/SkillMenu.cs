using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillMenu : MonoBehaviour
{
    public Text skillName;
    public TMP_Text skillEnergyCost;
    public Text skillDetails;
    public TMP_Text skillActionCost;
    public TacticActiveSkill activeSkill;
    public SkillSelectList skillList;
    public AttackMenu lockOnMenu;
    public TerrainMap terrainMap;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void ResetSkillInfo()
    {
        Color tempColor = Color.white;
        tempColor.a = 0f;
        skillName.text = "N/A";
        skillDetails.text = "N/A";
    }

    public void UpdateSkill(TacticActiveSkill newSkill)
    {
        skillList.UpdateSkills();
        activeSkill = newSkill;
        if (newSkill == null)
        {
            ResetSkillInfo();
            return;
        }
        UpdateSkillInfo();
    }

    private void UpdateSkillInfo()
    {
        string skillDetailsText = "";
        // Need to show the energy and action cost.
        skillName.text = activeSkill.skillName;
        skillActionCost.text = activeSkill.ReturnActionCostString();
        skillEnergyCost.text = activeSkill.cost.ToString();
        skillDetailsText += activeSkill.flavorText;
        skillDetailsText += "\n"+activeSkill.ReturnEffectDescription();
        skillDetails.text = skillDetailsText;
    }

    public void SwitchSkill(bool right = true)
    {
        terrainMap.SwitchSkill(right);
        UpdateSkill(terrainMap.ReturnCurrentSkill());
    }

    public void SelectSkill(int skillIndex)
    {
        skillList.SelectSkill(skillIndex);
        UpdateSkill(terrainMap.ReturnCurrentSkill());
        animator.SetTrigger("Select");
    }

    public void UseSkill()
    {
        if (!terrainMap.CheckSkillActivatable())
        {
            return;
        }
        /*if (terrainMap.targetableTiles.Count <= 0)
        {
            return;
        }*/
        if (activeSkill.lockOn == 0)
        {
            animator.SetTrigger("Use");
        }
        else if (activeSkill.lockOn == 1)
        {
            if (terrainMap.targetableTiles.Count <= 0)
            {
                return;
            }
            animator.SetTrigger("Lock");
            lockOnMenu.UpdateTarget(terrainMap.ReturnCurrentTarget());
        }
    }
}
