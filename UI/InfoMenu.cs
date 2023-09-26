using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoMenu : MonoBehaviour
{
    public Text playerLevelText;
    public Text goldCoinText;

    void Start()
    {
        UpdateTexts();
    }

    public void UpdateTexts()
    {
        playerLevelText.text = GameManager.instance.playerLevel.ToString();
        goldCoinText.text = GameManager.instance.goldCoins.ToString();
    }
}
