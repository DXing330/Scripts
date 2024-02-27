using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageGUI : MonoBehaviour
{
    protected void Start()
    {
        villageData = GameManager.instance.villageData;
        workerStatSheet.villageData = villageData;
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
                break;
        }
    }
    public int state = -1;
    public void ChangeState(int newState)
    {
        if (newState == state){state = -1;}
        else {state = newState;}
        ActivatePanels();
    }
}
