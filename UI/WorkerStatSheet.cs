using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorkerStatSheet : MonoBehaviour
{
    public VillageDataManager villageData;
    public int currentIndex = 0;
    public void ChangeIndex(bool right = true)
    {
        int last = villageData.workers.Count;
        if (right)
        {
            if (currentIndex + 1 < last){currentIndex++;}
            else {currentIndex = 0;}
        }
        else
        {
            if (currentIndex > 0){currentIndex--;}
            else {currentIndex = last - 1;}
        }
        UpdateWorkerStats();
    }
    public TMP_Text nameStat;
    public TMP_Text currentWorkLocation;
    public TMP_Text family;
    public List<TMP_Text> skills;
    public void UpdateWorkerStats()
    {
        if (currentIndex < 0){return;}
        nameStat.text = villageData.workers[currentIndex];
    }
}
