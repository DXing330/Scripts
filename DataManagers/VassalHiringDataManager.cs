using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VassalHiringDataManager : BasicDataManager
{
    // Start of game.
    protected string dividingCharacter = "!";
    public string allData;
    public int lastHiringDate = 0;
    // Cooldown could decrease.
    public int hiringCooldown = 30;
    // Keep track of who is currently available, they will stay awhile before moving on.
    public List<string> currentlyAvailable = new List<string>();

    public override void NewGame()
    {
        saveDataPath = Application.persistentDataPath;
        if (File.Exists(saveDataPath+"/hiringvassals.txt"))
        {
            File.Delete (saveDataPath+"/hiringvassals.txt");
        }
        lastHiringDate = 0;
        hiringCooldown = 30;
        currentlyAvailable = new List<string>();
        Save();
        Load();
    }

    public override void Save()
    {
        saveDataPath = Application.persistentDataPath;
        string data = "";
        data += lastHiringDate+"#"+hiringCooldown+"#";
        data += GameManager.instance.utility.ConvertListToString(currentlyAvailable, dividingCharacter);
        File.WriteAllText(saveDataPath+"/hiringvassals.txt", data);
    }

    public override void Load()
    {
        saveDataPath = Application.persistentDataPath;
        if (File.Exists(saveDataPath+"/hiringvassals.txt"))
        {
            loadedData = File.ReadAllText(saveDataPath+"/hiringvassals.txt");
            allData = loadedData;
            string[] dataBlocks = allData.Split("#");
            lastHiringDate = int.Parse(dataBlocks[0]);
            hiringCooldown = int.Parse(dataBlocks[1]);
            currentlyAvailable = dataBlocks[2].Split(dividingCharacter).ToList();
        }
        else
        {
            NewGame();
        }
    }

    protected void GenerateVassal()
    {
        string newVassal = "";
        // Need a name.
            // Random.
        // Need a cost.
            // Based on skills.
        // Need some skills.
            // Based on what is available in the village.
        // Need a family size.
            // Random.
        currentlyAvailable.Add(newVassal);
    }

    public string ReturnPotentialVassal(int index)
    {
        if (index < 0 || index >= currentlyAvailable.Count)
        {
            return "Currently No Applicants.";
        }
        return currentlyAvailable[index];
    }

    protected void StartHiring()
    {
        // Clear the previous vassals away if needed.
        if (lastHiringDate + hiringCooldown < GameManager.instance.time)
        {
            currentlyAvailable.Clear();
        }
        // Generate vassals based on stuff.
        // Max is the level of your city center.
    }
}
