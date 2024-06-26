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
    public ScriptableUtility utility;
    public int basePartySize = 2;
    public List<PlayerActor> playerActors;
    public PlayerActor player;
    public PlayerActor familiar;
    public PlayerSettings playerSettings;
    public string ReturnPlayerSetting(int settingIndex)
    {
        return playerSettings.ReturnSetting(settingIndex);
    }
    public List<BasicDataManager> gameData;
    public VillageDataManager villageData;
    public LevelDataManager levelData;
    public ArmyDataManager armyData;
    public EquipmentInventory equipInventory;
    public ActorDataManager actorData;
    //public EquipmentData equipData;
    public UnitUpgradeData upgradeData;
    private string saveDataPath;
    public string newGameData;
    public string loadedData;

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

    public int villageOnLevel = 0;
    public int villageOnIndex = 38;

    public void StartFromMenu(bool newGame = false)
    {
        if (newGame)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("NameSelect");
        }
        else
        {
            if (playerName.Length < 1)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("NameSelect");
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Overworld");
            }
        }
    }

    public void ReturnToHub(bool village = false)
    {
        if (village)
        {
            currentLevel = villageOnLevel;
            currentLocation = villageOnIndex;
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("Overworld");
        SaveData();
    }

    public void ReturnToVillage()
    {
        StartCoroutine(LoadAsyncScene("Village"));
        SaveData();
    }

    IEnumerator LoadAsyncScene(string sceneName)
    {
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void MoveScenes(string sceneName)
    {
        StartCoroutine(LoadAsyncScene(sceneName));
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

    public int CountOccurencesOfStringInList(List<string> listToCountFrom, string stringToCount)
    {
        int count = listToCountFrom.Count(s => s == stringToCount);
        return count;
    }

    public string playerName = "";
    public void UpdatePlayerName(string newName)
    {
        playerName = newName;
        player.SetPersonalName(playerName);
        QuickSave();
    }
    public int playerLevel = 1;
    public int goldCoins = 0;
    public int manaCrystals = 0;
    public int bloodCrystals = 0;
    public int recentlyWon = 0;
    public int recentlyGainedGold = 0;
    public int recentlyGainedMana = 0;
    public int recentlyGainedBlood = 0;
    public int currentLevel = -1;
    public int currentLocation = -1;
    public int villageLevel = 0;
    public int villageLocation = 38;
    public bool AtVillage()
    {
        if (currentLevel == villageLevel && currentLocation == villageLocation){return true;}
        return false;
    }
    public int time = 0;
    public int yearLength = 300;
    public List<string> playerPassives;
    public List<string> playerActives;
    public List<string> familiarPassives;
    public List<string> familiarActives;
    public int villageBattle = 0;
    public string villageBattleDataString;
    public void StartVillageBattle(string newBattle)
    {
        villageBattleDataString = newBattle;
        villageBattle = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene("BattleMap");
    }
    public int randomBattle = 0;
    public string battleName = "";
    public string fixedTerrainString;
    public List<string> forestFixedTerrains;
    public string fixedBattleString;
    public List<string> fixedBattles;
    public List<int> fixedBattleTerrain;
    public List<string> fixedBattleActors;
    public int battleLocationType;
    public int battleDifficulty;
    public int battleNumber;
    public int battleWinCondition = 0;
    public string winConSpecifics = "";

    [ContextMenu("Save Game")]
    public void SaveData()
    {
        string data = playerLevel+"|"+bloodCrystals+"|"+manaCrystals+"|"+goldCoins+"|"+time+"|"+currentLevel+"|"+currentLocation+"|"+playerName+"|"+villageLevel+"|"+villageLocation;
        File.WriteAllText(saveDataPath+"/saveData.txt", data);
        string activesPassives = "";
        activesPassives += ConvertListToString(playerPassives)+"#";
        activesPassives += ConvertListToString(playerActives)+"#";
        activesPassives += ConvertListToString(familiarPassives)+"#";
        activesPassives += ConvertListToString(familiarActives)+"#";
        File.WriteAllText(saveDataPath+"/skillData.txt", activesPassives);
        utility.DataManagerSave(gameData);
    }

    private void QuickSave()
    {
        string data = playerLevel+"|"+bloodCrystals+"|"+manaCrystals+"|"+goldCoins+"|"+time+"|"+currentLevel+"|"+currentLocation+"|"+playerName+"|"+villageLevel+"|"+villageLocation;
        File.WriteAllText(saveDataPath+"/saveData.txt", data);
    }

    private void LoadData()
    {
        if (File.Exists(saveDataPath+"/saveData.txt"))
        {
            loadedData = File.ReadAllText(saveDataPath+"/saveData.txt");
            LoadDataString();
            utility.DataManagerLoad(gameData);
        }
        else{NewGame();}
        // First load the current party.
        armyData.GetAllPartyMembers();
        // Then load their equipment.
        equipInventory.LoadEquipSets();
        // Then load their stats.
        armyData.UpdatePartyStats();
        RefreshMaps();
        RefreshBattles();
    }

    protected void LoadDataString()
    {
        string[] dataBlocks = loadedData.Split("|");
        if (dataBlocks.Length < 7){dataBlocks = newGameData.Split("|");}
        playerLevel = int.Parse(dataBlocks[0]);
        bloodCrystals = int.Parse(dataBlocks[1]);
        manaCrystals = int.Parse(dataBlocks[2]);
        goldCoins = int.Parse(dataBlocks[3]);
        time = int.Parse(dataBlocks[4]);
        currentLevel = int.Parse(dataBlocks[5]);
        currentLocation = int.Parse(dataBlocks[6]);
        UpdatePlayerName(dataBlocks[7]);
        villageLevel = int.Parse(dataBlocks[8]);
        villageLocation = int.Parse(dataBlocks[9]);
    }

    public void RefreshMaps()
    {
        forestFixedTerrains = fixedTerrainString.Split("#").ToList();
    }

    public void RefreshBattles()
    {
        fixedBattles = fixedBattleString.Split("#").ToList();
    }

    [ContextMenu("New Game")]
    public void NewGame()
    {
        saveDataPath = Application.persistentDataPath;
        if (File.Exists(saveDataPath+"/saveData.txt"))
        {
            File.Delete (saveDataPath+"/saveData.txt");
        }
        if (newGameData.Length > 0)
        {
            loadedData = newGameData;
            LoadDataString();
        }
        else
        {
            playerLevel = 1;
            bloodCrystals = 0;
            manaCrystals = 0;
            goldCoins = 0;
            time = 0;
            currentLevel = -1;
            currentLocation = -1;
            playerName = "";

        }
        File.WriteAllText(saveDataPath+"/skillData.txt", "");
        utility.DataManagerNewGame(gameData);
        SaveData();
        LoadData();
    }

    public void NewDay()
    {
        time++;
        // Gain Resources Weekly.
        villageData.NewDay(utility.DivisibleNumber(time, 6));
        if (utility.DivisibleNumber(time,yearLength))
        {
            villageData.NewYear();
        }
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

    public void ResetParty()
    {
        //if (playerActors.Count <= basePartySize){return;}
        playerActors.Clear();
        playerActors.Add(player);
        playerActors.Add(familiar);
    }

    public void UpdateLocation(int newLocation, int newLevel = -1)
    {
        if (newLevel >= 0){currentLevel = newLevel;}
        currentLocation = newLocation;
        QuickSave();
    }

    public void SetRandomBattleLocationDifficulty(int battleLocation, int difficulty)
    {
        ResetBattleRandomness();
        battleLocationType = battleLocation;
        battleDifficulty = difficulty;
        UnityEngine.SceneManagement.SceneManager.LoadScene("BattleMap");
    }

    public void StartRandomBattle(int newBattle)
    {
        randomBattle = 0;
        battleNumber = newBattle;
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
}
