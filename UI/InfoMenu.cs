using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoMenu : MonoBehaviour
{
    public Text playerLevelText;
    public Text goldCoinText;
    public Text manaText;
    public Text bloodText;
    public GameObject levelUpButton;

    void Start()
    {
        UpdateInfoTexts();
    }

    public void UpdateInfoTexts()
    {
        playerLevelText.text = GameManager.instance.playerLevel.ToString();
        goldCoinText.text = GameManager.instance.goldCoins.ToString();
        manaText.text = GameManager.instance.manaCrystals.ToString();
        bloodText.text = GameManager.instance.bloodCrystals.ToString();
        UpdateLevelButton();
    }

    public void UpdateLevelButton()
    {
        levelUpButton.SetActive(false);
        int currentLevel = GameManager.instance.playerLevel;
        if (GameManager.instance.CheckCost(0, currentLevel * currentLevel))
        {
            levelUpButton.SetActive(true);
        }
    }

    public void TryToLevel()
    {
        //GameManager.instance.LevelUp();
    }
}
