using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorSprites : BasicSpriteManager
{
    public override Sprite SpriteDictionary(string spriteName)
    {
        if (spriteName == "Player")
        {
            return allSprites[0];
        }
        if (spriteName == "Familiar")
        {
            return allSprites[1];
        }
        for (int i = 0; i < allSprites.Count; i++)
        {
            if (allSprites[i].name == spriteName)
            {
                return allSprites[i];
            }
        }
        return null;
    }
}
