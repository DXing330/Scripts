using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketDataManager : BasicDataManager
{
    public VillageDataManager villageData;
    // Generates equipment for the open market.
    public EquipmentGenerator equipmentGenerator;
    // What types of equipment are always available.
    public List<string> regularEquipment;
    // Affects the quality of equipment on the market.
    public int marketLevel;
    public int marketInvestment;
    // Randomly new equipment may be available.
    public int lastUpdateDay;
    // If you buy out the equipment it will take some time to restock.
    public List<string> availableEquipment;
    public List<string> equipmentPrices;
    // Affects comissionable equipment and how many smiths you have.
    // Every smith can work on one equipment at a time.
    public List<BasicDataManager> ownedBusinesses;
    public ForgeDataManager forgeData;
    public EnchanterDataManager enchanterData;

    public override void NewGame()
    {
        base.NewGame();
        GameManager.instance.utility.DataManagerNewGame(ownedBusinesses);
    }

    public override void Save()
    {
        saveDataPath = Application.persistentDataPath;
        string data = "";
        data += marketLevel+"#"+marketInvestment+"#"+lastUpdateDay+"#";
        data += GameManager.instance.ConvertListToString(availableEquipment, ",")+"#";
        data += GameManager.instance.ConvertListToString(equipmentPrices)+"#";
        File.WriteAllText(saveDataPath+fileName, data);
        GameManager.instance.utility.DataManagerSave(ownedBusinesses);
    }

    public override void Load()
    {
        saveDataPath = Application.persistentDataPath;
        if (File.Exists(saveDataPath+fileName))
        {
            loadedData = File.ReadAllText(saveDataPath+fileName);
            string[] blocks = loadedData.Split("#");
            marketLevel = int.Parse(blocks[0]);
            marketInvestment = int.Parse(blocks[1]);
            lastUpdateDay = int.Parse(blocks[2]);
            availableEquipment = blocks[3].Split(",").ToList();
            equipmentPrices = blocks[4].Split("|").ToList();
            GameManager.instance.utility.RemoveEmptyListItems(availableEquipment, 0);
            GameManager.instance.utility.RemoveEmptyListItems(equipmentPrices, 0);
        }
        else
        {
            NewGame();
        }
        GameManager.instance.utility.DataManagerLoad(ownedBusinesses);
    }

    // Generate equipment with rarity based on the market level.
    public void GenerateRandomEquipment()
    {
        // Market updates daily I guess?
        // Check the time first to see if you should generate equipment.
        if (GameManager.instance.time > lastUpdateDay)
        {
            lastUpdateDay = GameManager.instance.time;
            availableEquipment.Clear();
            equipmentPrices.Clear();
        }
        else {return;}
        int typeRng = 0;
        int levelRng = 0;
        for (int i = 0; i < marketLevel; i++)
        {
            typeRng = Random.Range(0, regularEquipment.Count);
            levelRng = Random.Range(0, GameManager.instance.utility.FloorSqrt(marketLevel));
            availableEquipment.Add(equipmentGenerator.GenerateEquipment(levelRng, regularEquipment[typeRng]));
            // Need to generate a cost for the equipment.
            equipmentPrices.Add(((levelRng+1)*(levelRng+1)*50 + Random.Range(-50,50)).ToString());
        }
    }

    public void BuyEquipment(int index)
    {
        // Check if you have the money.
        // Remove the money.
        // Add the equipment.
        // Remove it from the market.
    }
}