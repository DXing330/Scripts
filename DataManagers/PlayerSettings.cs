using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettings : BasicDataManager
{
    private string saveDataPath;
    public string allSettings;
    // First setting will be to whether to do a simulated auto battle or not.
    public List<string> settingsList;

    public override void Save()
    {
        saveDataPath = Application.persistentDataPath;
        allSettings = GameManager.instance.ConvertListToString(settingsList);
        File.WriteAllText(saveDataPath+"/PlayerSettings.txt", allSettings);
    }

    public override void Load()
    {
        saveDataPath = Application.persistentDataPath;
        if (File.Exists(saveDataPath+"/PlayerSettings.txt"))
        {
            allSettings = File.ReadAllText(saveDataPath+"/PlayerSettings.txt");
            settingsList = allSettings.Split("|").ToList();
            AdjustSettings();
        }
    }

    private void AdjustSettings()
    {
        // Make sure you don't have any useless settings.
        GameManager.instance.RemoveEmptyListItems(settingsList, 0);
    }

    public string ReturnSetting(int index)
    {
        if (index < 0 || index >= settingsList.Count){return "";}
        return settingsList[index];
    }

    public void SetSetting(int index, string newSetting)
    {
        if (index < 0){return;}
        if (index >= settingsList.Count)
        {
            settingsList.Add(newSetting);
        }
        else
        {
            settingsList[index] = newSetting;
        }
        Save();
    }
}
