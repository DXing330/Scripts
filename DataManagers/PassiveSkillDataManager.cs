using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveSkillDataManager : MonoBehaviour
{
    public string configData;
    public List<string> passiveNames;
    public List<string> passiveTimings;
    public List<string> passiveCondition;
    public List<string> passiveConditionSpecifics;
    public List<string> passiveEffects;
    public List<string> passiveSpecifics;
    public List<string> passiveFlavors;

    void Start()
    {
        LoadAllData();
    }

    [ContextMenu("Load")]
    public void LoadAllData()
    {
        string[] configBlocks = configData.Split("#");
        passiveNames = configBlocks[0].Split("|").ToList();
        passiveTimings = configBlocks[1].Split("|").ToList();
        passiveCondition = configBlocks[2].Split("|").ToList();
        passiveConditionSpecifics = configBlocks[3].Split("|").ToList();
        passiveEffects = configBlocks[4].Split("|").ToList();
        passiveSpecifics = configBlocks[5].Split("|").ToList();
        passiveFlavors = configBlocks[6].Split("|").ToList();
    }

    public void LoadDataForPassive(TacticPassiveSkill passive, string passiveName)
    {
        int index = passiveNames.IndexOf(passiveName);
        if (index < 0)
        {
            passive.passiveName = "";
            passive.timing = -1;
            passive.condition = "";
            passive.conditionSpecifics = "";
            passive.effect = "";
            passive.effectSpecifics = "";
            passive.flavor = "";
            return;
        }
        passive.passiveName = passiveNames[index];
        passive.timing = int.Parse(passiveTimings[index]);
        passive.condition = passiveCondition[index];
        passive.conditionSpecifics = passiveConditionSpecifics[index];
        passive.effect = passiveEffects[index];
        passive.effectSpecifics = passiveSpecifics[index];
        passive.flavor = passiveFlavors[index];
    }
}
