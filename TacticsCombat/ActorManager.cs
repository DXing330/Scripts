using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ActorManager : MonoBehaviour
{
    public ActorSprites actorSprites;
    public TacticActor actorPrefab;
    public TerrainMap terrainMap;
    public int winCondition = 0;
    public string winConditionSpecifics = null;
    public TMP_Text battleGoal;
    public void UpdateBattleGoal(string newGoal = "")
    {
        if (newGoal.Length <= 0)
        {
            battleGoal.text = "Defeat All Enemies";
            return;
        }
        battleGoal.text = newGoal;
    }
    public void ResetBattleGoalText()
    {
        battleGoal.text = "";
    }
    public string winReward;
    private int teamOneCount = 0;
    private int teamZeroCount = 0;
    public TerrainTile terrainTile;
    public ActorDataManager actorData;
    public EnemyGroupsData groupsData;
    public SkillDataManager skillData;
    public PassiveSkillDataManager passiveData;
    private int tries = 0;
    public List<int> usedTiles;
    public List<int> unusableTiles;
    public int collectedGold = 0;
    public int collectedMana = 0;
    public int collectedBlood = 0;
    
    public void LoadBattle(string[] dataBlocks)
    {
        SetWinReward(dataBlocks[1]);
        LoadFixedBattleEnemyTeam(dataBlocks[2].Split("|"), dataBlocks[3].Split("|"));
        SpawnPlayerTeamInFixedSpots(dataBlocks[4].Split("|"));
        LoadEnemyInRandomSetLocations(dataBlocks[5].Split("|"), dataBlocks[6].Split("|"));
    }

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
        GameManager.instance.armyData.PartyMemberDefeated(actor.typeName);
    }

    protected void LosePlayerArmy()
    {
        GameManager.instance.armyData.PartyWipe();
    }

    protected void LoadPlayerTeamMember(int index, int location)
    {
        LoadActor(GameManager.instance.armyData.allPartyMembers[index].playerActor, location);
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

    public void LoadFixedBattleEnemyTeam(string[] enemies, string[] locations)
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (locations[i].Length <= 0){continue;}
            GenerateActor(int.Parse(locations[i]), enemies[i], 1);
        }
    }

    public void LoadEnemyInRandomSetLocations(string[] enemies, string[] locations)
    {
        List<string> allLocs = locations.ToList();
        int locationIndex = 0;
        int location = 0;
        for (int i = 0; i < enemies.Length; i++)
        {
            locationIndex = Random.Range(0, allLocs.Count);
            location = int.Parse(allLocs[locationIndex]);
            allLocs.RemoveAt(locationIndex);
            GenerateActor(location, enemies[i], 1);
        }
    }

    public void SpawnPlayerTeamInFixedSpots(string[] spawnPoints)
    {
        usedTiles.Clear();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            usedTiles.Add(int.Parse(spawnPoints[i]));
        }
        for (int i = 0; i < GameManager.instance.armyData.allPartyMembers.Count; i++)
        {
            // Randomly spawn in the player team.
            int spawnIndex = Random.Range(0, usedTiles.Count);
            int spawnPoint = usedTiles[spawnIndex];
            usedTiles.RemoveAt(spawnIndex);
            // Spawn the player as soon as you get a location for them.
            LoadPlayerTeamMember(i, spawnPoint);
        }
    }

    private bool GenerateRandomLocation()
    {
        int rows = terrainMap.totalRows;
        int columns = terrainMap.totalColumns;
        if (tries > rows * columns){return false;}
        int row = Random.Range(0,rows);
        int column = Random.Range(0,columns);
        int index = column + (row*columns);
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

    public void LoadPassiveData(TacticPassiveSkill passive, string passiveName)
    {
        passiveData.LoadDataForPassive(passive, passiveName);
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
        if (name.Length <= 0){return;}
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
            UpdatePartyHealth();
        }
        else
        {
            ResetDrops();
        }
        GameManager.instance.MoveScenes("BattleOver");
    }

    protected void UpdatePartyHealth()
    {
        List<string> partyMembers = new List<string>();
        List<int> partyHealth = new List<int>();
        for (int i = 0; i < terrainMap.actors.Count; i++)
        {
            if (terrainMap.actors[i].team == 0 && terrainMap.actors[i].health > 0)
            {
                partyMembers.Add(terrainMap.actors[i].typeName);
                partyHealth.Add(terrainMap.actors[i].health);
            }
        }
        // Need some way to track duplicate types, ie two wolves.
        // Deal with it in the actual manager.
        GameManager.instance.armyData.UpdatePartyHealth(partyMembers, partyHealth);
    }

    public void SetWinReward(string newReward)
    {
        winReward = newReward;
    }

    private void ClaimDrops()
    {
        GameManager.instance.recentlyWon = 1;
        if (winReward.Length < 5){return;}
        string[] allRewards = winReward.Split("|");
        collectedBlood = int.Parse(allRewards[1]);
        collectedGold = int.Parse(allRewards[0]);
        collectedMana = int.Parse(allRewards[2]);
        GameManager.instance.GainResource(0, collectedBlood);
        GameManager.instance.GainResource(1, collectedMana);
        GameManager.instance.GainResource(2, collectedGold);
        GameManager.instance.recentlyGainedBlood = collectedBlood;
        GameManager.instance.recentlyGainedMana = collectedMana;
        GameManager.instance.recentlyGainedGold = collectedGold;
    }

    private void ResetDrops()
    {
        GameManager.instance.recentlyGainedBlood = 0;
        GameManager.instance.recentlyGainedMana = 0;
        GameManager.instance.recentlyGainedGold = 0;
        GameManager.instance.recentlyWon = 0;
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
        // If the player is defeated its an autoloss?
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
        int attackAdvantage = 1;
        int attackPower = attacker.attackDamage;
        if (skillPowerMultipler > 10)
        {
            attackPower += attacker.attackDamage*skillPowerMultipler/10 - attacker.attackDamage;
        }
        // Check for flanking/ally support.
        if (flanked){attackAdvantage++;}
        // Calculate terrain bonuses at the end then damage each other.
        int defenderBonus = terrainTile.ReturnDefenseBonus(defenderLocationType, defender.movementType);
        attackPower = attackPower*6/defenderBonus;
        int attackDamage = attacker.GenerateAttackDamage(attackAdvantage, attackPower);
        attackDamage = attacker.ApplyAttackingPassives(attackDamage, defender);
        defender.ReceiveDamage(attackDamage, attacker.currentDirection);
        if (counter)
        {
            int attackerLocationType = terrainMap.terrainInfo[attacker.locationIndex];
            int defenseBonus = terrainTile.ReturnDefenseBonus(attackerLocationType, attacker.movementType);
            int defenderPower = defender.attackDamage*6/defenseBonus;
            // Penalty for ranged defenders.
            if (attacker.currentAttackRange < defender.currentAttackRange){defenderPower/=2;}
            int attackerHealth = attacker.health;
            int attackerDefense = attacker.defense;
            int defenderDamageDealt = defender.GenerateAttackDamage(0, defenderPower);
            attacker.ReceiveDamage(defenderDamageDealt, defender.currentDirection);
            if (defenderDamageDealt - attackerDefense >= attackerHealth || attackerHealth <= 1){return true;}
            else if (attackerHealth <= 1 && defenderDamageDealt > attackerDefense/2){return true;}
        }
        return false;
    }

    private Sprite SpriteDictionary(string spriteName)
    {
        return actorSprites.SpriteDictionary(spriteName);
    }
}
