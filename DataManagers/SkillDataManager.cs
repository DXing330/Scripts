using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDataManager : MonoBehaviour
{
    public string configData;
    public List<string> skillClass;
    public List<string> skillNames;
    public List<string> skillLockOns;
    public List<string> skillRanges;
    public List<string> skillSpans;
    public List<string> skillTargetingShapes;
    public List<string> skillTargets;
    public List<string> skillCosts;
    public List<string> skillEffects;
    public List<string> skillSpecifics;
    public List<string> skillBasePowers;
    public List<string> skillactionCosts;
    public List<string> skillFlavorTexts;

    void Start()
    {
        string[] configBlocks = configData.Split("#");
        skillClass = configBlocks[0].Split("|").ToList();
        skillNames = configBlocks[1].Split("|").ToList();
        skillLockOns = configBlocks[2].Split("|").ToList();
        skillRanges = configBlocks[3].Split("|").ToList();
        skillSpans = configBlocks[4].Split("|").ToList();
        skillTargets = configBlocks[5].Split("|").ToList();
        skillCosts = configBlocks[6].Split("|").ToList();
        skillEffects = configBlocks[7].Split("|").ToList();
        skillSpecifics = configBlocks[8].Split("|").ToList();
        skillBasePowers = configBlocks[9].Split("|").ToList();
        skillactionCosts = configBlocks[10].Split("|").ToList();
        skillFlavorTexts = configBlocks[11].Split("|").ToList();
        skillTargetingShapes = configBlocks[12].Split("|").ToList();
    }

    public void LoadDataForSkill(TacticActiveSkill activeSkill, string skillName)
    {
        int skillIndex = skillNames.IndexOf(skillName);
        if (skillIndex < 0)
        {
            activeSkill.skillName = "";
            activeSkill.cost = 0;
            activeSkill.effect = "";
            activeSkill.actionCost = "0";
            return;
        }
        activeSkill.skillName = skillNames[skillIndex];
        activeSkill.lockOn = int.Parse(skillLockOns[skillIndex]);
        activeSkill.range = int.Parse(skillRanges[skillIndex]);
        activeSkill.span = int.Parse(skillSpans[skillIndex]);
        activeSkill.skillTarget = int.Parse(skillTargets[skillIndex]);
        activeSkill.cost = int.Parse(skillCosts[skillIndex]);
        activeSkill.effect = skillEffects[skillIndex];
        activeSkill.effectSpecifics = skillSpecifics[skillIndex];
        activeSkill.basePower = int.Parse(skillBasePowers[skillIndex]);
        activeSkill.actionCost = skillactionCosts[skillIndex];
        activeSkill.flavorText = (skillFlavorTexts[skillIndex]);
        activeSkill.targetingShape = skillTargetingShapes[skillIndex];
    }
}
