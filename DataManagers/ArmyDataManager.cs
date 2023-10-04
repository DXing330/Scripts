using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyDataManager : MonoBehaviour
{
    private string saveDataPath;
    private string loadedData;
    public int maxArmySize = 9;
    public List<string> armyFormation = new List<string>(9);
    public List<string> availableFighters = new List<string>(0);

    private void AdjustLists()
    {
        GameManager.instance.RemoveEmptyListItems(availableFighters);
        for (int i = 0; i < armyFormation.Count; i++)
        {
            if (armyFormation[i].Length < 3)
            {
                armyFormation[i] = "none";
            }
        }
    }

    public void NewGame()
    {
        armyFormation.Clear();
        for (int i = 0; i < maxArmySize; i++)
        {
            armyFormation.Add("none");
        }
        armyFormation[0] = "Familiar";
        armyFormation[1] = "Player";
        availableFighters.Clear();
        Save();
    }

    public void Save()
    {
        saveDataPath = Application.persistentDataPath;
        string armyData = GameManager.instance.ConvertListToString(armyFormation);
        File.WriteAllText(saveDataPath+"/armyData.txt", armyData);
        string fighterData = GameManager.instance.ConvertListToString(availableFighters);
        File.WriteAllText(saveDataPath+"/fighters.txt", fighterData);
    }

    public void Load()
    {
        saveDataPath = Application.persistentDataPath;
        loadedData = File.ReadAllText(saveDataPath+"/armyData.txt");
        armyFormation = loadedData.Split("|").ToList();
        loadedData = File.ReadAllText(saveDataPath+"/fighters.txt");
        availableFighters = loadedData.Split("|").ToList();
        if (armyFormation.Count < maxArmySize)
        {
            NewGame();
        }
        AdjustLists();
    }

    public void GainFighter(string fighterName)
    {
        availableFighters.Add(fighterName);
    }
}
