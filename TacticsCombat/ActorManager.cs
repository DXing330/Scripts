using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActorManager : MonoBehaviour
{
    public ActorSprites actorSprites;
    public TacticActor actorPrefab;
    public TerrainMap terrainMap;
    private int teamOneCount = 0;
    private int teamZeroCount = 0;
    public TerrainTile terrainTile;
    public ActorDataManager actorData;
    public EnemyGroupsData groupsData;
    public SkillDataManager skillData;
    public List<int> usedTiles;
    public int collectedGold = 0;
    public int collectedMana = 0;
    public int collectedBlood = 0;

    public void LoadPlayerTeam()
    {
        int column = 0;
        int row = 0;
        for (int i = 0; i < GameManager.instance.armyData.armyFormation.Count; i++)
        {
            string actorType = GameManager.instance.armyData.armyFormation[i];
            LoadPlayerTeamMember(actorType, row, column);
            column++;
            if (column >= 3)
            {
                column = 0;
                row++;
            }
        }
    }

    private void LoadPlayerTeamMember(string type, int row, int column)
    {
        int location = column + (row * terrainMap.fullSize);
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
        usedTiles.Clear();
        for (int i = 0; i < enemies.Length; i++)
        {
            GenerateRandomLocation();
        }
        for (int i = 0; i < enemies.Length; i++)
        {
            GenerateActor(usedTiles[i], enemies[i], 1);
        }
    }

    private void GenerateRandomLocation()
    {
        int fullSize = terrainMap.fullSize;
        int row = Random.Range(3, fullSize);
        int column = Random.Range(3, fullSize);
        int index = column + (row*fullSize);
        if (!usedTiles.Contains(index))
        {
            usedTiles.Add(index);
        }
        else{GenerateRandomLocation();}
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
        newActor.team = team;
        newActor.SetMap(terrainMap);
        terrainMap.AddActor(newActor);
        newActor.QuickStart();
    }

    public void GenerateActor(int location, string name = "Mob", int team = 0)
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
        terrainMap.AddActor(newActor);
        newActor.QuickStart();
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
        CountTeams();
        if (teamZeroCount <= 0)
        {
            return 1;
        }
        else if (teamOneCount <= 0)
        {
            return 0;
        }
        return -1;
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
