using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentSlot : BasicImagePanel
{
    public List<Color> rarityColors;
    public Image frame;
    protected void ResetFrameColor()
    {
        frame.color = rarityColors[0];
    }
    protected void UpdateFrameColor(Color newColor)
    {
        frame.color = newColor;
    }
    public TMP_Text title;
    public void ResetTitleColor()
    {
        title.color = rarityColors[0];
    }
    public void HighlightTitle()
    {
        title.color = rarityColors[1];
    }
    // Change color based on rarity.
    public void UpdateColorBasedOnRarity(int rarity)
    {
        if (rarity <= 0)
        {
            UpdateColor(rarityColors[0]);
        }
        else if (rarity >= rarityColors.Count)
        {
            UpdateColor(rarityColors[rarityColors.Count-1]);
        }
        else
        {
            UpdateColor(rarityColors[rarity]);
        }
    }

    public Color ReturnColorBasedOnRarity(int rarity)
    {
        if (rarity <= 0)
        {
            return (rarityColors[0]);
        }
        else if (rarity >= rarityColors.Count)
        {
            return (rarityColors[rarityColors.Count-1]);
        }
        else
        {
            return (rarityColors[rarity]);
        }
    }
}
