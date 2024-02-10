using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    public Image equipImage;
    public GameObject imageObject;

    public void DisableImage()
    {
        imageObject.SetActive(false);
    }

    public void EnableImage()
    {
        imageObject.SetActive(true);
    }

    public void UpdateImage(Sprite newImage)
    {
        EnableImage();
        equipImage.sprite = newImage;
    }
}
