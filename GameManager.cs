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
    }

    void Start()
    {
        saveDataPath = Application.persistentDataPath;
        Load();
    }

    public string ConvertListToString(List<string> string_list, string delimiter = "|")
    {
        return String.Join(delimiter, string_list);
    }

    public int playerLevel = 1;
    public int goldCoins = 0;
    public int manaCrystals = 0;
    public int bloodCrystals = 0;
    public int time = 0;
    public List<string> armyFormation;
    public List<string> playerPassives;
    public List<string> playerActives;
    public List<string> familiarPassives;
    public List<string> familiarActives;

    public void Save()
    {
        string data = playerLevel+"|"+bloodCrystals+"|"+manaCrystals+"|"+goldCoins+"|"+time;
        File.WriteAllText(saveDataPath+"/saveData.txt", data);
        string armyData = ConvertListToString(armyFormation);
        File.WriteAllText(saveDataPath+"/armyData.txt", armyData);
        string activesPassives = "";
        activesPassives += ConvertListToString(playerPassives)+"#";
        activesPassives += ConvertListToString(playerActives)+"#";
        activesPassives += ConvertListToString(familiarPassives)+"#";
        activesPassives += ConvertListToString(familiarActives)+"#";
        File.WriteAllText(saveDataPath+"/skillData.txt", activesPassives);
    }

    private void Load()
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
            loadedData = File.ReadAllText(saveDataPath+"/armyData.txt");
            armyFormation = loadedData.Split("|").ToList();
        }
    }

    private void NewGame()
    {
        playerLevel = 1;
        bloodCrystals = 0;
        manaCrystals = 0;
        goldCoins = 0;
        time = 0;
        Save();
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

}
