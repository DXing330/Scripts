using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
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

    void Start()
    {
        saveDataPath = Application.persistentDataPath+"/saveData.txt";
        Load();
    }

    public int playerLevel = 1;
    public int familiarLevel = 1;
    public int goldCoins = 0;
    public int manaCrystals = 0;
    public int bloodCrystals = 0;
    public int time = 0;

    public void Save()
    {
        string data = playerLevel+"|"+familiarLevel+"|"+bloodCrystals+"|"+manaCrystals+"|"+goldCoins+"|"+time;
        File.WriteAllText(saveDataPath, data);
    }

    private void Load()
    {
        if (File.Exists(saveDataPath))
        {
            loadedData = File.ReadAllText(saveDataPath);
            string[] dataBlocks = loadedData.Split("|");
            playerLevel = int.Parse(dataBlocks[0]);
            familiarLevel = int.Parse(dataBlocks[1]);
            bloodCrystals = int.Parse(dataBlocks[2]);
            manaCrystals = int.Parse(dataBlocks[3]);
            goldCoins = int.Parse(dataBlocks[4]);
            time = int.Parse(dataBlocks[5]);
        }
    }

    private void NewGame()
    {
        playerLevel = 1;
        familiarLevel = 1;
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
