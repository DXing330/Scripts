using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NewWorkerStatSheet : WorkerStatSheet
{
    public TMP_Text applicantsAmount;
    protected void UpdateApplicantsText()
    {
        applicantsAmount.text = (currentIndex+1)+"/"+numberApplicants;
    }
    public int numberApplicants = 0;
    public void UpdateApplicantsAmount(int amount){numberApplicants = amount;}
    public TMP_Text costStat;
    public List<GameObject> errorPanels;
    public GameObject poorError;
    public List<string> currentApplicants;
    // Need to generate the cost, skills, family size and name from somewhere.
    public override void ChangeIndex(bool right = true)
    {
        int last = currentApplicants.Count;
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
        UpdateNewWorkerStats(currentApplicants[currentIndex]);
    }

    public void StartViewingApplicants()
    {
        GameManager.instance.utility.DisableAllObjects(errorPanels);
        errorPanel.SetActive(false);
        currentApplicants = villageData.vassalHiring.currentlyAvailable;
        UpdateApplicantsAmount(currentApplicants.Count);
        if (currentApplicants.Count > 0)
        {
            currentIndex = 0;
            UpdateNewWorkerStats(currentApplicants[currentIndex]);
        }
        else
        {
            errorPanel.SetActive(true);
        }
    }

    public void PoorError()
    {
        poorError.SetActive(true);
    }

    protected void UpdateNewWorkerStats(string potentialWorker)
    {
        ResetSkills();
        string[] workerData = potentialWorker.Split("|");
        nameStat.text = workerData[0];
        costStat.text = workerData[3];
        UpdateNewWorkerSkills(workerData[1]);
        family.text = workerData[2];
        UpdateApplicantsText();
    }

    protected void UpdateNewWorkerSkills(string allSkills)
    {
        int index = 0;
        string[] newSkills = allSkills.Split(",");
        string[] specificSkills = new string[2];
        for (int i = 0; i < newSkills.Length; i++)
        {
            if (index >= skills.Count){break;}
            if (newSkills[i].Length < 3){continue;}
            specificSkills = newSkills[i].Split("=");
            // Only add real skills.
            if (int.Parse(specificSkills[1]) >= 100)
            {
                skills[index].text = buildingData.ReturnBuildingName(int.Parse(specificSkills[0]));
                index++;
            }
        }
    }
}
