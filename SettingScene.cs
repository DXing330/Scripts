using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingScene : MonoBehaviour
{
    public List<SettingsPanel> allSettingPanels;
    public List<string> playerSettings;

    void Start()
    {
        playerSettings = GameManager.instance.playerSettings.settingsList;
        UpdateSettingPanels();
    }

    public void UpdateSettingPanels()
    {
        if (playerSettings.Count < 1){return;}
        for (int i = 0; i < playerSettings.Count; i++)
        {
            allSettingPanels[i].UpdateSettingsText(playerSettings[i]);
        }
    }
}
