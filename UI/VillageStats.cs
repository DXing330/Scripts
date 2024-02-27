using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VillageStats : MonoBehaviour
{
    public VillageDataManager villageData;
    public List<TMP_Text> resourceStats;
    public TMP_Text workerPop;
    public TMP_Text housing;
    // need housing and population stats
    public void UpdateVillageStats()
    {
        UpdateResourceStats(villageData.resources);
        UpdatePopulationStats();
    }

    protected void UpdateResourceStats(List<string> statString)
    {
        for (int i = 0; i < Mathf.Min(resourceStats.Count, statString.Count); i++)
        {
            resourceStats[i].text = statString[i];
        }
    }

    protected void UpdatePopulationStats()
    {
        workerPop.text = villageData.DetermineWorkerPopulation().ToString();
        housing.text = villageData.DetermineHousingLimit().ToString();
    }
}
