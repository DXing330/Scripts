using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VillageStats : MonoBehaviour
{
    public VillageDataManager villageData;
    public List<TMP_Text> resourceStats;
    public List<TMP_Text> projectedResourceChange;
    protected List<int> projectedOutputs;
    public TMP_Text workerPop;
    public TMP_Text housing;
    // need housing and population stats
    public void UpdateVillageStats()
    {
        UpdateResourceStats(villageData.resources);
        UpdatePopulationStats();
        UpdateProjectedResources();
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

    protected void UpdateProjectedResources()
    {
        projectedOutputs = villageData.ReturnOutputs();
        for (int i = 0; i < projectedOutputs.Count; i++)
        {
            if (projectedOutputs[i] > 0)
            {
                projectedResourceChange[i].text = "+"+projectedOutputs[i].ToString();
                projectedResourceChange[i].color = Color.green;
            }
            else
            {
                projectedResourceChange[i].text = projectedOutputs[i].ToString();
                projectedResourceChange[i].color = Color.red;
            }
        }
    }
}
