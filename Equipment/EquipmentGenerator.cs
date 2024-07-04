using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentGenerator : MonoBehaviour
{
    public int totalEquipTypes = 4;
    public string randomEquipTestType;
    public int randomEquipTestRarity;
    public int testQuantity;
    public EquipmentData equipData;

    public string GenerateEquipment(int rarity, string type)
    {
        string newEquip = "";
        List<string> baseStats = equipData.LoadEquipBaseStats(type);
        if (baseStats.Count <= 1){return newEquip;}
        newEquip += GenerateStat(rarity, int.Parse(baseStats[0]))+"|";
        newEquip += GenerateStat(rarity, int.Parse(baseStats[1]))+"|";
        newEquip += GenerateStat(rarity, int.Parse(baseStats[2]))+"|";
        newEquip += GenerateStat(rarity, int.Parse(baseStats[3]))+"|";
        newEquip += GenerateSkills(rarity, baseStats[4], baseStats[6])+"|";
        newEquip += GenerateSkills(rarity, baseStats[5], baseStats[7])+"|";
        newEquip += baseStats[8]+"|";
        newEquip += baseStats[9]+"|";
        newEquip += baseStats[10]+"|";
        newEquip += rarity+"|"+type;
        return newEquip;
    }

    [ContextMenu("Generate")]
    public void TestGenerate()
    {
        for (int i = 0; i < testQuantity; i++)
        {
            Debug.Log(GenerateEquipment(randomEquipTestRarity, randomEquipTestType));
        }
    }

    protected string GenerateStat(int rarity, int baseStat)
    {
        if (baseStat == 0){return "0";}
        if (baseStat < 0){return baseStat.ToString();}
        string stat = "";
        int iStat = baseStat*(rarity+1);
        stat = iStat.ToString();
        return stat;
    }

    protected string GenerateSkills(int rarity, string baseStat, string rareStat)
    {
        string stat = "";
        stat += baseStat;
        if (rareStat.Length <= 1){return stat;}
        List<string> possibleRareEffects = rareStat.Split(",").ToList();
        int rareStatChance = possibleRareEffects.Count;
        int roll = 0;
        int effectIndex = -1;
        for (int i = 0; i < rarity; i++)
        {
            if (possibleRareEffects.Count <= 0){break;}
            roll = Random.Range(0, rareStatChance);
            // More rare effects means its harder to get them.
            if (roll == 0)
            {
                // Get a random rare effect.
                effectIndex = Random.Range(0, possibleRareEffects.Count);
                stat += ","+possibleRareEffects[effectIndex];
                possibleRareEffects.RemoveAt(effectIndex);
            }
        }
        return stat;
    }
}