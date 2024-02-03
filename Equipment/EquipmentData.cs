using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentData : MonoBehaviour
{
    /*
    public string configData;
    public List<string> equipmentNames;
    public List<string> equipTypes;
    public List<string> possibleUsers;
    public List<string> equipHealths;
    public List<string> equipAttacks;
    public List<string> equipDefenses;
    public List<string> equipEnergies;
    public List<string> equipMovements;
    public List<string> equipAttackRanges;
    public List<string> equipActions;
    public List<string> equipMoveTypes;
    public List<string> equipSizes;
    public List<string> equipInitiatives;
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
        equipmentNames = configBlocks[0].Split("|").ToList();
        equipTypes = configBlocks[1].Split("|").ToList();
        possibleUsers = configBlocks[2].Split("|").ToList();
        equipHealths = configBlocks[3].Split("|").ToList();
        equipAttacks = configBlocks[4].Split("|").ToList();
        equipDefenses = configBlocks[5].Split("|").ToList();
        equipEnergies = configBlocks[6].Split("|").ToList();
        equipMovements = configBlocks[7].Split("|").ToList();
        equipAttackRanges = configBlocks[8].Split("|").ToList();
        equipActions = configBlocks[9].Split("|").ToList();
        equipMoveTypes = configBlocks[10].Split("|").ToList();
        equipSizes = configBlocks[11].Split("|").ToList();
        equipInitiatives = configBlocks[12].Split("|").ToList();
        flavorTexts = configBlocks[13].Split("|").ToList();
    }

    public void LoadEquipData(Equipment loaded, string equipName)
    {
        int index = equipmentNames.IndexOf(equipName);
        if (index < 0)
        {
            loaded.NullAllStats();
            return;
        }
        loaded.equipName = equipmentNames[index];
        loaded.baseHealth = int.Parse(equipHealths[index]);
        loaded.baseAttack = int.Parse(equipAttacks[index]);
        loaded.baseDefense = int.Parse(equipDefenses[index]);
        loaded.baseEnergy = int.Parse(equipEnergies[index]);
        loaded.baseMovement = int.Parse(equipMovements[index]);
        loaded.attackRange = int.Parse(equipAttackRanges[index]);
        loaded.baseActions = int.Parse(equipActions[index]);
        loaded.moveType = int.Parse(equipMoveTypes[index]);
        loaded.size = int.Parse(equipSizes[index]);
        loaded.baseInitiative = int.Parse(equipInitiatives[index]);
        loaded.equipType = int.Parse(equipTypes[index]);
        loaded.possibleUsers = possibleUsers[index];
        //loaded.flavorText = flavorTexts[index];
    }*/
}
