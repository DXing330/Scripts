using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyDataManager : BasicDataManager
{
    // Extra party members, excluding player and familiar.
    public List<PlayerActor> partyMembers;
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
        if (pcenergy.Count <= 0){return;}
        // Update the PC health if possible.
        for (int i = 0; i < pcenergy.Count; i++)
        {
            allPartyMembers[i].UpdateCurrentHealth(int.Parse(pchealths[i]));
            allPartyMembers[i].UpdateCurrentEnergy(int.Parse(pcenergy[i]));
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
    public List<string> pchealths = new List<string>(0);
    public List<string> pcenergy = new List<string>(0);
    public List<string> availableFighters = new List<string>(0);
    public List<string> fighterHealths = new List<string>(0);
    public List<string> fighterEnergy = new List<string>(0);
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
        pchealths.Clear();
        pcenergy.Clear();
        availableFighters.Clear();
        fighterHealths.Clear();
        fighterEnergy.Clear();
        List<int> deadMembers = new List<int>();
        for (int i = 0; i < allPartyMembers.Count; i++)
        {
            if (allPartyMembers[i].typeName == "Player" || allPartyMembers[i].typeName == "Familiar")
            {
                pchealths.Add(Mathf.Max(1, allPartyMembers[i].ReturnCurrentHealth()).ToString());
                pcenergy.Add(Mathf.Max(1, allPartyMembers[i].ReturnCurrentEnergy()).ToString());
                continue;
            }
            int currentHealth = allPartyMembers[i].ReturnCurrentHealth();
            int currentEnergy = allPartyMembers[i].ReturnCurrentEnergy();
            if (currentHealth <= 0)
            {
                deadMembers.Insert(0, i);
                continue;
            }
            availableFighters.Add(allPartyMembers[i].typeName);
            fighterHealths.Add(currentHealth.ToString());
            fighterEnergy.Add(currentEnergy.ToString());
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
        if (File.Exists(saveDataPath+fileName))
        {
            File.Delete (saveDataPath+fileName);
        }
        availableFighters.Clear();
        fighterHealths.Clear();
        LoadAvailableFighters();
        GetAllPartyMembers();
        Save();
    }

    public override void Save()
    {
        UpdateAvailableHealths();
        saveDataPath = Application.persistentDataPath;
        string fighterData = GameManager.instance.utility.ConvertListToString(availableFighters);
        fighterData += "#"+GameManager.instance.utility.ConvertListToString(fighterHealths);
        fighterData += "#"+GameManager.instance.utility.ConvertListToString(fighterEnergy);
        fighterData += "#"+GameManager.instance.utility.ConvertListToString(pchealths);
        fighterData += "#"+GameManager.instance.utility.ConvertListToString(pcenergy);
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
                availableFighters = dataBlocks[0].Split("|").ToList();
                fighterHealths = dataBlocks[1].Split("|").ToList();
                fighterEnergy = dataBlocks[2].Split("|").ToList();
                pchealths = dataBlocks[3].Split("|").ToList();
                pcenergy = dataBlocks[4].Split("|").ToList();
            }
            GameManager.instance.utility.RemoveEmptyListItems(availableFighters);
            GameManager.instance.utility.RemoveEmptyListItems(fighterHealths, 0);
            GameManager.instance.utility.RemoveEmptyListItems(fighterEnergy, 0);
            GameManager.instance.utility.RemoveEmptyListItems(pchealths, 0);
            GameManager.instance.utility.RemoveEmptyListItems(pcenergy, 0);
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
        if (availableFighters.Count <= 0)
        {
            for (int i = 0; i < Mathf.Min(availableFighters.Count, partyMembers.Count); i++)
            {
                partyMembers[i].typeName = "";
                partyMembers[i].UpdateCurrentHealth();
                partyMembers[i].UpdateCurrentEnergy();
            }
            return;
        }
        for (int i = 0; i < Mathf.Min(availableFighters.Count, partyMembers.Count); i++)
        {
            if (availableFighters[i].Length <= 0){continue;}
            partyMembers[i].typeName = availableFighters[i];
            partyMembers[i].SideCharacterUpdateStats();
            partyMembers[i].UpdateCurrentHealth(int.Parse(fighterHealths[i]));
            partyMembers[i].UpdateCurrentEnergy(int.Parse(fighterEnergy[i]));
        }
    }
}
