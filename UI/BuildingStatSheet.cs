using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingStatSheet : MonoBehaviour
{
    public VillageDataManager villageData;
    public BuildingDataManager buildingData;
    public int currentIndex = 0;
    public void SetIndex(int newIndex)
    {
        if (newIndex < 0){return;}
        currentIndex = newIndex;
        UpdateBuildingStats();
    }
    public TMP_Text nameStat;
    public TMP_Text levelStat;
    public TMP_Text healthStat;
    public TMP_Text hp_level;
    public TMP_Text workerLimit;
    public TMP_Text worker_level;
    public List<StatImageText> outputs;
    public List<GameObject> outputObjects;
    public List<Sprite> outputSprites;
    public TMP_Text flavor;
    public TMP_Text upgradeCost;
    public TMP_Text upgradeTime;

    public void UpdateBuildingStats()
    {
        int buildingType = int.Parse(villageData.buildings[currentIndex]);
        int buildingLevel = int.Parse(villageData.buildingLevels[currentIndex]);
        flavor.text = buildingData.ReturnFlavorText(buildingType);
        nameStat.text = buildingData.ReturnBuildingName(buildingType).ToString();
        levelStat.text = buildingLevel.ToString();
        healthStat.text = villageData.buildingHealths[currentIndex]+"/"+buildingData.ReturnBuildingMaxHealth(buildingType, buildingLevel);
        hp_level.text = "(+"+buildingData.ReturnHealthPerLevel(buildingType)+")";
        workerLimit.text = buildingData.ReturnWorkerLimit(buildingType, buildingLevel).ToString();
        worker_level.text = "(+"+buildingData.ReturnWorkerPerLevel(buildingType)+")";
        upgradeCost.text = buildingData.ReturnBuildCost(buildingType, buildingLevel).ToString();
        upgradeTime.text = buildingData.ReturnBuildTime(buildingType, buildingLevel).ToString();
        UpdateOutputs(buildingType);
    }

    protected void UpdateOutputs(int buildingType)
    {
        for (int i = 0; i < outputObjects.Count; i++)
        {
            outputObjects[i].SetActive(false);
        }
        if (buildingType < 0){return;}
        int outputTypes = 0;
        List<string> allOutputs = buildingData.ReturnOutputList(buildingType);
        for (int i = 0; i < allOutputs.Count; i++)
        {
            if (allOutputs[i].Length <= 2){continue;}
            string[] outputSpecifics = allOutputs[i].Split("=");
            outputObjects[outputTypes].SetActive(true);
            outputs[outputTypes].SetText(outputSpecifics[1]);
            outputs[outputTypes].SetSprite(outputSprites[int.Parse(outputSpecifics[0])]);
            outputTypes++;
        }   
    }

    public void UpdateBasicStats(int buildingType)
    {
        nameStat.text = buildingData.ReturnBuildingName(buildingType).ToString();
        healthStat.text = buildingData.ReturnBuildingMaxHealth(buildingType).ToString();
        workerLimit.text = buildingData.ReturnWorkerLimit(buildingType).ToString();
        upgradeCost.text = buildingData.ReturnBuildCost(buildingType).ToString();
        upgradeTime.text = buildingData.ReturnBuildTime(buildingType).ToString();
        UpdateOutputs(buildingType);
    }

    public void ResetBasicStats()
    {
        nameStat.text = "";
        healthStat.text = "";
        workerLimit.text = "";
        upgradeCost.text = "";
        upgradeTime.text = "";
        UpdateOutputs(-1);
    }
}
