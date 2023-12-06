using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyDataManager : BasicDataManager
{
    private string saveDataPath;
    private string loadedData;
    public List<PlayerActor> partyMembers;
    public int maxArmySize = 9;
    public List<string> armyFormation = new List<string>(9);
    public List<string> availableFighters = new List<string>(0);

    private void AdjustLists()
    {
        GameManager.instance.RemoveEmptyListItems(availableFighters);
        for (int i = 0; i < armyFormation.Count; i++)
        {
            if (armyFormation[i].Length < 3)
            {
                armyFormation[i] = "none";
            }
        }
    }

    private void LoadPartyMembersByName()
    {
        GameManager.instance.ResetParty();
        int partyMemberIndex = 0;
        for (int i = 0; i < armyFormation.Count; i++)
        {
            if (armyFormation[i] == "none" || armyFormation[i] == "Player" || armyFormation[i] == "Familiar"){continue;}
            partyMembers[partyMemberIndex].SetName(armyFormation[i]);
            GameManager.instance.playerActors.Add(partyMembers[partyMemberIndex]);
            partyMemberIndex++;
        }
    }

    public override void NewGame()
    {
        armyFormation.Clear();
        for (int i = 0; i < maxArmySize; i++)
        {
            armyFormation.Add("none");
        }
        armyFormation[0] = "Familiar";
        armyFormation[1] = "Player";
        availableFighters.Clear();
        Save();
    }

    public override void Save()
    {
        saveDataPath = Application.persistentDataPath;
        string armyData = GameManager.instance.ConvertListToString(armyFormation);
        File.WriteAllText(saveDataPath+"/armyData.txt", armyData);
        string fighterData = GameManager.instance.ConvertListToString(availableFighters);
        File.WriteAllText(saveDataPath+"/fighters.txt", fighterData);
    }

    public override void Load()
    {
        saveDataPath = Application.persistentDataPath;
        loadedData = File.ReadAllText(saveDataPath+"/armyData.txt");
        armyFormation = loadedData.Split("|").ToList();
        loadedData = File.ReadAllText(saveDataPath+"/fighters.txt");
        availableFighters = loadedData.Split("|").ToList();
        if (armyFormation.Count < maxArmySize)
        {
            NewGame();
        }
        AdjustLists();
        LoadPartyMembersByName();
    }

    public void GainFighter(string fighterName)
    {
        availableFighters.Add(fighterName);
    }
}
