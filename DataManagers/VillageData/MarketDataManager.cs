using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketDataManager : BasicDataManager
{
    public int basePrice = 50;
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
    public List<string> currentAvailable;
    public List<string> currentQuantity;
    public List<string> currentPrices;
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
        data += GameManager.instance.ConvertListToString(currentAvailable, ",")+"#";
        data += GameManager.instance.ConvertListToString(currentQuantity)+"#";
        data += GameManager.instance.ConvertListToString(currentPrices)+"#";
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
            currentAvailable = blocks[3].Split(",").ToList();
            currentQuantity = blocks[4].Split("|").ToList();
            currentPrices = blocks[4].Split("|").ToList();
            GameManager.instance.utility.RemoveEmptyListItems(currentAvailable, 0);
            GameManager.instance.utility.RemoveEmptyListItems(currentQuantity, 0);
            GameManager.instance.utility.RemoveEmptyListItems(currentPrices, 0);
        }
        else
        {
            NewGame();
        }
        GameManager.instance.utility.DataManagerLoad(ownedBusinesses);
    }

    public override void NewDay()
    {
        GenerateRandomEquipment();
    }

    // Generate equipment with rarity based on the market level.
    protected void GenerateRandomEquipment()
    {
        int timeDifference = 0;
        // Market updates daily I guess?
        // Check the time first to see if you should generate equipment.
        if (GameManager.instance.time > lastUpdateDay)
        {
            timeDifference = GameManager.instance.time - lastUpdateDay;
            lastUpdateDay = GameManager.instance.time;
        }
        else {return;}
        int typeRng = 0;
        int levelRng = 0;
        int sqrtLevel = GameManager.instance.utility.FloorSqrt(marketLevel);
        for (int i = 0; i < timeDifference; i++)
        {
            typeRng = Random.Range(0, regularEquipment.Count);
            levelRng = Random.Range(0, Random.Range(1,sqrtLevel+1));
            string newEquip = equipmentGenerator.GenerateEquipment(levelRng, regularEquipment[typeRng]);
            int newPrice = GeneratePriceForEquipment(newEquip, levelRng);
            for (int j = 0; j < Random.Range(1,sqrtLevel+1); j++)
            {
                int indexOf = currentAvailable.IndexOf(newEquip);
                if (indexOf >= 0)
                {
                    AdjustQuantity(indexOf);
                    AdjustPrice(indexOf, newPrice);
                }
                else
                {
                    currentAvailable.Add(newEquip);
                    currentQuantity.Add("1");
                    currentPrices.Add(newPrice.ToString());
                }
            }
        }
    }

    protected int GeneratePriceForEquipment(string equip, int rarity = 0, int volatility = 0)
    {
        int price = 0;
        // Increase price for rarity.
        price += (rarity+1)*(rarity+1)*basePrice;
        // Increase price for special abilities.
        string[] equipStats = equip.Split("|");
        // 0-1 abilities is free.
        int baseAbilities = GameManager.instance.utility.CountOccurencesOfCharInString(equipStats[5], ',')+GameManager.instance.utility.CountOccurencesOfCharInString(equipStats[6], ',');
        if (baseAbilities <= 0)
        {
            if (equipStats[5].Length > 1 || equipStats[6].Length > 1){baseAbilities++;}
        }
        int rareAbilities = GameManager.instance.utility.CountOccurencesOfCharInString(equipStats[7], ',')+GameManager.instance.utility.CountOccurencesOfCharInString(equipStats[8], ',');
        if (rareAbilities <= 0)
        {
            if (equipStats[7].Length > 1){rareAbilities++;}
            if (equipStats[8].Length > 1){rareAbilities++;}
        }
        int totalAbilites = baseAbilities+(2*rareAbilities);
        price += totalAbilites*basePrice*(rarity+1);
        if (volatility > 0)
        {
            int maxVolatility = Mathf.Max(volatility, price*volatility/100);
            price += Random.Range(-maxVolatility, maxVolatility);
        }
        return price;
    }

    protected void AdjustQuantity(int index, int quantity = 1)
    {
        if (index >= currentQuantity.Count || index < 0){return;}
        int newQuantity = int.Parse(currentQuantity[index])+quantity;
        if (newQuantity <= 0)
        {
            currentAvailable.RemoveAt(index);
            currentQuantity.RemoveAt(index);
            currentPrices.RemoveAt(index);
            return;
        }
        currentQuantity[index] = (newQuantity).ToString();
    }

    protected void AdjustPrice(int index, int price = -1)
    {
        if (index >= currentPrices.Count || index < 0 || price <= 0){return;}
        int averagePrice = (int.Parse(currentPrices[index])+price)/2;
        currentPrices[index] = averagePrice.ToString();
    }

    public void BuyEquipment(int index)
    {
        // Check if you have the money.
        // Remove the money.
        // Add the equipment.
        // Remove it from the market.
    }
}