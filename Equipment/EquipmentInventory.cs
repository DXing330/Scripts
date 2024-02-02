using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentInventory : BasicDataManager
{
    /*
    private string loadedData;
    public Equipment equipment;
    public List<string> allEquipment;
    public List<string> allEquippedEquipment;
    public List<string> allWeapons;
    public List<string> allArmors;
    public List<string> allHelmets;
    public List<string> allBoots;
    public List<string> allAccessories;

    public void SortEquipmentIntoLists()
    {
        allWeapons.Clear();
        allArmors.Clear();
        allHelmets.Clear();
        allBoots.Clear();
        allAccessories.Clear();
        for (int i = 0; i < allEquipment.Count; i++)
        {
            if (allEquipment[i].Length < 6){continue;}
            GameManager.instance.equipData.LoadEquipData(equipment, allEquipment[i]);
            SortEquip(equipment.equipName, equipment.equipType);
        }
    }

    private void SortEquip(string equip, int equipType)
    {
        switch (equipType)
        {
            case 0:
                allWeapons.Add(equip);
                break;
            case 1:
                allArmors.Add(equip);
                break;
            case 2:
                allHelmets.Add(equip);
                break;
            case 3:
                allBoots.Add(equip);
                break;
            case 4:
                allAccessories.Add(equip);
                break;
        }
    }

    public override void NewGame()
    {
        allEquipment.Clear();
        allEquippedEquipment.Clear();
        Save();
        Load();
    }

    public override void Save()
    {
        saveDataPath = Application.persistentDataPath;
        SaveEquipSets();
        string data = "";
        data += GameManager.instance.ConvertListToString(allEquipment)+"#";
        data += GameManager.instance.ConvertListToString(allEquippedEquipment)+"#";
        File.WriteAllText(saveDataPath+"/equipInventoryData.txt", data);
    }

    private void SaveEquipSets()
    {
        allEquippedEquipment.Clear();
        for (int i = 0; i < GameManager.instance.playerActors.Count; i++)
        {
            allEquippedEquipment.Add(GameManager.instance.playerActors[i].ReturnAllEquipped());
        }
    }

    public override void Load()
    {
        saveDataPath = Application.persistentDataPath;
        loadedData = File.ReadAllText(saveDataPath+"/equipInventoryData.txt");
        string[] blocks = loadedData.Split("#");
        allEquipment = blocks[0].Split("|").ToList();
        SortEquipmentIntoLists();
        allEquippedEquipment = blocks[1].Split("|").ToList();
        LoadEquipSets();
    }

    // Need to make sure that the party loads first for this to work.
    private void LoadEquipSets()
    {
        int max = Mathf.Min(allEquippedEquipment.Count, GameManager.instance.playerActors.Count);
        for (int i = 0; i < max; i++)
        {
            GameManager.instance.playerActors[i].LoadAllEquipped(allEquippedEquipment[i]);
        }
    }

    public void GainEquipment(string newEquipment)
    {
        allEquipment.Add(newEquipment);
        SortEquipmentIntoLists();
    }

    public void LoseEquipment(int index)
    {
        allEquipment.RemoveAt(index);
        SortEquipmentIntoLists();
    }

    public void EquipToActor(int actorIndex, int equipType, int equipIndex)
    {
        PlayerActor actorToEquip = GameManager.instance.playerActors[actorIndex];
        // Make sure the equipment exists.
        switch (equipType)
        {
            case 0:
                if (equipIndex >= allWeapons.Count){return;}
                GameManager.instance.equipData.LoadEquipData(equipment, allWeapons[equipIndex]);
                break;
            case 1:
                if (equipIndex >= allArmors.Count){return;}
                GameManager.instance.equipData.LoadEquipData(equipment, allArmors[equipIndex]);
                break;
            case 2:
                if (equipIndex >= allHelmets.Count){return;}
                GameManager.instance.equipData.LoadEquipData(equipment, allHelmets[equipIndex]);
                break;
            case 3:
                if (equipIndex >= allBoots.Count){return;}
                GameManager.instance.equipData.LoadEquipData(equipment, allBoots[equipIndex]);
                break;
            case 4:
                if (equipIndex >= allAccessories.Count){return;}
                GameManager.instance.equipData.LoadEquipData(equipment, allAccessories[equipIndex]);
                break;
        }
        // Make sure the equipment can be equipped.
        if (equipment.possibleUsers != "All"){return;}
        // Equip it to the appropriate slot.
        switch (equipment.equipType)
        {
            case 0:
                if (actorToEquip.equipWeapon != "none")
                {
                    GainEquipment(actorToEquip.equipWeapon);
                }
                actorToEquip.equipWeapon = equipment.equipName;
                break;
            case 1:
                if (actorToEquip.equipArmor != "none")
                {
                    GainEquipment(actorToEquip.equipArmor);
                }
                actorToEquip.equipArmor = equipment.equipName;
                break;
            case 2:
                if (actorToEquip.equipHelmet != "none")
                {
                    GainEquipment(actorToEquip.equipHelmet);
                }
                actorToEquip.equipHelmet = equipment.equipName;
                break;
            case 3:
                if (actorToEquip.equipBoots != "none")
                {
                    GainEquipment(actorToEquip.equipBoots);
                }
                actorToEquip.equipBoots = equipment.equipName;
                break;
            case 4:
                if (actorToEquip.equipAccessory != "none")
                {
                    GainEquipment(actorToEquip.equipAccessory);
                }
                actorToEquip.equipAccessory = equipment.equipName;
                break;
        }
        // One equipment per character.
        int index = allEquipment.IndexOf(equipment.equipName);
        LoseEquipment(index);
        //actorToEquip.UpdateEquipment();
        SaveEquipSets();
    }

    public void UnequipFromActor(int actorIndex, int equipType)
    {
        PlayerActor actorToEquip = GameManager.instance.playerActors[actorIndex];
        switch (equipType)
        {
            case 0:
                if (actorToEquip.equipWeapon != "none")
                {
                    GainEquipment(actorToEquip.equipWeapon);
                }
                actorToEquip.equipWeapon = "none";
                break;
            case 1:
                if (actorToEquip.equipArmor != "none")
                {
                    GainEquipment(actorToEquip.equipArmor);
                }
                actorToEquip.equipArmor = "none";
                break;
            case 2:
                if (actorToEquip.equipHelmet != "none")
                {
                    GainEquipment(actorToEquip.equipHelmet);
                }
                actorToEquip.equipHelmet = "none";
                break;
            case 3:
                if (actorToEquip.equipBoots != "none")
                {
                    GainEquipment(actorToEquip.equipBoots);
                }
                actorToEquip.equipBoots = "none";
                break;
            case 4:
                if (actorToEquip.equipAccessory != "none")
                {
                    GainEquipment(actorToEquip.equipAccessory);
                }
                actorToEquip.equipAccessory = "none";
                break;
        }
        SaveEquipSets();
    }*/
}
