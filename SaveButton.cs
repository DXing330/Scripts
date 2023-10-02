using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveButton : MonoBehaviour
{
    public void Save()
    {
        GameManager.instance.SaveData();
    }

    public void Work()
    {
        GameManager.instance.GainResource(2, 1);
    }

    public void Battle()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("BattleMap");
    }

    public void ManageParty()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("PickArmy");
    }
}
