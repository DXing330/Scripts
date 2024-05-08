using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleResult : MonoBehaviour
{
    public bool win = true;
    public bool morale = false;
    public TMP_Text resultText;
    public int goldGain = 0;
    public int manaGain = 0;
    public int bloodGain = 0;
    public List<GameObject> rewardObjects;
    public List<StatImageText> rewards;
    public SpriteContainer rewardSprites;
    public GameObject victoryReturnButton;
    public GameObject defeatReturnButton;

    void Start()
    {
        if (GameManager.instance.recentlyWon == 0)
        {
            win = false;
            defeatReturnButton.SetActive(true);
        }
        else
        {
            if (GameManager.instance.recentlyWon == 2){morale = true;}
            goldGain = GameManager.instance.recentlyGainedGold;
            manaGain = GameManager.instance.recentlyGainedMana;
            bloodGain = GameManager.instance.recentlyGainedBlood;
            victoryReturnButton.SetActive(true);
        }
        UpdateResults();
    }

    private void UpdateResults()
    {
        GameManager.instance.utility.DisableAllObjects(rewardObjects);
        int rewardTypes = -1;
        if (win)
        {
            if (!morale)
            {
                resultText.text = "Victory!!!";
                resultText.color = Color.green;
            }
            else
            {
                resultText.text = "Enemy Routed!";
                resultText.color = Color.green;
            }
            if (goldGain > 0)
            {
                rewardTypes++;
                rewardObjects[rewardTypes].SetActive(true);
                rewards[rewardTypes].SetText(goldGain.ToString());
                rewards[rewardTypes].SetSprite(rewardSprites.allSprites[0]);
            }
            if (bloodGain > 0)
            {
                rewardTypes++;
                rewardObjects[rewardTypes].SetActive(true);
                rewards[rewardTypes].SetText(bloodGain.ToString());
                rewards[rewardTypes].SetSprite(rewardSprites.allSprites[1]);
            }
            if (manaGain > 0)
            {
                rewardTypes++;
                rewardObjects[rewardTypes].SetActive(true);
                rewards[rewardTypes].SetText(manaGain.ToString());
                rewards[rewardTypes].SetSprite(rewardSprites.allSprites[2]);
            }
        }
        else
        {
            resultText.text = "Defeat...";
            resultText.color = Color.red;
        }
    }
}
