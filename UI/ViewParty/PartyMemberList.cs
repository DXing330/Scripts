using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyMemberList : MonoBehaviour
{
    public List<StatImageText> memberImageNames;
    public List<GameObject> memberObjects;
    public List<PlayerActor> partyMembers;
    public ActorSprites spriteDictionary;

    void Start()
    {
        partyMembers = GameManager.instance.armyData.allPartyMembers;
        UpdateMembers();
    }

    protected void ResetObjects()
    {
        for (int i = 0; i < memberObjects.Count; i++)
        {
            memberObjects[i].SetActive(false);
        }
    }

    protected void UpdateMembers()
    {
        ResetObjects();
        string name = "";
        for (int i = 0; i < partyMembers.Count; i++)
        {
            memberObjects[i].SetActive(true);
            name = partyMembers[i].typeName;
            memberImageNames[i].SetText(name);
            memberImageNames[i].SetSprite(spriteDictionary.SpriteDictionary(name));
        }
    }

    public void HighlightSelectedMember(int memberIndex)
    {
        ResetHighlights();
        if (memberIndex < 0){return;}
        memberImageNames[memberIndex].HighlightText();
    }

    protected void ResetHighlights()
    {
        for (int i = 0; i < partyMembers.Count; i++)
        {
            memberImageNames[i].Unhighlight();
        }
    }
}
