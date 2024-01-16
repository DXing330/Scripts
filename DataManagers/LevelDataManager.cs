using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDataManager : BasicDataManager
{
    // When you start a new game use the default levels.
    public string defaultAllLevels;
    // As you play make a copy and save changes made to the copy.
    // As you play levels might change; terrain/npcs/etc.
    public string allLevels;
    public List<string> allLevelsList;

    public override void NewGame()
    {
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
    }

    public override void Save()
    {
        File.WriteAllText(saveDataPath+"/currentLevels.txt", allLevels);
    }
}
