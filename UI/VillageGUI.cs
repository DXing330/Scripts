using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VillageGUI : MonoBehaviour
{
    protected void Start()
    {
        villageData = GameManager.instance.villageData;
        workerStatSheet.villageData = villageData;
        buildingStatSheet.villageData = villageData;
        newBuildingStatSheet.villageData = villageData;
        villageStats.villageData = villageData;
        villageStats.UpdateVillageStats();
    }
    public VillageDataManager villageData;
    public VillageEditor villageManager;
    public VillageStats villageStats;
    public WorkerStatSheet workerStatSheet;
    public void ChangeWorkerIndex(bool right = true)
    {
        workerStatSheet.ChangeIndex(right);
        villageManager.HighlightSelectedWorker(workerStatSheet.currentIndex);
    }
    public BuildingStatSheet buildingStatSheet;
    public void ShiftBuildingIndex(bool right = true)
    {
        int count = villageData.buildings.Count;
        int currentIndex = buildingStatSheet.currentIndex;
        if (right)
        {
            if (currentIndex < count - 1){currentIndex++;}
            else {currentIndex = 0;}
        }
        else
        {
            if (currentIndex > 0){currentIndex--;}
            else {currentIndex = count - 1;}
        }
        ChangeBuildingIndex(currentIndex);
        UpdatePanels();
    }
    public void ChangeBuildingIndex(int newIndex)
    {
        buildingStatSheet.SetIndex(newIndex);
        //villageManager.HighlightSelectedBuilding(workerStatSheet.currentIndex);
    }
    public NewBuildingStatSheet newBuildingStatSheet;
    public void ChangeTerrainType(int newType)
    {
        newBuildingStatSheet.SetTerrainType(newType);
    }
    public void CheckUnbuiltBuilding(int tileNumber)
    {
        newBuildingStatSheet.CheckOnUnBuilt(tileNumber);
    }
    public List<GameObject> panels;
    protected void ActivatePanels()
    {
        for (int i = 0; i < panels.Count; i++)
        {
            panels[i].SetActive(false);
        }
        if (state >= 0)
        {
            panels[state].SetActive(true);
        }
        switch (state)
        {
            case 0:
                workerStatSheet.UpdateWorkerStats();
                villageManager.HighlightSelectedWorker(workerStatSheet.currentIndex);
                break;
            case 1:
                buildingStatSheet.UpdateBuildingStats();
                villageManager.HighlightSelectedBuilding(buildingStatSheet.currentIndex);
                break;
            case 2:
                newBuildingStatSheet.SetTerrainType(-1);
                break;
        }
    }
    public void UpdatePanels()
    {
        switch (state)
        {
            case 0:
                villageStats.UpdateVillageStats();
                workerStatSheet.UpdateWorkerStats();
                villageManager.HighlightSelectedWorker(workerStatSheet.currentIndex);
                break;
            case 1:
                villageStats.UpdateVillageStats();
                buildingStatSheet.UpdateBuildingStats();
                villageManager.HighlightSelectedBuilding(buildingStatSheet.currentIndex);
                break;
        }
    }
    public int state = -1;
    public List<TMP_Text> stateTexts;
    protected void BoldStateText()
    {
        for (int i = 0; i < stateTexts.Count; i++)
        {
            bool isSet = (stateTexts[i].fontStyle & FontStyles.Bold) != 0;
            if(isSet){stateTexts[i].fontStyle ^= FontStyles.Bold;}
        }
        if (state >= 0)
        {
            stateTexts[state].fontStyle |= FontStyles.Bold;
        }
    }

    public void ChangeState(int newState)
    {
        if (newState == state){state = -1;}
        else {state = newState;}
        villageManager.SetState(state);
        ActivatePanels();
        BoldStateText();
    }

    public void TryToBuildNew()
    {
        // Get the tile and building.
        int buildingType = newBuildingStatSheet.ReturnSelectedBuilding();
        if (buildingType < 0){return;}
        villageManager.TryToBuildNew(buildingType);
        villageStats.UpdateVillageStats();
    }

    public void TryToUpgrade()
    {
        // Get the tile and building.
        int buildingType = buildingStatSheet.buildingType;
        int buildingLevel = buildingStatSheet.buildingLevel;
        if (buildingType < 0){return;}
        villageManager.TryToUpgrade(buildingType, buildingLevel);
        villageStats.UpdateVillageStats();
    }
}
