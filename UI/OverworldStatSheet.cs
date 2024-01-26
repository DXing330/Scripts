using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OverworldStatSheet : MonoBehaviour
{
    public TMP_Text actorName;
    public RectTransform greenHealthBar;
    public TMP_Text healthBarText;
    public List<TMP_Text> statTexts;
    public Image actorSprite;
    public ActorSprites spriteDictionary;

    public void UpdateStatSheet(PlayerActor actor)
    {
        actorName.text = actor.typeName;
        List<int> allStats = actor.ReturnStatList();
        healthBarText.text = actor.ReturnCurrentHealth()+"/"+allStats[0];
        greenHealthBar.localScale = new Vector3(actor.ReturnCurrentHealth()/allStats[0], 1, 0);
        for (int i = 0; i < statTexts.Count; i++)
        {
            statTexts[i].text = allStats[i+1].ToString();
        }
        actorSprite.sprite = spriteDictionary.SpriteDictionary(actor.typeName);
    }
}
