using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EquipmentStatsUI : MonoBehaviour
{
    public List<StatImageText> statTexts;

    public void ResetStatTexts()
    {
        for (int i = 0; i < statTexts.Count; i++)
        {
            statTexts[i].SetText("");
        }
    }

    public void UpdateStatTextsFromString(string equipInfo)
    {
        ResetStatTexts();
        string[] blocks = equipInfo.Split("|");
        for (int i = 0; i < 3; i++)
        {
            statTexts[i].SetText(blocks[i+1]);
        }
        statTexts[3].SetText(blocks[0]);
        string[] equipStats = blocks[4].Split(",");
        string skills = "";
        for (int i = 0; i < equipStats.Length; i++)
        {
            if (equipStats[i].Length <= 1){continue;}
            skills += equipStats[i]+"\n";
        }
        statTexts[4].SetText(skills);
        equipStats = blocks[5].Split(",");
        skills = "";
        for (int i = 0; i < equipStats.Length; i++)
        {
            if (equipStats[i].Length <= 1){continue;}
            skills += equipStats[i]+"\n";
        }
        statTexts[5].SetText(skills);
    }
}
