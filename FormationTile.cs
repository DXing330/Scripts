using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FormationTile : MonoBehaviour
{
    public Image image;

    public void ResetActorSprite()
    {
        image.sprite = null;
    }

    public void UpdateActorSprite(Sprite newSprite)
    {
        image.sprite = newSprite;
    }
}
