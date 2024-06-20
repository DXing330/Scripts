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
    // Roughly 81*125, based on ACKs 125 families per wilderness tile, we start with 81 tiles and are basically in the wilderness.
    //protected int maxPopulationLimit = 10000;
    // Based on how many houses you have and their levels.
    //public int populationLimit;
    // Based on your actions as a ruler.
    public int morale;
    // Should decrease over a monthly basis, you need to keep investing to keep up growth.
    //public int investment;
    //public int bonusGrowthRate = 0;
    // Based on ACK's services + taxes income.
    //public int averageIncome = 6;
        // Don't hard code it, base it on factors from the village.
        // Add incomes from buildings.

    // Based on ACK's wilderness garrison.
    //public int averageExpenses = 4;
        // Don't hard code it, base it on factors from the village.
        // Add expenses for building maintences.

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

    protected void GainPopulation(int amount = 0)
    {
        if (amount == 0)
        {
            if (population >= DeterminePopulationLimit()){return;}
            // Gain at least 1 population unless morale is low.
            // Can gain more at larger populations.
            population += 1 + morale + (GameManager.instance.utility.FloorLog(population, 3))/6;
        }
        else{population += amount;}
    }

    public int DeterminePopulationLimit()
    {
        return (villageData.DetermineHousingLimit() + 1)*100;
    }

    public int ReturnTaxIncome()
    {
        return (population/100);
    }
}
