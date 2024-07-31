using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyDataManager : BasicDataManager
{
    public List<string> PCstats;
    protected void SavePCStats()
    {
        PCstats.Clear();
        PCstats.Add(GameManager.instance.player.ReturnCurrentStats());
        PCstats.Add(GameManager.instance.familiar.ReturnCurrentStats());
    }
    // Extra party members, excluding player and familiar.
    public List<PlayerActor> partyMembers;
    public List<string> partyMemberNames;
    public List<string> partyMemberStats;
    protected void SavePartyMemberStats()
    {
        if (partyMemberNames.Count <= 0)
        {
            return;
        }
        for (int i = partyMemberNames.Count - 1; i > -1; i--)
        {
            if (partyMembers[i].typeName.Length < 1 || partyMembers[i].ReturnCurrentHealth() <= 0)
            {
                partyMemberNames.RemoveAt(i);
                partyMemberStats.RemoveAt(i);
            }
        }
    }
    // All party members, including PC and familiar.
    public List<PlayerActor> allPartyMembers;
    public void GetAllPartyMembers()
    {
        allPartyMembers.Clear();
        // First add the PC and their familiar.
        allPartyMembers.Add(GameManager.instance.player);
        allPartyMembers.Add(GameManager.instance.familiar);
        // Then all all other party members.
        for (int i = 0; i < partyMembers.Count; i++)
        {
            if (partyMembers[i].typeName.Length <= 0 || partyMembers[i].currentHealth <= 0){continue;}
            allPartyMembers.Add(partyMembers[i]);
        }
        if (PCstats.Count <= 0){return;}
        // Update the PC health if possible.
        for (int i = 0; i < PCstats.Count; i++)
        {
            allPartyMembers[i].SetCurrentStats(PCstats[i]);
        }
    }
    protected void GetPartyMembersAndStats()
    {
        GetAllPartyMembers();
        UpdatePartyStats();
    }
    public void PartyMemberDefeated(string memberName)
    {
        if (memberName == "Player")
        {
            allPartyMembers[0].UpdateCurrentHealth(1);
            return;
        }
        if (memberName == "Familiar")
        {
            allPartyMembers[1].UpdateCurrentHealth(1);
            return;
        }
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
    
    public void UpdatePartyHealth(List<string> names, List<int> healths, List<int> energies)
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
                int energy = Mathf.Max(0, energies[indexOf]);
                allPartyMembers[i].UpdateCurrentHealth(health);
                allPartyMembers[i].UpdateCurrentEnergy(energy);
                names.RemoveAt(indexOf);
                healths.RemoveAt(indexOf);
                energies.RemoveAt(indexOf);
            }
        }
        UpdateAvailableHealths();
    }
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
        partyMemberNames.Clear();
        partyMemberStats.Clear();
        PCstats.Clear();
        List<int> deadMembers = new List<int>();
        for (int i = 0; i < allPartyMembers.Count; i++)
        {
            if (allPartyMembers[i].typeName == "Player" || allPartyMembers[i].typeName == "Familiar")
            {
                PCstats.Add(allPartyMembers[i].ReturnCurrentStats());
                continue;
            }
            int currentHealth = allPartyMembers[i].ReturnCurrentHealth();
            int currentEnergy = allPartyMembers[i].ReturnCurrentEnergy();
            if (currentHealth <= 0)
            {
                deadMembers.Insert(0, i);
                continue;
            }
            partyMemberNames.Add(allPartyMembers[i].typeName);
            partyMemberStats.Add(allPartyMembers[i].ReturnCurrentStats());
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
                allPartyMembers[i].UpdateStats(false);
            }
        }
    }

    public override void NewGame()
    {
        saveDataPath = Application.persistentDataPath;
        if (File.Exists(saveDataPath+fileName))
        {
            File.Delete (saveDataPath+fileName);
        }
        partyMemberNames.Clear();
        partyMemberStats.Clear();
        LoadAvailableFighters();
        GetAllPartyMembers();
        Save();
    }

    public override void Save()
    {
        UpdateAvailableHealths();
        SavePCStats();
        SavePartyMemberStats();
        saveDataPath = Application.persistentDataPath;
        string fighterData = GameManager.instance.utility.ConvertListToString(PCstats, "$");
        fighterData += "#"+GameManager.instance.utility.ConvertListToString(partyMemberStats, "$");
        fighterData += "#"+GameManager.instance.utility.ConvertListToString(partyMemberNames);
        File.WriteAllText(saveDataPath+fileName, fighterData);
    }

    public override void Load()
    {
        saveDataPath = Application.persistentDataPath;
        if (File.Exists(saveDataPath+fileName))
        {
            loadedData = File.ReadAllText(saveDataPath+fileName);
            string[] dataBlocks = loadedData.Split("#");
            if (dataBlocks.Length >= 2)
            {
                PCstats = dataBlocks[0].Split("$").ToList();
                partyMemberStats = dataBlocks[1].Split("$").ToList();
                partyMemberNames = dataBlocks[2].Split("|").ToList();
            }
            GameManager.instance.utility.RemoveEmptyListItems(PCstats, 0);
            GameManager.instance.utility.RemoveEmptyListItems(partyMemberStats, 0);
            GameManager.instance.utility.RemoveEmptyListItems(partyMemberNames, 0);
        }
        LoadAvailableFighters();
        GetAllPartyMembers();
    }

    public void TestGainFighter(string fighterName)
    {
        GainFighter(fighterName);
    }

    public void GainFighter(string fighterName, string stats = null)
    {
        partyMemberNames.Add(fighterName);
        // Update their base stats from the data manager for the first time.
        if (stats == null)
        {
            partyMembers[partyMemberNames.Count - 1].SideCharacterUpdateStats(fighterName);
            partyMemberStats.Add(partyMembers[partyMemberNames.Count - 1].ReturnCurrentStats());
        }
        else{partyMemberStats.Add(stats);}
        LoadAvailableFighters();
        GetAllPartyMembers();
    }

    public void FullPartyHeal()
    {
        for (int i = 0; i < allPartyMembers.Count; i++)
        {
            allPartyMembers[i].UpdateCurrentHealth();
            allPartyMembers[i].UpdateCurrentEnergy();
        }
        UpdateAvailableHealths();
    }

    protected void LoadAvailableFighters()
    {
        if (partyMemberNames.Count <= 0)
        {
            for (int i = 0; i < partyMembers.Count; i++)
            {
                partyMembers[i].typeName = "";
                partyMembers[i].UpdateCurrentHealth();
                partyMembers[i].UpdateCurrentEnergy();
            }
            return;
        }
        for (int i = 0; i < partyMemberNames.Count; i++)
        {
            if (partyMemberNames[i].Length <= 0){continue;}
            partyMembers[i].typeName = partyMemberNames[i];
            partyMembers[i].SetCurrentStats(partyMemberStats[i]);
        }
    }
}
