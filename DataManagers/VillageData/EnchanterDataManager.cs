using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnchanterDataManager : BasicDataManager
{
    // To learn enchantment you need to pay mana for research at a chance to learn or destroy and enchanted equipment to learn its enchantment.
    public List<string> knownEnchantments;
    protected void SortEnchantments()
    {
        weaponEnchantments.Clear();
        armorEnchantments.Clear();
        accessoryEnchantments.Clear();
        for (int i = 0; i < knownEnchantments.Count; i++)
        {
            string[] enchantments = knownEnchantments[i].Split('=');
            switch (enchantments[0])
            {
                case "-1":
                    weaponEnchantments.Add(enchantments[1]);
                    break;
                case "0":
                    weaponEnchantments.Add(enchantments[1]);
                    break;
                case "1":
                    armorEnchantments.Add(enchantments[1]);
                    break;
                case "2":
                    accessoryEnchantments.Add(enchantments[1]);
                    break;
            }
        }
    }
    public List<string> weaponEnchantments;
    public List<string> armorEnchantments;
    public List<string> accessoryEnchantments;
    public List<string> enchantEquipment;
    public List<string> enchantDay;
    public List<string> enchantTime;

    public override void Save()
    {
        saveDataPath = Application.persistentDataPath;
        string data = "";
        data += GameManager.instance.ConvertListToString(knownEnchantments)+"#";
        data += GameManager.instance.ConvertListToString(enchantEquipment, ",")+"#";
        data += GameManager.instance.ConvertListToString(enchantDay)+"#";
        data += GameManager.instance.ConvertListToString(enchantTime)+"#";
        File.WriteAllText(saveDataPath+fileName, data);
    }

    public override void Load()
    {
        saveDataPath = Application.persistentDataPath;
        if (File.Exists(saveDataPath+fileName))
        {
            loadedData = File.ReadAllText(saveDataPath+fileName);
            string[] blocks = loadedData.Split("#");
            knownEnchantments = blocks[0].Split("|").ToList();
            enchantEquipment = blocks[1].Split(",").ToList();
            enchantDay = blocks[2].Split("|").ToList();
            enchantTime = blocks[3].Split("|").ToList();
            SortEnchantments();
        }
        else
        {
            NewGame();
        }
    }
}
