using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyDataManager : BasicDataManager
{
    private string loadedData;
    public List<PlayerActor> partyMembers;
    public List<PlayerActor> allPartyMembers;
    protected void GetAllPartyMembers()
    {
        allPartyMembers.Clear();
        allPartyMembers.Add(GameManager.instance.player);
        allPartyMembers.Add(GameManager.instance.familiar);
        for (int i = 0; i < partyMembers.Count; i++)
        {
            if (partyMembers[i].typeName.Length <= 0 || partyMembers[i].currentHealth <= 0){continue;}
            allPartyMembers.Add(partyMembers[i]);
        }
        for (int i = 0; i < allPartyMembers.Count; i++)
        {
            if (allPartyMembers[i].typeName == "Player" || allPartyMembers[i].typeName == "Familiar")
            {
                allPartyMembers[i].UpdateStats();
            }
            else
            {
                allPartyMembers[i].SideCharacterUpdateStats();
            }
        }
    }
    public void PartyMemberDefeated(string memberName)
    {
        for (int i = 0; i < partyMembers.Count; i++)
        {
            if (partyMembers[i].typeName == memberName)
            {
                // If they're already defeated find the next one with the same name.
                if (partyMembers[i].currentHealth == 0){continue;}
                partyMembers[i].currentHealth = 0;
                return;
            }
        }
    }
    public void PartyWipe()
    {
        for (int i = 0; i < partyMembers.Count; i++)
        {
            partyMembers[i].currentHealth = 0;
        }
    }
    public void UpdatePartyHealth(List<string> names, List<int> healths)
    {
        // O(n^2)
        for (int i = 0; i < allPartyMembers.Count; i++)
        {
            if (allPartyMembers[i].currentHealth <= 0){continue;}
            string memberName = allPartyMembers[i].typeName;
            int indexOf = names.IndexOf(memberName);
            if (indexOf >= 0)
            {
                int health = Mathf.Max(0, healths[indexOf]);
                allPartyMembers[i].UpdateCurrentHealth(health);
                names.RemoveAt(indexOf);
                healths.RemoveAt(indexOf);
            }
        }
        UpdateAvailableHealths();
    }
    public List<string> availableFighters = new List<string>(0);
    public List<string> fighterHealths = new List<string>(0);
    protected void UpdateAvailableHealths()
    {
        availableFighters.Clear();
        fighterHealths.Clear();
        for (int i = 0; i < allPartyMembers.Count; i++)
        {
            if (allPartyMembers[i].typeName == "Player" || allPartyMembers[i].typeName == "Familiar"){continue;}
            int currentHealth = allPartyMembers[i].ReturnCurrentHealth();
            if (currentHealth <= 0){continue;}
            availableFighters.Add(allPartyMembers[i].typeName);
            fighterHealths.Add(currentHealth.ToString());
        }
        LoadAvailableFighters();
        GetAllPartyMembers();
    }

    public override void NewGame()
    {
        availableFighters.Clear();
        fighterHealths.Clear();
        LoadAvailableFighters();
        GetAllPartyMembers();
        Save();
    }

    public override void Save()
    {
        saveDataPath = Application.persistentDataPath;
        string fighterData = GameManager.instance.ConvertListToString(availableFighters);
        fighterData += "#"+GameManager.instance.ConvertListToString(fighterHealths);
        File.WriteAllText(saveDataPath+"/fighters.txt", fighterData);
    }

    public override void Load()
    {
        saveDataPath = Application.persistentDataPath;
        if (File.Exists(saveDataPath+"/fighters.txt"))
        {
            loadedData = File.ReadAllText(saveDataPath+"/fighters.txt");
            string[] dataBlocks = loadedData.Split("#");
            availableFighters = dataBlocks[0].Split("|").ToList();
            fighterHealths = dataBlocks[1].Split("|").ToList();
            GameManager.instance.RemoveEmptyListItems(availableFighters);
            GameManager.instance.RemoveEmptyListItems(fighterHealths);
        }
        LoadAvailableFighters();
        GetAllPartyMembers();
    }

    public void GainFighter(string fighterName)
    {
        availableFighters.Add(fighterName);
        fighterHealths.Add("-1");
        LoadAvailableFighters();
        GetAllPartyMembers();
    }

    protected void LoadAvailableFighters()
    {
        if (availableFighters.Count <= 0)
        {
            for (int i = 0; i < Mathf.Min(availableFighters.Count, partyMembers.Count); i++)
            {
                partyMembers[i].typeName = "";
                partyMembers[i].UpdateCurrentHealth();
            }
            return;
        }
        for (int i = 0; i < Mathf.Min(availableFighters.Count, partyMembers.Count); i++)
        {
            if (availableFighters[i].Length <= 0){continue;}
            partyMembers[i].typeName = availableFighters[i];
            partyMembers[i].SideCharacterUpdateStats();
            partyMembers[i].UpdateCurrentHealth(int.Parse(fighterHealths[i]));
        }
    }
}
