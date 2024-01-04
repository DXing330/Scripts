using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActorManager : MonoBehaviour
{
    public ActorSprites actorSprites;
    public TacticActor actorPrefab;
    public TerrainMap terrainMap;
    public int winCondition = 0;
    public string winConditionSpecifics = null;
    private int teamOneCount = 0;
    private int teamZeroCount = 0;
    public TerrainTile terrainTile;
    public ActorDataManager actorData;
    public EnemyGroupsData groupsData;
    public SkillDataManager skillData;
    private int tries = 0;
    public List<int> usedTiles;
    public List<int> unusableTiles;
    public int collectedGold = 0;
    public int collectedMana = 0;
    public int collectedBlood = 0;

    void Start()
    {
        GetActorData();
    }

    public void GetActorData()
    {
        actorData = GameManager.instance.actorData;
        winCondition = GameManager.instance.battleWinCondition;
        winConditionSpecifics = GameManager.instance.winConSpecifics;
    }

    public void RemoveFromPlayerTeam(TacticActor actor)
    {
        if (actor == null){return;}
        if (actor.typeName == "Player" || actor.typeName == "Familiar" || actor.typeName == ""){return;}
        int indexOf = GameManager.instance.armyData.armyFormation.IndexOf(actor.typeName);
        if (indexOf < 0){return;}
        GameManager.instance.armyData.armyFormation[indexOf] = "none";
    }

    private void LosePlayerArmy()
    {
        for (int i = 0; i < GameManager.instance.armyData.armyFormation.Count; i++)
        {
            string name = GameManager.instance.armyData.armyFormation[i];
            if (name == "Player" || name == "Familiar")
            {
                continue;
            }
            GameManager.instance.armyData.armyFormation[i] = "none";
        }
    }

    public void LoadPlayerTeam(bool fix = false)
    {
        // Can load this based on the party list now.
        /*if (!fix)
        {
            tries = 0;
            int enemyCount = usedTiles.Count;
            for (int i = 0; i < GameManager.instance.playerActors.Count; i++)
            {
                GenerateRandomLocation();
                LoadPlayerTeamMemberFromActor(GameManager.instance.playerActors[i].playerActor, usedTiles[^1]);
            }
        }*/
        if (!fix)
        {
            tries = 0;
            int enemyCount = usedTiles.Count;
            for (int i = 0; i < GameManager.instance.armyData.armyFormation.Count; i++)
            {
                if (GameManager.instance.armyData.armyFormation[i] == "none")
                {
                    continue;
                }
                // Randomly spawn in the player team.
                GenerateRandomLocation();
                // Spawn the player as soon as you get a location for them.
                LoadPlayerTeamMember(GameManager.instance.armyData.armyFormation[i], usedTiles[^1]);
            }
        }
        if (fix)
        {
            int column = 0;
            int row = 0;
            for (int i = 0; i < GameManager.instance.armyData.armyFormation.Count; i++)
            {
                string actorType = GameManager.instance.armyData.armyFormation[i];
                int rowColLoc = row*terrainMap.fullSize+(column);
                LoadPlayerTeamMember(actorType, rowColLoc);
                column++;
                if (column >= 3)
                {
                    column = 0;
                    row++;
                }
            }
        }
    }

    private void LoadPlayerTeamMemberFromActor(TacticActor actor, int location)
    {
        LoadActor(actor, location);
    }

    private void LoadPlayerTeamMember(string type, int location)
    {
        if (type == "Player")
        {
            LoadActor(GameManager.instance.player.playerActor, location);
        }
        else if (type == "Familiar")
        {
            LoadActor(GameManager.instance.familiar.playerActor, location);
        }
        else if (type == "none")
        {
            return;
        }
        else
        {
            GenerateActor(location, type, 0);
        }
    }

    public void LoadEnemyTeam()
    {
        int type = GameManager.instance.battleLocationType;
        int difficulty = GameManager.instance.battleDifficulty;
        string enemyGroup = groupsData.ReturnEnemyGroup(type, difficulty);
        string[] enemies = enemyGroup.Split(",");
        tries = 0;
        usedTiles.Clear();
        unusableTiles.Clear();
        FindUnusableTiles();
        for (int i = 0; i < enemies.Length; i++)
        {
            GenerateRandomLocation();
        }
        for (int i = 0; i < Mathf.Min(usedTiles.Count, enemies.Length); i++)
        {
            GenerateActor(usedTiles[i], enemies[i], 1);
        }
    }

    public void LoadFixedEnemyTeam()
    {
        List<string> allEnemies = GameManager.instance.fixedBattleActors;
        for (int i = 0; i < allEnemies.Count; i++)
        {
            if (allEnemies[i].Length < 3)
            {
                continue;
            }
            GenerateActor(i, allEnemies[i], 1);
        }
    }

    private bool GenerateRandomLocation()
    {
        int fullSize = terrainMap.fullSize;
        if (tries > fullSize * fullSize){return false;}
        int row = Random.Range(0,fullSize);
        int column = Random.Range(0,fullSize);
        int index = column + (row*fullSize);
        if (!usedTiles.Contains(index) && !unusableTiles.Contains(index))
        {
            usedTiles.Add(index);
        }
        else
        {
            tries++;
            GenerateRandomLocation();
        }
        return true;
    }

    private void FindUnusableTiles()
    {
        for (int i = 0; i < terrainMap.terrainInfo.Count; i++)
        {
            int tileType = terrainMap.terrainInfo[i];
            if (tileType == 7)
            {
                unusableTiles.Add(i);
            }
        }
    }

    public void LoadSkillData(TacticActiveSkill tacticActive, string skillName)
    {
        skillData.LoadDataForSkill(tacticActive, skillName);
    }

    public void LoadActor(TacticActor actorToCopy, int location, int team = 0)
    {
        TacticActor newActor = Instantiate(actorPrefab, transform.position, new Quaternion(0, 0, 0, 0));
        newActor.CopyStats(actorToCopy);
        newActor.InitialLocation(location);
        UpdateActorSprite(newActor, actorToCopy.typeName);
        newActor.team = team;
        newActor.SetMap(terrainMap);
        terrainMap.AddActor(newActor);
        newActor.QuickStart();
    }

    public void GenerateActor(int location, string name = "Mob", int team = 0, bool start = true)
    {
        TacticActor newActor = Instantiate(actorPrefab, transform.position, new Quaternion(0, 0, 0, 0));
        newActor.typeName = name;
        actorData.LoadActorData(newActor, name);
        newActor.InitialLocation(location);
        UpdateActorSprite(newActor, name);
        newActor.team = team;
        if (team == 0)
        {
            GameManager.instance.upgradeData.AdjustUnitStats(newActor);
        }
        newActor.SetMap(terrainMap);
        terrainMap.AddActor(newActor, start);
        newActor.QuickStart();
    }

    public TacticActor GenerateCopyActor(TacticActor copiedActor)
    {
        TacticActor newActor = Instantiate(actorPrefab, transform.position, new Quaternion(0, 0, 0, 0));
        newActor.CopyStats(copiedActor);
        newActor.QuickStart();
        return newActor;
    }

    private void UpdateActorSprite(TacticActor actor, string spriteName)
    {
        // Need the sprite dictionary later.
        Sprite tempSprite = SpriteDictionary(spriteName);
        if (tempSprite != null)
        {
            actor.SetSprite(tempSprite);
        }
        else
        {
            actor.SetSprite(actorSprites.allSprites[0]);
        }
    }

    public void ReturnToHub(bool win = true)
    {
        if (win)
        {
            ClaimDrops();
        }
        else
        {
            ResetDrops();
        }
        GameManager.instance.MoveScenes("BattleOver");
    }

    private void ClaimDrops()
    {
        GameManager.instance.GainResource(0, collectedBlood);
        GameManager.instance.GainResource(1, collectedMana);
        GameManager.instance.GainResource(2, collectedGold);
        GameManager.instance.recentlyGainedBlood = collectedBlood;
        GameManager.instance.recentlyGainedMana = collectedMana;
        GameManager.instance.recentlyGainedGold = collectedGold;
        GameManager.instance.recentlyWon = 1;
    }

    private void ResetDrops()
    {
        GameManager.instance.recentlyGainedBlood = 0;
        GameManager.instance.recentlyGainedMana = 0;
        GameManager.instance.recentlyGainedGold = 0;
        GameManager.instance.recentlyWon = 0;
    }

    public void GetDrops(TacticActor actor)
    {
        switch (actor.dropType)
        {
            case 0:
                collectedBlood += actor.dropAmount;
                break;
            case 1:
                collectedMana += actor.dropAmount;
                break;
            case 2:
                collectedGold += actor.dropAmount;
                break;
        }
    }

    public int WinningTeam()
    {
        // You can always win/lose by being defeated.
        CountTeams();
        if (teamZeroCount <= 0)
        {
            LosePlayerArmy();
            return 1;
        }
        else if (teamOneCount <= 0)
        {
            return 0;
        }
        // There may be other special win/loss conditions though.
        switch (winCondition)
        {
            case 1:
                if (CheckWinConditionOne())
                {
                    return 0;
                }
                break;
            case 2:
                if (!CheckWinConditionTwo())
                {
                    return 1;
                }
                break;
        }
        return -1;
    }

    private bool CheckWinConditionOne()
    {
        // Basic alternate win con, kill a key target.
        for (int i = 0; i < terrainMap.actors.Count; i++)
        {
            if (terrainMap.actors[i].health <= 0)
            {
                continue;
            }
            if (terrainMap.actors[i].typeName == winConditionSpecifics)
            {
                return false;
            }
        }
        // If you can't find the key target then they're dead.
        return true;
    }

    private bool CheckWinConditionTwo()
    {
        // Alternate loss condition, fail to protect a key target.
        for (int i = 0; i < terrainMap.actors.Count; i++)
        {
            if (terrainMap.actors[i].health <= 0)
            {
                continue;
            }
            if (terrainMap.actors[i].typeName == winConditionSpecifics)
            {
                return true;
            }
        }
        // If they die then you lose.
        return false;
    }

    private int CheckTargetSlain()
    {
        return 0;
    }

    private void CountTeams()
    {
        teamOneCount = 0; teamZeroCount = 0;
        for (int i = 0; i < terrainMap.actors.Count; i++)
        {
            if (terrainMap.actors[i].health <= 0){continue;}
            switch (terrainMap.actors[i].team)
            {
                case 0:
                    teamZeroCount++;
                    break;
                case 1:
                    teamOneCount++;
                    break;
            }
        }
    }

    public void SetActorStats(TacticActor tacticActor)
    {
        // health|move|attack|defense|energy|actions
        // excel time.
    }

    // Work on this.
    public void AddBuffDebuff(TacticActor tacticActor, string name)
    {

    }

    public bool BattleBetweenActors(TacticActor attacker, TacticActor defender, bool counter = true, bool flanked = false, int skillPowerMultipler = 0)
    {
        int defenderLocationType = terrainMap.terrainInfo[defender.locationIndex];
        // Encourage attacking.
        int attackAdvantage = attacker.attackDamage*6/5;
        if (skillPowerMultipler > 10)
        {
            attackAdvantage += attacker.attackDamage*skillPowerMultipler/10 - attacker.attackDamage;
        }
        // Check for flanking/ally support.
        if (flanked)
        {
            attackAdvantage += attacker.attackDamage*6/5 - attacker.attackDamage;
        }
        // Calculate terrain bonuses at the end then damage each other.
        int defenderBonus = terrainTile.ReturnDefenseBonus(defenderLocationType, defender.movementType);
        attackAdvantage = attackAdvantage*6/defenderBonus;
        defender.ReceiveDamage(attackAdvantage);
        if (counter)
        {
            int attackerLocationType = terrainMap.terrainInfo[attacker.locationIndex];
            int defenseBonus = terrainTile.ReturnDefenseBonus(attackerLocationType, attacker.movementType);
            int defenderPower = defender.attackDamage*6/defenseBonus;
            // Penalty for ranged defenders.
            if (attacker.currentAttackRange < defender.currentAttackRange)
            {
                defenderPower/=2;
            }
            int attackerHealth = attacker.health;
            int attackerDefense = attacker.defense;
            attacker.ReceiveDamage(defenderPower);
            if (defenderPower - attackerDefense >= attackerHealth || attackerHealth <= 1)
            {
                return true;
            }
        }
        return false;
    }

    private Sprite SpriteDictionary(string spriteName)
    {
        return actorSprites.SpriteDictionary(spriteName);
    }
}
