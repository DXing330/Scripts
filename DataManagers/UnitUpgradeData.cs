using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitUpgradeData : MonoBehaviour
{
    private string saveDataPath;
    private string loadedData;
    // Bonus health/attack/defense for each unit.
    public List<string> upgradedUnits;
    public List<string> bonusHealth;
    public List<string> bonusAttack;
    public List<string> bonusDefense;
    public List<string> bonusPassiveSkills;
    public List<string> bonusActiveSkills;

    public void NewGame()
    {
        upgradedUnits.Clear();
        bonusHealth.Clear();
        bonusAttack.Clear();
        bonusDefense.Clear();
        Save();
        Load();
    }

    public void Save()
    {
        saveDataPath = Application.persistentDataPath;
        string upgradeData = "";
        upgradeData += GameManager.instance.ConvertListToString(upgradedUnits)+"#";
        upgradeData += GameManager.instance.ConvertListToString(bonusHealth)+"#";
        upgradeData += GameManager.instance.ConvertListToString(bonusAttack)+"#";
        upgradeData += GameManager.instance.ConvertListToString(bonusDefense)+"#";
        File.WriteAllText(saveDataPath+"/upgradeData.txt", upgradeData);
    }

    public void Load()
    {
        saveDataPath = Application.persistentDataPath;
        loadedData = File.ReadAllText(saveDataPath+"/upgradeData.txt");
        string[] blocks = loadedData.Split("#");
        upgradedUnits = blocks[0].Split("|").ToList();
        bonusHealth = blocks[1].Split("|").ToList();
        bonusAttack = blocks[2].Split("|").ToList();
        bonusDefense = blocks[3].Split("|").ToList();
    }

    public void AddNewUnit(string unitName)
    {
        if (upgradedUnits.Contains(unitName))
        {
            return;
        }
        upgradedUnits.Add(unitName);
        bonusHealth.Add("0");
        bonusAttack.Add("0");
        bonusDefense.Add("0");
        Save();
    }

    public void UpgradeUnit(string unitName, int statType, int statAmount)
    {
        // O(n);
        int indexOf = upgradedUnits.IndexOf(unitName);
        if (indexOf < 0)
        {
            AddNewUnit(unitName);
            indexOf = upgradedUnits.Count - 1;
        }
        switch (statType)
        {
            case 0:
                bonusHealth[indexOf] = (int.Parse(bonusHealth[indexOf])+statAmount).ToString();
                break;
            case 1:
                bonusAttack[indexOf] = (int.Parse(bonusAttack[indexOf])+statAmount).ToString();
                break;
            case 2:
                bonusDefense[indexOf] = (int.Parse(bonusDefense[indexOf])+statAmount).ToString();
                break;
        }
    }

    public void UpgradeUnitRandomly(string unitName)
    {
        int indexOf = upgradedUnits.IndexOf(unitName);
        if (indexOf < 0)
        {
            AddNewUnit(unitName);
            indexOf = upgradedUnits.Count - 1;
        }
        // Add 6% to a random stat.
        int rng = Random.Range(0, 3);
        switch (rng)
        {
            case 0:
                bonusHealth[indexOf] = (int.Parse(bonusHealth[indexOf])+6).ToString();
                break;
            case 1:
                bonusAttack[indexOf] = (int.Parse(bonusAttack[indexOf])+6).ToString();
                break;
            case 2:
                bonusDefense[indexOf] = (int.Parse(bonusDefense[indexOf])+6).ToString();
                break;
        }
    }

    public void AdjustUnitStats(TacticActor actor)
    {
        int indexOf = upgradedUnits.IndexOf(actor.typeName);
        if (indexOf < 0)
        {
            return;
        }
        // Add a percentage of base stats.
        actor.baseHealth += actor.baseHealth*int.Parse(bonusHealth[indexOf])/100;
        actor.baseAttack += actor.baseAttack*int.Parse(bonusAttack[indexOf])/100;
        actor.baseDefense += actor.baseDefense*int.Parse(bonusDefense[indexOf])/100;
    }

    public string ReturnUpgradeAmounts(string unitName)
    {
        int indexOf = upgradedUnits.IndexOf(unitName);
        if (indexOf < 0)
        {
            return "";
        }
        string stats = "";
        stats += bonusHealth[indexOf]+"|"+bonusAttack[indexOf]+"|"+bonusDefense[indexOf];
        return stats;
    }
}
