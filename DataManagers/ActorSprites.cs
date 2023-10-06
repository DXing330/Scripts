using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorSprites : MonoBehaviour
{
    public List<Sprite> allSprites;

    public Sprite SpriteDictionary(string actorName)
    {
        if (actorName == "Player")
        {
            return allSprites[0];
        }
        if (actorName == "Familiar")
        {
            return allSprites[1];
        }
        for (int i = 0; i < allSprites.Count; i++)
        {
            if (allSprites[i].name == actorName)
            {
                return allSprites[i];
            }
        }
        return null;
    }
}
