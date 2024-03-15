using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatImageText : MonoBehaviour
{
    public Image image;
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
        stat.color = Color.red;
    }

    public void Unhighlight()
    {
        stat.color = Color.white;
    }
}
