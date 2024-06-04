using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighlightableImage : MonoBehaviour
{
    public Image image;
    public Image highlight;
    public Color blankColor;
    public Color defaultColor;
    public Color highlightColor;

    public void ResetHighlight(){highlight.color = defaultColor;}

    public void Highlight(){highlight.color = highlightColor;}

    public void ResetImage(){image.sprite = null;}

    public void UpdateSprite(Sprite newSprite)
    {
        image.sprite = newSprite;
        image.color = defaultColor;
    }

    public void ResetImageColor(){image.color = blankColor;}

    public void UpdateImageColor(Color newColor){image.color = newColor;}
}
