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
    public void SetActor(PlayerActor nActor){actor = nActor;}
    public TextList skillTextList;
    public void ResetSkillView()
    {
        skillTextList.HighlightText(-1);
        detailsPanel.SetActive(false);
    }
    public GameObject detailsPanel;
    public ImagePanel actionCost;
    public ImagePanel energyCost;
    public TMP_Text skillDescription;
    public List<TerrainTile> rangeDetails;
    public BasicPathfinder pathfinder;
    public int rangeMapSize = 9;
    public ActorSprites actorSprites;
    public List<string> skillDetailStrings;
    public void UpdateSkillTextList()
    {
        skillTextList.SetAllText(actor.ReturnActives());
    }

    protected virtual void UpdateSkill(string skillName)
    {
        skillData.LoadDataForSkill(dummySkill, skillName);
        //skillDetailStrings = dummySkill.ReturnStatList();
        UpdateCosts();
        UpdateDescription();
        UpdateRange();
    }

    public void ClickOnSkill(int index)
    {
        skillTextList.HighlightText(index);
        detailsPanel.SetActive(true);
        UpdateSkill(skillTextList.ReturnText(index));
    }

    protected void UpdateCosts()
    {
        actionCost.ActivateImages(dummySkill.ReturnActionCost());
        energyCost.ActivateImages(dummySkill.cost);
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
        int range = int.Parse(rangeStats[0]);
        pathfinder.RecursiveAdjacency(center, range);
        // Highlight tiles in range.
        for (int i = 0; i < pathfinder.adjacentTiles.Count; i++)
        {
            rangeDetails[pathfinder.adjacentTiles[i]].Highlight();
        }
        // If the span is greater than zero then highlight the span as well.
        int span = int.Parse(rangeStats[1]);
        if (span > 0)
        {
            pathfinder.RecursiveAdjacency(center, span);
            for (int i = 0; i < pathfinder.adjacentTiles.Count; i++)
            {
                rangeDetails[pathfinder.adjacentTiles[i]].Highlight(false);
            }
            if (range > 0){rangeDetails[center].Highlight(false);}
        }
    }

    protected virtual void UpdateDescription()
    {
        skillDescription.text = "";
        int parts = GameManager.instance.utility.CountOccurencesOfCharInString(dummySkill.effect, '+');
        for (int i = 0; i < parts+1; i++)
        {
            skillDescription.text += dummySkill.ReturnEffectDescription(i)+"\n";
        }
    }
}
