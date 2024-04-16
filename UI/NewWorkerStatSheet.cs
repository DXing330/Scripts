using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NewWorkerStatSheet : WorkerStatSheet
{
    public TMP_Text costStat;
    // Need to generate the cost, skills, family size and name from somewhere.
    public void UpdateNewWorkerStats(string potentialWorker)
    {
        ResetSkills();
        string[] workerData = potentialWorker.Split("|");
        nameStat.text = workerData[0];
        costStat.text = workerData[1];
        UpdateNewWorkerSkills(workerData[2]);
        family.text = workerData[3];
    }

    protected void UpdateNewWorkerSkills(string allSkills)
    {
        string[] newSkills = allSkills.Split(",");
        for (int i = 0; i < Mathf.Min(skills.Count, newSkills.Length); i++)
        {
            skills[i].text = newSkills[i];
        }
    }
}
