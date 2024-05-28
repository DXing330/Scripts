using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VillageStats : MonoBehaviour
{
    public VillageDataManager villageData;
    public List<StatImageText> resourceStats;
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
            resourceStats[i].SetText(statString[i]);
        }
    }

    protected void UpdatePopulationStats()
    {
        housing.text = villageData.DetermineWorkerPopulation()+"/"+villageData.DetermineHousingLimit();
    }

    protected void UpdateProjectedResources()
    {
        projectedOutputs = villageData.ReturnOutputs();
        for (int i = 0; i < projectedResourceChange.Count; i++)
        {
            if (projectedOutputs[i+1] > 0)
            {
                projectedResourceChange[i].text = "+"+projectedOutputs[i+1].ToString();
                projectedResourceChange[i].color = Color.green;
            }
            else
            {
                projectedResourceChange[i].text = projectedOutputs[i+1].ToString();
                projectedResourceChange[i].color = Color.red;
            }
        }
    }
}
