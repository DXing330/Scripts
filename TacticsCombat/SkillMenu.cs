using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillMenu : MonoBehaviour
{
    public Text skillName;
    public Text skillDetails;
    public TacticActiveSkill activeSkill;
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
        skillName.text = activeSkill.skillName;
        skillDetailsText += "Effect: "+activeSkill.effect;
        skillDetailsText += ", Range: "+activeSkill.range;
        skillDetailsText += ", Span: "+activeSkill.span;
        skillDetailsText += ", Cost: "+activeSkill.cost;
        skillDetails.text = skillDetailsText;
    }

    public void SwitchSkill(bool right = true)
    {
        terrainMap.SwitchSkill(right);
        UpdateSkill(terrainMap.ReturnCurrentSkill());
    }

    public void UseSkill()
    {
        if (!terrainMap.CheckSkillActivatable())
        {
            return;
        }
        if (activeSkill.lockOn == 0)
        {
            animator.SetTrigger("Use");
        }
        else if (activeSkill.lockOn == 1)
        {
            animator.SetTrigger("Lock");
            lockOnMenu.UpdateTarget(terrainMap.ReturnCurrentTarget());
        }
    }
}
