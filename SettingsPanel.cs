using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SettingsPanel : MonoBehaviour
{
    public TMP_Text onText;
    public TMP_Text offText;
    public int settingIndex;
    public int largeSize;
    public int smallSize;

    public void UpdateSettingsText(string settingSpecifics)
    {
        // 0 means off.
        if (settingSpecifics == "0")
        {
            LargeOffText();
        }
        else
        {
            LargeOnText();
        }
    }

    private void LargeOnText()
    {
        onText.fontSize = largeSize;
        offText.fontSize = smallSize;
    }

    private void LargeOffText()
    {
        onText.fontSize = smallSize;
        offText.fontSize = largeSize;
    }

    public void ChangeSetting(bool on = true)
    {
        if (on)
        {
            GameManager.instance.playerSettings.SetSetting(settingIndex, "1");
            LargeOnText();
        }
        else
        {
            GameManager.instance.playerSettings.SetSetting(settingIndex, "0");
            LargeOffText();
        }
    }
}
