using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SceneManager", menuName = "ScriptableObjects/SceneManager", order = 1)]
public class ScriptableSceneManager : ScriptableObject
{
    public void ReturnToHub()
    {
        GameManager.instance.ReturnToHub();
    }

    public void ReturnFromVillage()
    {
        GameManager.instance.ReturnToHub(true);
    }

    public void ReturnToVillage()
    {
        GameManager.instance.ReturnToVillage();
    }

    public void MoveScenes(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
