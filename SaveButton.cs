using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveButton : MonoBehaviour
{
    public void Save()
    {
        GameManager.instance.Save();
        Debug.Log("Saved");
    }

    public void Work()
    {
        GameManager.instance.GainResource(2, 1);
    }

    public void Battle()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("BattleMap");
    }
}
