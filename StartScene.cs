using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScene : MonoBehaviour
{
    public void StartGame()
    {
        GameManager.instance.StartFromMenu();
    }

    public void NewGame()
    {
        GameManager.instance.NewGame();
        GameManager.instance.StartFromMenu(true);
    }
}
