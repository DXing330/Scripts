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
    public ScriptedBattleDataManager fixedBattleData;
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
    public int location = 2203;
    public int time = 0;
    public List<string> playerPassives;
    public List<string> playerActives;
    public List<string> familiarPassives;
    public List<string> familiarActives;
    public int randomBattle = 0;
    public string battleName = "";
    public List<int> fixedBattleTerrain;
    public List<string> fixedBattleActors;
    public int battleLocationType;
    public int battleDifficulty;
    public int battleWinCondition = 0;
    public string winConSpecifics = "";

    public void SaveData()
    {
        string data = playerLevel+"|"+bloodCrystals+"|"+manaCrystals+"|"+goldCoins+"|"+time+"|"+location;
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

    private void QuickSave()
    {
        string data = playerLevel+"|"+bloodCrystals+"|"+manaCrystals+"|"+goldCoins+"|"+time+"|"+location;
        File.WriteAllText(saveDataPath+"/saveData.txt", data);
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
            location = int.Parse(dataBlocks[5]);
            armyData.Load();
            upgradeData.Load();
        }
        player.UpdateStats();
        familiar.UpdateStats();
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
        location = 2203;
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

    public int ReturnCurrency(int type)
    {
        switch (type)
        {
            case 0:
                return bloodCrystals;
            case 1:
                return manaCrystals;
            case 2:
                return goldCoins;
        }
        return 0;
    }

    public void LevelUp()
    {
        if (bloodCrystals >= playerLevel * playerLevel)
        {
            bloodCrystals -= playerLevel * playerLevel;
            playerLevel++;
            player.UpdateStats();
            familiar.UpdateStats();
            SaveData();
        }
    }

    public void UpdateLocation(int newLocation)
    {
        location = newLocation;
        QuickSave();
    }

    public void SetRandomBattleLocationDifficulty(int location, int difficulty)
    {
        ResetBattleRandomness();
        battleLocationType = location;
        battleDifficulty = difficulty;
        UnityEngine.SceneManagement.SceneManager.LoadScene("BattleMap");
    }

    public void ResetWinCon()
    {
        battleWinCondition = 0;
        winConSpecifics = "";
    }

    public void UpdateWinCon(int type, string specifics)
    {
        battleWinCondition = type;
        winConSpecifics = specifics;
    }

    public void ResetBattleRandomness()
    {
        randomBattle = 0;
        battleName = "";
        fixedBattleTerrain.Clear();
        fixedBattleActors.Clear();
    }

    public void GenerateFixedBattle(string namedBattle)
    {
        randomBattle = 1;
        battleName = namedBattle;
        UpdateFixedBattleData();
        UnityEngine.SceneManagement.SceneManager.LoadScene("BattleMap");
    }

    private void UpdateFixedBattleData()
    {
        string[] newWinCons = fixedBattleData.ReturnFixedWinCons(battleName).Split("=");
        UpdateWinCon(int.Parse(newWinCons[0]), newWinCons[1]);
        fixedBattleTerrain = fixedBattleData.ReturnFixedBattleTerrain(battleName);
        fixedBattleActors = fixedBattleData.ReturnFixedBattleActors(battleName);
    }
}
