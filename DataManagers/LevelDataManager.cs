using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDataManager : BasicDataManager
{
    // When you start a new game use the default levels.
    public DataStringContainer allData;
    public string defaultAllLevels;
    // As you play make a copy and save changes made to the copy.
    // As you play levels might change; terrain/npcs/etc.
    public string allLevels;
    public List<string> allLevelsList;

    [ContextMenu("New Game")]
    public override void NewGame()
    {
        saveDataPath = Application.persistentDataPath;
        allLevels = defaultAllLevels;
        Save();
    }

    public override void Load()
    {
        saveDataPath = Application.persistentDataPath;
        if (File.Exists(saveDataPath+"/currentLevels.txt"))
        {
            allLevels = File.ReadAllText(saveDataPath+"/currentLevels.txt");
        }
        else
        {
            NewGame();
        }
        allLevelsList = allLevels.Split("#").ToList();
        if (allLevelsList.Count < 2)
        {
            allLevels = defaultAllLevels;
            allLevelsList = allLevels.Split("#").ToList();
        }
    }

    public override void Save()
    {
        saveDataPath = Application.persistentDataPath;
        File.WriteAllText(saveDataPath+"/currentLevels.txt", allLevels);
    }
}
