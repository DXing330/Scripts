using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sprites", menuName = "ScriptableObjects/Sprites", order = 1)]
public class SpriteContainer : ScriptableObject
{
    public List<Sprite> allSprites;
    public List<string> specialNameKeys;
    public List<string> specialNameValues;

    public Sprite SpriteDictionary(string spriteName)
    {
        for (int i = 0; i < allSprites.Count; i++)
        {
            if (allSprites[i].name == spriteName)
            {
                return allSprites[i];
            }
        }
        int indexOf = specialNameKeys.IndexOf(spriteName);
        if (indexOf >= 0){return SpriteDictionary(specialNameValues[indexOf]);}
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

    public int IndexBySprite(string spriteName)
    {
        for (int i = 0; i < allSprites.Count; i++)
        {
            if (allSprites[i].name == spriteName)
            {
                return i;
            }
        }
        return -1;
    }
}
