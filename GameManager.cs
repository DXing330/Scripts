using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public PlayerActor player;
    public PlayerActor familiar;
    public ArmyDataManager armyData;
    public ActorDataManager actorData;
    public UnitUpgradeData upgradeData;
    private string saveDataPath;
    private string loadedData;

    private void Awake()
    {
        if (GameManager.instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ReturnToHub()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Hub");
        SaveData();
    }

    public void MoveScenes(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    void Start()
    {
        saveDataPath = Application.persistentDataPath;
        LoadData();
    }

    public string ConvertListToString(List<string> string_list, string delimiter = "|")
    {
        return String.Join(delimiter, string_list);
    }

    public void RemoveEmptyListItems(List<string> listToRemoveFrom, int minLength = 1)
    {
        for (int i = 0; i < listToRemoveFrom.Count; i++)
        {
            if (listToRemoveFrom[i].Length <= minLength)
            {
                listToRemoveFrom.RemoveAt(i);
            }
        }
    }

    public int playerLevel = 1;
    public int goldCoins = 0;
    public int manaCrystals = 0;
    public int bloodCrystals = 0;
    public int recentlyWon = 0;
    public int recentlyGainedGold = 0;
    public int recentlyGainedMana = 0;
    public int recentlyGainedBlood = 0;
    public int time = 0;
    public List<string> playerPassives;
    public List<string> playerActives;
    public List<string> familiarPassives;
    public List<string> familiarActives;
    public int battleLocationType;
    public int battleDifficulty;

    public void SaveData()
    {
        string data = playerLevel+"|"+bloodCrystals+"|"+manaCrystals+"|"+goldCoins+"|"+time;
        File.WriteAllText(saveDataPath+"/saveData.txt", data);
        string activesPassives = "";
        activesPassives += ConvertListToString(playerPassives)+"#";
        activesPassives += ConvertListToString(playerActives)+"#";
        activesPassives += ConvertListToString(familiarPassives)+"#";
        activesPassives += ConvertListToString(familiarActives)+"#";
        File.WriteAllText(saveDataPath+"/skillData.txt", activesPassives);
        armyData.Save();
        upgradeData.Save();
    }

    private void LoadData()
    {
        if (File.Exists(saveDataPath+"/saveData.txt"))
        {
            loadedData = File.ReadAllText(saveDataPath+"/saveData.txt");
            string[] dataBlocks = loadedData.Split("|");
            playerLevel = int.Parse(dataBlocks[0]);
            bloodCrystals = int.Parse(dataBlocks[1]);
            manaCrystals = int.Parse(dataBlocks[2]);
            goldCoins = int.Parse(dataBlocks[3]);
            time = int.Parse(dataBlocks[4]);
            armyData.Load();
            upgradeData.Load();
        }
    }

    [ContextMenu("New Game")]
    public void NewGame()
    {
        saveDataPath = Application.persistentDataPath;
        playerLevel = 1;
        bloodCrystals = 0;
        manaCrystals = 0;
        goldCoins = 0;
        time = 0;
        armyData.NewGame();
        SaveData();
    }

    public void GainResource(int type, int amount)
    {
        switch (type)
        {
            case 0:
                bloodCrystals += amount;
                break;
            case 1:
                manaCrystals += amount;
                break;
            case 2:
                goldCoins += amount;
                break;
        }
    }

    public void SetBattleLocation(int locationType)
    {
        battleLocationType = locationType;
    }

    public void SetBattleDifficulty(int newBattleDifficulty)
    {
        battleDifficulty = newBattleDifficulty;
    }

    public bool CheckCost(int type, int amount)
    {
        switch (type)
        {
            case 0:
                return (bloodCrystals >= amount);
            case 1:
                return (manaCrystals >= amount);
            case 2:
                return (goldCoins >= amount);
        }
        return false;
    }

}
