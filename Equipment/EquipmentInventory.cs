using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentInventory : BasicDataManager
{
    private string saveDataPath;
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
    }

    private void SortEquip(string equip, int equipType)
    {
        switch (equipType)
        {
            case 0:
                allWeapons.Add(equip);
                break;
            case 0:
                allArmors.Add(equip);
                break;
            case 0:
                allHelmets.Add(equip);
                break;
            case 0:
                allBoots.Add(equip);
                break;
            case 0:
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
        allEquippedEquipment = blocks[1].Split("|").ToList();
        LoadEquipSets();
    }

    private void LoadEquipSets()
    {
        for (int i = 0; i < allEquippedEquipment.Count; i++)
        {
            GameManager.instance.playerActors[i].LoadAllEquipped(allEquippedEquipment[i]);
        }
    }

    public void GainEquipment(string newEquipment)
    {
        allEquipment.Add(newEquipment);
    }

    public void LoseEquipment(int index)
    {
        allEquipment.RemoveAt(index);
    }

    public void EquipToActor(int actorIndex, int equipIndex)
    {
        // Make sure the equipment exists.
        if (equipIndex >= allEquipment.Count){return;}
        GameManager.instance.equipData.LoadEquipData(equipment, allEquipment[equipIndex]);
        PlayerActor actorToEquip = GameManager.instance.playerActors[actorIndex];
        // Make sure the equipment can be equipped.
        if (equipment.possibleUsers != "All"){return;}
        // Equip it to the appropriate slot.
        switch (equipment.equipType)
        {
            case 0:
                actorToEquip.equipWeapon = equipment.equipName;
                break;
            case 1:
                actorToEquip.equipArmor = equipment.equipName;
                break;
            case 2:
                actorToEquip.equipHelmet = equipment.equipName;
                break;
            case 3:
                actorToEquip.equipBoots = equipment.equipName;
                break;
            case 4:
                actorToEquip.equipAccessory = equipment.equipName;
                break;
        }
        actorToEquip.UpdateEquipment();
    }
}
