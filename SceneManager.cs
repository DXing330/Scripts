using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public void ReturnToHub()
    {
        GameManager.instance.ReturnToHub();
    }

    public void MoveScenes(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void SetBattleDifficulty(int difficulty)
    {
        GameManager.instance.battleDifficulty = difficulty;
    }

    public void SetBattleTerrain(int terrainType)
    {
        GameManager.instance.battleLocationType = terrainType;
    }
}
