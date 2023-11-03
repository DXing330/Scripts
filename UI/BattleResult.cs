using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleResult : MonoBehaviour
{
    public bool win = true;
    public TMP_Text resultText;
    public int goldGain = 0;
    public int manaGain = 0;
    public int bloodGain = 0;
    public TMP_Text goldText;
    public TMP_Text manaText;
    public TMP_Text bloodText;
    public GameObject rewardGold;
    public GameObject rewardMana;
    public GameObject rewardBlood;

    void Start()
    {
        if (GameManager.instance.recentlyWon == 0)
        {
            win = false;
        }
        else
        {
            goldGain = GameManager.instance.recentlyGainedGold;
            manaGain = GameManager.instance.recentlyGainedMana;
            bloodGain = GameManager.instance.recentlyGainedBlood;
        }
        UpdateResults();
    }

    private void UpdateResults()
    {
        if (win)
        {
            resultText.text = "Victory!!!";
            resultText.color = Color.green;
            goldText.text = goldGain.ToString();
            if (goldGain <= 0)
            {
                rewardGold.SetActive(false);
            }
            manaText.text = manaGain.ToString();
            if (manaGain <= 0)
            {
                rewardMana.SetActive(false);
            }
            bloodText.text = bloodGain.ToString();
            if (bloodGain <= 0)
            {
                rewardBlood.SetActive(false);
            }
        }
        else
        {
            resultText.text = "Defeat...";
            resultText.color = Color.red;
            rewardGold.SetActive(false);
            rewardMana.SetActive(false);
            rewardBlood.SetActive(false);
        }
    }
}
