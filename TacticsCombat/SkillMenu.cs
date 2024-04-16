using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillMenu : MonoBehaviour
{
    public TMP_Text TMPskillName;
    public TMP_Text skillEnergyCost;
    public TMP_Text TMPskillDetails;
    public TMP_Text skillActionCost;
    public List<GameObject> actionCostOrbs;
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
        TMPskillName.text = "N/A";
        TMPskillDetails.text = "N/A";
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
        TMPskillName.text = activeSkill.skillName;
        UpdateActionCost(activeSkill.ReturnActionCost());
        //skillActionCost.text = activeSkill.ReturnActionCostString();
        skillEnergyCost.text = activeSkill.cost.ToString();
        skillDetailsText += activeSkill.flavorText;
        skillDetailsText += "\n"+activeSkill.ReturnEffectDescription();
        TMPskillDetails.text = skillDetailsText;
    }

    private void UpdateActionCost(int cost)
    {
        GameManager.instance.utility.DisableAllObjects(actionCostOrbs);
        for (int i = 0; i < Mathf.Min(cost, actionCostOrbs.Count); i++)
        {
            actionCostOrbs[i].SetActive(true);
        }
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
