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

    public virtual void UpdateStatSheet(PlayerActor actor)
    {
        statActor = actor;
        actorName.text = actor.typeName;
        List<int> allStats = actor.ReturnStatList();
        healthBarText.text = actor.ReturnCurrentHealth()+"/"+allStats[0];
        float xScale = ((float) actor.ReturnCurrentHealth())/((float) allStats[0]);
        greenHealthBar.localScale = new Vector3(xScale, 1, 0);
        for (int i = 0; i < statTexts.Count; i++)
        {
            statTexts[i].text = allStats[i+1].ToString();
        }
        actorSprite.sprite = spriteDictionary.SpriteDictionary(actor.typeName);
    }

    public virtual void ClickOnStatSheet()
    {
        GameManager.instance.armyData.SetViewStatsActor(statActor);
        UnityEngine.SceneManagement.SceneManager.LoadScene("EquipmentSelect");
    }
}
