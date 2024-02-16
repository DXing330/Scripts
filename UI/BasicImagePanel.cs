using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicImagePanel : MonoBehaviour
{
    public Color transColor;
    public Image image;
    public GameObject imageObject;

    public virtual void DisableImage()
    {
        imageObject.SetActive(false);
    }

    public virtual void EnableImage()
    {
        imageObject.SetActive(true);
    }

    public virtual void UpdateImage(Sprite newImage)
    {
        EnableImage();
        image.sprite = newImage;
    }

    public virtual void ResetColor()
    {
        image.color = transColor;
    }

    public virtual void UpdateColor(Color newColor)
    {
        image.color = newColor;
    }
}
