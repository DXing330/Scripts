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

    public void LoadCurrentLocationScene()
    {
        // Based on location in GameManager.
        // For now just return to hub.
        ReturnToHub();
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

    public void SetBattleWinCon(string winCon = "")
    {
        if (winCon.Length < 5)
        {
            GameManager.instance.ResetWinCon();
        }
        string[] conSpecifics = winCon.Split("=");
        GameManager.instance.battleWinCondition = int.Parse(conSpecifics[0]);
        GameManager.instance.winConSpecifics = conSpecifics[1];
    }
}
