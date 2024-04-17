using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorkerStatSheet : MonoBehaviour
{
    public VillageDataManager villageData;
    public BuildingDataManager buildingData;
    public int currentIndex = 0;
    public virtual void ChangeIndex(bool right = true)
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
    public TMP_Text healthStat;
    public TMP_Text currentWorkLocation;
    public TMP_Text family;
    public List<TMP_Text> skills;
    public GameObject errorPanel;
    public virtual void UpdateWorkerStats()
    {
        errorPanel.SetActive(false);
        if (currentIndex < 0){return;}
        if (villageData.workers.Count <= 0 || villageData.workers[currentIndex].Length <= 0)
        {
            errorPanel.SetActive(true);
            return;
        }
        nameStat.text = villageData.workers[currentIndex];
        //healthStat.text = villageData.workerHealth[currentIndex];
        family.text = villageData.workerFamilySize[currentIndex];
        currentWorkLocation.text = buildingData.ReturnBuildingTask(villageData.ReturnWorkersBuilding(currentIndex));
        if (currentWorkLocation.text.Length < 3){currentWorkLocation.text = "Build";}
        if (villageData.WorkerBuilding(currentIndex)){currentWorkLocation.text = "Build";}
        UpdateWorkerSkills();
    }

    protected virtual void ResetSkills()
    {
        for (int i = 0; i < skills.Count; i++)
        {
            skills[i].text = "";
        }
    }

    protected virtual void UpdateWorkerSkills()
    {
        ResetSkills();
        int index = 0;
        villageData.SortWorkerSkills(currentIndex);
        string[] allSkills = villageData.workerSkills[currentIndex].Split(",");
        string[] specificSkills = new string[2];
        for (int i = 0; i < allSkills.Length; i++)
        {
            if (index >= skills.Count){break;}
            if (allSkills[i].Length < 3){continue;}
            specificSkills = allSkills[i].Split("=");
            // Only add real skills.
            if (int.Parse(specificSkills[1]) >= 100)
            {
                skills[index].text = buildingData.ReturnBuildingName(int.Parse(specificSkills[0]));
                index++;
            }
        }
    }
}
