using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroupsData : MonoBehaviour
{
    public string configData;
    public List<string> forestEnemiesEZ;
    public List<string> forestEnemiesMID;
    public List<string> forestEnemiesHARD;
    public List<string> mountainEnemiesEZ;
    public List<string> mountainEnemiesMID;
    public List<string> mountainEnemiesHARD;

    void Awake()
    {
        LoadEnemyGroupData();
    }

    private void LoadEnemyGroupData()
    {
        string[] blocks = configData.Split("#");
        forestEnemiesEZ = blocks[0].Split("||")[0].Split("|").ToList();
        forestEnemiesMID = blocks[1].Split("||")[0].Split("|").ToList();
        forestEnemiesHARD = blocks[2].Split("||")[0].Split("|").ToList();
        mountainEnemiesEZ = blocks[3].Split("||")[0].Split("|").ToList();
        mountainEnemiesMID = blocks[4].Split("||")[0].Split("|").ToList();
        mountainEnemiesHARD = blocks[5].Split("||")[0].Split("|").ToList();
    }

    public string ReturnEnemyGroup(int type, int difficulty)
    {
        switch (type)
        {
            case 1:
                return ReturnForestGroup(difficulty);
        }
        return "Wolf";
    }

    private string ReturnForestGroup(int difficulty)
    {
        int rng = 0;
        switch (difficulty)
        {
            case 0:
                rng = Random.Range(0, forestEnemiesEZ.Count);
                return forestEnemiesEZ[rng];
            case 1:
                rng = Random.Range(0, forestEnemiesMID.Count);
                return forestEnemiesMID[rng];
            case 2:
                rng = Random.Range(0, forestEnemiesHARD.Count);
                return forestEnemiesHARD[rng];
        }
        return "Wolf";
    }
}
