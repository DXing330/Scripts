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
    public int buildingType = -1;
    public int buildingLevel = -1;
    public int location = -1;
    public TMP_Text nameStat;
    public TMP_Text levelStat;
    public TMP_Text healthStat;
    public TMP_Text hp_level;
    public TMP_Text workerLimit;
    public TMP_Text worker_level;
    public List<StatImageText> outputs;
    public List<GameObject> outputObjects;
    public List<Sprite> resourceSprites;
    public TMP_Text flavor;
    public TMP_Text upgradeCost;
    public List<GameObject> costObjects;
    public List<StatImageText> costs;
    public TMP_Text upgradeTime;

    public void UpdateBuildingStats()
    {
        buildingType = int.Parse(villageData.buildings.buildings[currentIndex]);
        buildingLevel = int.Parse(villageData.buildings.buildingLevels[currentIndex]);
        location = int.Parse(villageData.buildings.buildingLocations[currentIndex]);
        flavor.text = buildingData.ReturnFlavorText(buildingType);
        nameStat.text = buildingData.ReturnBuildingName(buildingType).ToString();
        levelStat.text = buildingLevel.ToString();
        //healthStat.text = villageData.buildings.buildingHealths[currentIndex]+"/"+buildingData.ReturnBuildingMaxHealth(buildingType, buildingLevel);
        healthStat.text = buildingData.ReturnBuildingMaxHealth(buildingType, buildingLevel).ToString();
        hp_level.text = "(+"+buildingData.ReturnHealthPerLevel(buildingType)+")";
        workerLimit.text = buildingData.ReturnWorkerLimit(buildingType, buildingLevel).ToString();
        worker_level.text = "(+"+buildingData.ReturnWorkerPerLevel(buildingType)+")";
        upgradeTime.text = buildingData.ReturnBuildTime(buildingType, buildingLevel).ToString();
        // If already upgrading then return the remaining time.
        if (villageData.CheckIfNewBuilding(location))
        {
            upgradeTime.text = villageData.ReturnNewBuildingTime(location);
        }
        UpdateOutputs(buildingType);
        UpdateBuildCost(buildingType, buildingLevel);
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
            outputs[outputTypes].SetSprite(resourceSprites[int.Parse(outputSpecifics[0])]);
            outputTypes++;
        }   
    }

    protected void UpdateBuildCost(int buildingType, int level = 0)
    {
        for (int i = 0; i < costObjects.Count; i++)
        {
            costObjects[i].SetActive(false);
        }
        if (buildingType < 0){return;}
        int costTypes = 0;
        int specificCost = 0;
        List<string> allCosts = buildingData.ReturnBuildCost(buildingType);
        for (int i = 0; i < allCosts.Count; i++)
        {
            if (allCosts[i].Length <= 2){continue;}
            string[] costSpecifics = allCosts[i].Split("=");
            costObjects[costTypes].SetActive(true);
            specificCost = int.Parse(costSpecifics[1]);
            specificCost *= (level+1);
            costs[costTypes].SetText(specificCost.ToString());
            costs[costTypes].SetSprite(resourceSprites[int.Parse(costSpecifics[0])]);
            costTypes++;
        }
    }
    
    public void UpdateBasicStats(int buildingType)
    {
        nameStat.text = buildingData.ReturnBuildingName(buildingType).ToString();
        healthStat.text = buildingData.ReturnBuildingMaxHealth(buildingType).ToString();
        upgradeTime.text = buildingData.ReturnBuildTime(buildingType).ToString();
        UpdateOutputs(buildingType);
        UpdateBuildCost(buildingType);
    }

    public void UpdateUnbuiltStats(int buildingType)
    {
        nameStat.text = buildingData.ReturnBuildingName(buildingType).ToString();
        healthStat.text = buildingData.ReturnBuildingMaxHealth(buildingType).ToString();
        UpdateOutputs(buildingType);
    }

    public void ResetBasicStats()
    {
        nameStat.text = "";
        healthStat.text = "";
        upgradeCost.text = "";
        upgradeTime.text = "";
        UpdateOutputs(-1);
        UpdateBuildCost(-1);
    }
}
