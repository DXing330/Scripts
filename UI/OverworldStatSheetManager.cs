using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldStatSheetManager : MonoBehaviour
{
    public void Start()
    {
        UpdateStatSheets();
    }
    public List<OverworldStatSheet> statSheets;
    public List<GameObject> statSheetObjects;

    protected void DisableStatSheets()
    {
        for (int i = 0; i < statSheetObjects.Count; i++)
        {
            statSheetObjects[i].SetActive(false);
        }
    }

    protected void UpdateStatSheets()
    {
        DisableStatSheets();
        // First determine how many stat sheets there are.
        for (int i = 0; i < Mathf.Min(statSheetObjects.Count, GameManager.instance.armyData.allPartyMembers.Count); i++)
        {
            statSheetObjects[i].SetActive(true);
            statSheets[i].UpdateStatSheet(GameManager.instance.armyData.allPartyMembers[i]);
        }
    }
}
