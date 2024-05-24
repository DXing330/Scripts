using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VassalDataManager : BasicDataManager
{
    public VillageDataManager villageData;

    public List<string> vassals;
    public List<string> familySizes;
    public List<string> morales;
    public List<string> skills;
    public List<string> locations;
    public List<string> plots;
    public List<string> plotSpecifics;
    public List<string> plotTimings;
    public List<string> timingSpecifics;

    public override void Save()
    {
        saveDataPath = Application.persistentDataPath;
        string data = "";
        data += GameManager.instance.ConvertListToString(vassals)+"#";
        data += GameManager.instance.ConvertListToString(familySizes)+"#";
        data += GameManager.instance.ConvertListToString(morales)+"#";
        data += GameManager.instance.ConvertListToString(skills)+"#";
        data += GameManager.instance.ConvertListToString(locations)+"#";
        data += GameManager.instance.ConvertListToString(plots)+"#";
        data += GameManager.instance.ConvertListToString(plotSpecifics)+"#";
        data += GameManager.instance.ConvertListToString(plotTimings)+"#";
        data += GameManager.instance.ConvertListToString(timingSpecifics)+"#";
        File.WriteAllText(saveDataPath+fileName, data);
    }

    public override void Load()
    {
        saveDataPath = Application.persistentDataPath;
        if (File.Exists(saveDataPath+fileName))
        {
            loadedData = File.ReadAllText(saveDataPath+fileName);
            string[] blocks = loadedData.Split("#");
            vassals = blocks[0].Split("|").ToList();
            GameManager.instance.RemoveEmptyListItems(vassals);
            familySizes = blocks[1].Split("|").ToList();
            GameManager.instance.RemoveEmptyListItems(familySizes,0);
            morales = blocks[2].Split("|").ToList();
            GameManager.instance.RemoveEmptyListItems(morales,0);
            skills = blocks[3].Split("|").ToList();
            GameManager.instance.RemoveEmptyListItems(skills);
            locations = blocks[4].Split("|").ToList();
            GameManager.instance.RemoveEmptyListItems(locations,0);
        }
        else
        {
            NewGame();
        }
    }

    public void GainVassal(string newVassal)
    {
        string[] stats = newVassal.Split("|");
        vassals.Add(stats[0]);
        skills.Add(stats[1]);
        familySizes.Add(stats[2]);
        // They start in the center where they were hired.
        locations.Add(villageData.buildings.buildingLocations[0]);
        // They start off with basic loyalty.
        morales.Add("6");
    }

    protected void LoseVassal(int index)
    {
        vassals.RemoveAt(index);
        familySizes.RemoveAt(index);
        locations.RemoveAt(index);
        morales.RemoveAt(index);
        skills.RemoveAt(index);
    }

    public void DecreaseVassalMorale()
    {
        for (int i = 0; i < morales.Count; i++)
        {
            int newMorale = int.Parse(morales[i])-1;
            // Bigger families have a harder time moving away.
            if (newMorale <= -(int.Parse(familySizes[i]))){LoseVassal(i);}
            else{morales[i] = (newMorale).ToString();}
        }
    }

    public void IncreaseVassalMorale(int max = 6)
    {
        for (int i = 0; i < morales.Count; i++)
        {
            int newMorale = int.Parse(morales[i])+1;
            if (newMorale > max){newMorale = max;}
            {morales[i] = (newMorale).ToString();}
        }
    }

    public void UpdateFamilySizes()
    {
        int size = 1;
        for (int i = 0; i < familySizes.Count; i++)
        {
            size = int.Parse(familySizes[i]);
            size += DetermineFamilySizeChange(size);
            familySizes[i] = size.ToString();
        }
    }

    protected int DetermineFamilySizeChange(int currentSize)
    {
        // Rush to start a family.
        if (currentSize < 4)
        {
            return 1;
        }
        // Slow down a little.
        else if (currentSize > 4 && currentSize < 8)
        {
            return Random.Range(0,1);
        }
        // Too many means not enough care, easier to die.
        else
        {
            return Random.Range(-1,1);
        }
    }

    public int DetermineFoodConsumption()
    {
        int consumption = 0;
        for (int i = 0; i < familySizes.Count; i++)
        {
            if (familySizes[i].Length <= 0){continue;}
            consumption += int.Parse(familySizes[i]);
        }
        return consumption;
    }
}
