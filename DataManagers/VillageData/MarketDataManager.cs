using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketDataManager : BasicDataManager
{
    public VillageDataManager villageData;
    public string fileName = "/market.txt";
    public string newGameData;
    // Affects the open market.
    public int marketLevel;
    public int marketInvestment;
    // Affects comissionable equipment and how many smiths you have.
    public int smithLevel;
    public int smithInvestment;
    // Randomly new equipment may be available.
    public int lastUpdateDay;
    // If you buy out the equipment it will take some time to restock.
    public List<string> availableEquipment;
    // Every smith can work on one equipment at a time.
    public List<string> comissionedEquipment;
    public List<string> comissionedDay;
    public List<string> comissionedTime;

    [ContextMenu("New Game")]
    public override void NewGame()
    {
        saveDataPath = Application.persistentDataPath;
        if (File.Exists(saveDataPath+fileName))
        {
            File.Delete (saveDataPath+fileName);
        }
        File.WriteAllText(saveDataPath+fileName, newGameData);
        Load();
    }

    public override void Save()
    {
        saveDataPath = Application.persistentDataPath;
        string data = "";
        data += marketLevel+"#"+marketInvestment+"#"+smithLevel+"#"+smithInvestment+"#"+lastUpdateDay+"#";
        data += GameManager.instance.ConvertListToString(availableEquipment)+"#";
        data += GameManager.instance.ConvertListToString(comissionedEquipment)+"#";
        data += GameManager.instance.ConvertListToString(comissionedDay)+"#";
        data += GameManager.instance.ConvertListToString(comissionedTime)+"#";
        File.WriteAllText(saveDataPath+fileName, data);
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
            smithLevel = int.Parse(blocks[2]);
            smithInvestment = int.Parse(blocks[3]);
            lastUpdateDay = int.Parse(blocks[4]);
            availableEquipment = blocks[5].Split(",").ToList();
            comissionedEquipment = blocks[6].Split(",").ToList();
            comissionedDay = blocks[7].Split(",").ToList();
            comissionedTime = blocks[8].Split(",").ToList();
            GameManager.instance.RemoveEmptyListItems(availableEquipment,0);
            GameManager.instance.RemoveEmptyListItems(comissionedEquipment,0);
            GameManager.instance.RemoveEmptyListItems(comissionedDay,0);
            GameManager.instance.RemoveEmptyListItems(comissionedTime,0);
        }
        else
        {
            NewGame();
        }
    }

    public void Invest(int amount = 1, bool market = true)
    {
        if (market){marketInvestment += amount;}
        else{smithInvestment += amount;}
        CheckInvestmentThreshold(market);
    }

    protected void CheckInvestmentThreshold(bool market = true)
    {
        if (market)
        {
            if (marketInvestment > Mathf.Pow(2, marketLevel))
            {
                marketInvestment = 0;
                marketLevel++;
            }
        }
        else
        {
            if (smithInvestment > Mathf.Pow(2, smithLevel))
            {
                smithInvestment = 0;
                smithLevel++;
            }
        }
    }

    // Generate equipment with rarity based on the market level.
    // Can only generate basic human usable equipment.

    // Can comission rarer equipment for other creatures.
    // Have some degree of choice over special effects.
}
