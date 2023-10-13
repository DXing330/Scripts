using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FormationTile : MonoBehaviour
{
    public Image image;
    public Image highlight;

    public void ResetHighlight()
    {
        Color tempColor = Color.white;
        tempColor.a = 0f;
        highlight.color = tempColor;
    }

    public void Highlight()
    {
        Color tempColor = Color.green;
        tempColor.a = 0.3f;
        highlight.color = tempColor;
    }

    public void ResetActorSprite()
    {
        image.sprite = null;
    }

    public void UpdateActorSprite(Sprite newSprite)
    {
        image.sprite = newSprite;
        Color tempColor = Color.white;
        tempColor.a = 1f;
        image.color = tempColor;
    }
}
