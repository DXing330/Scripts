using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSpriteManager : MonoBehaviour
{
    public List<Sprite> allSprites;

    public virtual Sprite SpriteDictionary(string spriteName)
    {
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
