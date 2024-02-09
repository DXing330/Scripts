using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentData : MonoBehaviour
{
    public string configData;
    public List<string> equipTypes;
    public List<string> equipSlot;
    public List<string> possibleUsers;
    public List<string> userSizes;
    public List<string> equipHealths;
    public List<string> equipAttacks;
    public List<string> equipDefenses;
    public List<string> equipMovements;
    public List<string> baseActives;
    public List<string> basePassives;
    public List<string> rareActives;
    public List<string> rarePassives;
    public List<string> flavorTexts;

    // Start is called before the first frame update
    void Start()
    {
        LoadAllData();
    }

    [ContextMenu("Load")]
    public void LoadAllData()
    {
        string[] configBlocks = configData.Split("#");
        equipTypes = configBlocks[0].Split("|").ToList();
        equipSlot = configBlocks[1].Split("|").ToList();
        possibleUsers = configBlocks[2].Split("|").ToList();
        userSizes = configBlocks[3].Split("|").ToList();
        equipHealths = configBlocks[4].Split("|").ToList();
        equipAttacks = configBlocks[5].Split("|").ToList();
        equipDefenses = configBlocks[6].Split("|").ToList();
        equipMovements = configBlocks[7].Split("|").ToList();
        baseActives = configBlocks[8].Split("|").ToList();
        basePassives = configBlocks[9].Split("|").ToList();
        rareActives = configBlocks[10].Split("|").ToList();
        rarePassives = configBlocks[11].Split("|").ToList();
    }

    // Used for generating a random equipment.
    public List<string> LoadEquipBaseStats(string equipName)
    {
        List<string> stats = new List<string>();
        int index = equipTypes.IndexOf(equipName);
        if (index < 0)
        {
            return stats;
        }
        stats.Add(equipHealths[index]);
        stats.Add(equipAttacks[index]);
        stats.Add(equipDefenses[index]);
        stats.Add(equipMovements[index]);
        stats.Add(baseActives[index]);
        stats.Add(basePassives[index]);
        stats.Add(rareActives[index]);
        stats.Add(rarePassives[index]);
        stats.Add(equipSlot[index]);
        stats.Add(possibleUsers[index]);
        stats.Add(userSizes[index]);
        return stats;
    }
}
