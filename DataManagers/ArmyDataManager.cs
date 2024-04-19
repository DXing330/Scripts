using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyDataManager : BasicDataManager
{
    public List<PlayerActor> partyMembers;
    public List<PlayerActor> allPartyMembers;
    public void GetAllPartyMembers()
    {
        allPartyMembers.Clear();
        allPartyMembers.Add(GameManager.instance.player);
        allPartyMembers.Add(GameManager.instance.familiar);
        for (int i = 0; i < partyMembers.Count; i++)
        {
            if (partyMembers[i].typeName.Length <= 0 || partyMembers[i].currentHealth <= 0){continue;}
            allPartyMembers.Add(partyMembers[i]);
        }
    }
    protected void GetPartyMembersAndStats()
    {
        GetAllPartyMembers();
        UpdatePartyStats();
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
    public PlayerActor viewStatsActor;
    public int viewStatsIndex = -1;
    public void SetViewStatsIndex(int index)
    {
        viewStatsIndex = index;
        if (viewStatsIndex >= 0 && viewStatsIndex < allPartyMembers.Count)
        {
            viewStatsActor = allPartyMembers[viewStatsIndex];
        }
    }
    public void SetViewStatsActor(PlayerActor newActor)
    {
        viewStatsActor = newActor;
    }
    // This function should remove party members that have no more health.
    protected void UpdateAvailableHealths()
    {
        availableFighters.Clear();
        fighterHealths.Clear();
        List<int> deadMembers = new List<int>();
        for (int i = 0; i < allPartyMembers.Count; i++)
        {
            if (allPartyMembers[i].typeName == "Player" || allPartyMembers[i].typeName == "Familiar"){continue;}
            int currentHealth = allPartyMembers[i].ReturnCurrentHealth();
            if (currentHealth <= 0)
            {
                deadMembers.Insert(0, i);
                continue;
            }
            availableFighters.Add(allPartyMembers[i].typeName);
            fighterHealths.Add(currentHealth.ToString());
        }
        LoadAvailableFighters();
        GetPartyMembersAndStats();
        // Need to also remove equipment from party members that have been defeated. Usually if they die they lose all their equipment, basically destroyed in battle or so damaged its worthless.
        GameManager.instance.equipInventory.LoseEquipSetsAtIndices(deadMembers);
    }

    public void UpdatePartyStats()
    {
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

    public override void NewGame()
    {
        saveDataPath = Application.persistentDataPath;
        if (File.Exists(saveDataPath+"/fighters.txt"))
        {
            File.Delete (saveDataPath+"/fighters.txt");
        }
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
            if (dataBlocks.Length >= 2)
            {
                availableFighters = dataBlocks[0].Split("|").ToList();
                fighterHealths = dataBlocks[1].Split("|").ToList();
            }
            GameManager.instance.RemoveEmptyListItems(availableFighters);
            GameManager.instance.RemoveEmptyListItems(fighterHealths);
        }
        LoadAvailableFighters();
        //GetAllPartyMembers();
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
