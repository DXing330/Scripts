using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImagePanel : MonoBehaviour
{
    public List<GameObject> images;
    //void Start(){ActivateImages(0);}
    public void ActivateImages(int amount)
    {
        GameManager.instance.utility.DisableAllObjects(images);
        if (amount <= 0){return;}
        for (int i = 0; i < Mathf.Min(amount, images.Count); i++)
        {
            images[i].SetActive(true);
        }
    }
}
