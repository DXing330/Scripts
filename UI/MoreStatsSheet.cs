using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoreStatsSheet : OverworldStatSheet
{
    public List<TMP_Text> equipStatTexts;

    void Start()
    {
        statActor = GameManager.instance.armyData.viewStatsActor;
        UpdateStatSheet(statActor);
    }

    public void ReUpdate()
    {
        statActor = GameManager.instance.armyData.viewStatsActor;
        UpdateStatSheet(statActor);
    }

    public void UpdateMoreStats()
    {
        UpdateEquipStats();
    }

    protected void UpdateEquipStats()
    {
        List<int> equipStats = statActor.allEquipment.ReturnStatList();
        for (int i = 0; i < equipStatTexts.Count - 1; i++)
        {
            equipStatTexts[i].text = equipStats[i+1].ToString();
        }
        equipStatTexts[equipStatTexts.Count - 1].text = equipStats[0].ToString();
    }

    public override void UpdateStatSheet(PlayerActor actor, bool baseStats = false)
    {
        base.UpdateStatSheet(actor, true);
        UpdateEquipStats();
    }
}
