using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewMapMenu : MonoBehaviour
{
    public GameObject startGameButton;
    public GameObject moveMapMenu;

    public void HideStartGameButton()
    {
        startGameButton.SetActive(false);
    }
}
