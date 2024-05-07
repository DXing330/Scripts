using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sprites", menuName = "ScriptableObjects/Sprites", order = 1)]
public class SpriteContainer : ScriptableObject
{
    public List<Sprite> allSprites;

    public Sprite SpriteDictionary(string spriteName)
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

    public Sprite SpriteByIndex(int index)
    {
        if (index >= 0 && index < allSprites.Count)
        {
            return allSprites[index];
        }
        return null;
    }
}
