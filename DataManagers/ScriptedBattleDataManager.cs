using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptedBattleDataManager : MonoBehaviour
{
    public List<string> fixedBattleNames;
    public List<string> fixedBattleWinCons;
    public List<string> fixedBattleTerrains;
    public List<string> fixedBattleActors;

    public string ReturnFixedWinCons(string battleName)
    {
        int index = fixedBattleNames.IndexOf(battleName);
        if (index < 0)
        {
            return "0=0";
        }
        return fixedBattleWinCons[index];
    }

    public List<string> ReturnFixedBattleActors(string battleName)
    {
        int index = fixedBattleNames.IndexOf(battleName);
        if (index < 0)
        {
            return null;
        }
        return fixedBattleActors[index].Split("|").ToList();
    }

    public List<int> ReturnFixedBattleTerrain(string battleName)
    {
        int index = fixedBattleNames.IndexOf(battleName);
        if (index < 0)
        {
            return null;
        }
        List<int> battleTerrains = new List<int>(fixedBattleTerrains[index].Split("|").ToList().ConvertAll<int>(x => int.Parse(x)));
        return battleTerrains;
    }
}
