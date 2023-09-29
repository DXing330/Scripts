using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewMapMenu : MonoBehaviour
{
    public GameObject startGameButton;
    public GameObject moveMapMenu;
    public GameObject delayTurnButton;

    public void HideStartGameButton()
    {
        startGameButton.SetActive(false);
        moveMapMenu.SetActive(true);
        delayTurnButton.SetActive(true);
    }

    /*public void ViewBattlers()
    {
        moveMapMenu.SetActive(false);
        viewBattlersButton.SetActive(false);
        viewBattlersMenu.SetActive(true);
    }

    public void Return()
    {
        moveMapMenu.SetActive(true);
        viewBattlersButton.SetActive(true);
        viewBattlersMenu.SetActive(false);
    }*/
}
