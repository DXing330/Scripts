using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatImageText : MonoBehaviour
{
    public Image image;
    public Color highlightColor;
    public Color normalColor;
    public TMP_Text stat;

    public void SetText(string newText)
    {
        stat.text = newText;
    }

    public void SetSprite(Sprite newSprite)
    {
        image.sprite = newSprite;
    }

    public void HighlightText()
    {
        stat.color = highlightColor;
    }

    public void Unhighlight()
    {
        stat.color = normalColor;
    }
}
