using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GeneralPopulationDataManager : BasicDataManager
{
    // All the village data stuff will keep track of the main village.
    public VillageDataManager villageData;
    public int population;
    public int morale;

    public override void Save()
    {
        saveDataPath = Application.persistentDataPath;
        string data = "";
        data += population+"|"+morale;
        File.WriteAllText(saveDataPath+fileName, data);
    }

    public override void Load()
    {
        saveDataPath = Application.persistentDataPath;
        if (File.Exists(saveDataPath+fileName))
        {
            loadedData = File.ReadAllText(saveDataPath+fileName);
            string[] blocks = loadedData.Split("|");
            population = int.Parse(blocks[0]);
            morale = int.Parse(blocks[1]);
        }
        else
        {
            NewGame();
        }
    }

    public override void NewDay()
    {
        GainPopulation();
    }

    protected void GainPopulation(int amount = 1)
    {
        population += amount;
    }

    public int DeterminePopulationLimit()
    {
        return (villageData.DetermineHousingLimit() + 1)*100;
    }

    public int ReturnTaxIncome()
    {
        return Mathf.Max(0, morale+(population/100));
    }
}
