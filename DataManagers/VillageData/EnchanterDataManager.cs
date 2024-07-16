using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnchanterDataManager : BasicDataManager
{
    // To learn enchantment you need to pay mana for research at a chance to learn or destroy and enchanted equipment to learn its enchantment.
    public List<string> allKnownEnchantments;
    public List<string> enchantCosts;
    public void LearnEnchantment(string newEnchantment)
    {
        int indexOf = allKnownEnchantments.IndexOf(newEnchantment);
        if (indexOf < 0)
        {
            allKnownEnchantments.Add(newEnchantment);
            Save();
        }
    }
    protected void SortEnchantments()
    {
        weaponEnchantments.Clear();
        armorEnchantments.Clear();
        accessoryEnchantments.Clear();
        for (int i = 0; i < allKnownEnchantments.Count; i++)
        {
            string[] enchantments = allKnownEnchantments[i].Split('=');
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
        data += GameManager.instance.utility.ConvertListToString(allKnownEnchantments);
        File.WriteAllText(saveDataPath+fileName, data);
    }

    public override void Load()
    {
        saveDataPath = Application.persistentDataPath;
        if (File.Exists(saveDataPath+fileName))
        {
            loadedData = File.ReadAllText(saveDataPath+fileName);
            string[] blocks = loadedData.Split("#");
            allKnownEnchantments = blocks[0].Split("|").ToList();
            SortEnchantments();
        }
        else
        {
            NewGame();
        }
    }
}
