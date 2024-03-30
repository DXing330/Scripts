using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ViewSkillGUI : MonoBehaviour
{
    public SkillDataManager skillData;
    public TacticActiveSkill dummySkill;
    public PlayerActor actor;
    public void SetActor(PlayerActor nActor)
    {
        actor = nActor;
    }
    public TextList skillTextList;
    public void ResetSkillView()
    {
        skillTextList.HighlightText(-1);
        detailsPanel.SetActive(false);
    }
    public GameObject detailsPanel;
    public List<TMP_Text> skillDetails;
    public List<TerrainTile> rangeDetails;
    public BasicPathfinder pathfinder;
    public int rangeMapSize = 9;
    public ActorSprites actorSprites;
    public List<string> skillDetailStrings;
    public void UpdateSkillTextList(bool active = true)
    {
        if (active)
        {
            // Grab the currently selected actor's active list.
            skillTextList.SetAllText(actor.ReturnActives());
        }
        else
        {
            skillTextList.SetAllText(actor.ReturnPassives());
        }
    }

    protected void UpdateSkill(string skillName)
    {
        skillData.LoadDataForSkill(dummySkill, skillName);
        skillDetailStrings = dummySkill.ReturnStatList();
        for (int i = 0; i < skillDetails.Count; i++)
        {
            skillDetails[i].text = skillDetailStrings[i];
        }
        UpdateRange();
    }

    public void ClickOnSkill(int index)
    {
        skillTextList.HighlightText(index);
        detailsPanel.SetActive(true);
        UpdateSkill(skillTextList.ReturnText(index));
    }

    protected void UpdateRange()
    {
        for (int i = 0; i < rangeDetails.Count; i++)
        {
            rangeDetails[i].ResetHighlight();
        }
        List<string> rangeStats = dummySkill.ReturnRangeStats();
        // Draw the actor in the center tile.
        // Center = 9*9/2 = 41
        int center = (rangeMapSize*rangeMapSize/2);
        rangeDetails[center].UpdateImage(actorSprites.SpriteDictionary(actor.typeName));
        // Find what tiles are in range.
        pathfinder.RecursiveAdjacency(center, int.Parse(rangeStats[0]));
        List<int> tilesInRange = pathfinder.adjacentTiles;
        // Highlight tiles in range.
        for (int i = 0; i < tilesInRange.Count; i++)
        {
            rangeDetails[tilesInRange[i]].Highlight();
        }
    }
}
