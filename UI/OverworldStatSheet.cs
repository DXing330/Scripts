using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class OverworldStatSheet : MonoBehaviour
{
    public TMP_Text actorName;
    public RectTransform greenHealthBar;
    public TMP_Text healthBarText;
    public List<TMP_Text> statTexts;
    public Image actorSprite;
    public ActorSprites spriteDictionary;
    public PlayerActor statActor;
    public int index;

    public virtual void UpdateStatSheet(PlayerActor actor, bool baseStats = false)
    {
        statActor = actor;
        actorName.text = actor.ReturnName();
        List<int> allStats = actor.ReturnStatList();
        List<int> equipStats = actor.allEquipment.ReturnStatList();
        float xScale = 0;
        if (!baseStats)
        {
            healthBarText.text = (actor.ReturnCurrentHealth()+equipStats[0])+"/"+(allStats[0]+equipStats[0]);
            xScale = ((float) (actor.ReturnCurrentHealth()+equipStats[0]))/((float) (allStats[0]+equipStats[0]));
        }
        else
        {
            healthBarText.text = (actor.ReturnCurrentHealth())+"/"+(allStats[0]);
            xScale = ((float) actor.ReturnCurrentHealth())/((float) allStats[0]);
        }
        greenHealthBar.localScale = new Vector3(xScale, 1, 0);
        for (int i = 0; i < statTexts.Count; i++)
        {
            if (baseStats)
            {
                statTexts[i].text = (allStats[i+1]).ToString();
            }
            else
            {
                statTexts[i].text = (allStats[i+1]+equipStats[i+1]).ToString();
            }
        }
        actorSprite.sprite = spriteDictionary.SpriteDictionary(actor.typeName);
    }

    public virtual void ClickOnStatSheet()
    {
        GameManager.instance.armyData.SetViewStatsIndex(index);
        UnityEngine.SceneManagement.SceneManager.LoadScene("EquipmentSelect");
    }
}
