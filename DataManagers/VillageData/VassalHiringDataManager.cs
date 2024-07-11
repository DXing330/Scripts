using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VassalHiringDataManager : BasicDataManager
{
    // Start of game.
    public VillageDataManager villageData;
    protected string dividingCharacter = "!";
    public int lastHiringDate = 0;
    // Cooldown could decrease.
    public int hiringCooldown = 30;
    // Keep track of who is currently available, they will stay awhile before moving on.
    public List<string> currentlyAvailable = new List<string>();
    public List<string> preNames;
    public List<string> midNames;
    public List<string> postNames;

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
            string[] dataBlocks = loadedData.Split("#");
            lastHiringDate = int.Parse(dataBlocks[0]);
            hiringCooldown = int.Parse(dataBlocks[1]);
            currentlyAvailable = dataBlocks[2].Split(dividingCharacter).ToList();
            GameManager.instance.utility.RemoveEmptyListItems(currentlyAvailable);
        }
        else
        {
            NewGame();
        }
    }

    [ContextMenu("Generate Vassal")]
    protected void GenerateVassal()
    {
        string newVassal = "";
        newVassal += GenerateName()+"|";
        // Need a skill.
        int skillLevel = 1;
        int maxLevel = villageData.ReturnCenterLevel();
        if (maxLevel > skillLevel)
        {
            skillLevel = Random.Range(skillLevel, maxLevel+1);
        }
        int skillPoints = ReturnSkillPointsForSkillLevel(skillLevel);
        List<string> possibleSkills = new List<string>(villageData.possibleBuildings);
        // Remove things that don't require skills.
        possibleSkills.Remove("House");
        int skillType = villageData.buildingData.ReturnBuildingIndex(possibleSkills[Random.Range(0, possibleSkills.Count)]);
        newVassal += skillType+"="+skillPoints+"|";
        // More skilled people are often older with bigger families.
        int familySize = Random.Range(1, 2*skillLevel);
        newVassal += familySize.ToString()+"|";
        // Pay upfront for roughly double how much they'll make over the next period of time, plus an extra for their skill level.
        int cost = skillLevel * hiringCooldown + (Random.Range(0, skillLevel*10));
        // Cost might be decreased if they have a big family, since you'll need to feed their family.
        cost -= Random.Range(0, familySize*10);
        // Later might decrease cost more if your village is good.
        newVassal += cost.ToString();
        currentlyAvailable.Add(newVassal);
    }

    protected string GenerateName()
    {
        string newName = "";
        newName += preNames[Random.Range(0, preNames.Count)];
        newName += midNames[Random.Range(0, midNames.Count)];
        newName += postNames[Random.Range(0, postNames.Count)];
        return newName;
    }

    protected int ReturnSkillPointsForSkillLevel(int level)
    {
        // This will be priced into their cost.
        return (level * level * 100);
    }

    public string ReturnPotentialVassal(int index)
    {
        if (index < 0 || index >= currentlyAvailable.Count)
        {
            return "Currently No Applicants.";
        }
        return currentlyAvailable[index];
    }

    public void StartHiring()
    {
        if (lastHiringDate + hiringCooldown < GameManager.instance.time)
        {
            currentlyAvailable.Clear();
            for (int i = 0; i < villageData.ReturnCenterLevel(); i++)
            {
                GenerateVassal();
            }
            lastHiringDate = GameManager.instance.time;
        }
    }

    public int ReturnHiringCost(int index)
    {
        string[] stats = currentlyAvailable[index].Split("|");
        return int.Parse(stats[3]);
    }


    public string HireVassal(int index)
    {
        string newVassal = currentlyAvailable[index];
        currentlyAvailable.RemoveAt(index);
        return newVassal;
    }
}
