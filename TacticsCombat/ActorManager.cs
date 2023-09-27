using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActorManager : MonoBehaviour
{
    public List<Sprite> actorSprites;
    public TacticActor actorPrefab;
    public TerrainMap terrainMap;
    private int teamOneCount = 0;
    private int teamZeroCount = 0;
    public TerrainTile terrainTile;
    public ActorDataManager actorData;

    void Start()
    {
        /*for (int i = 0; i < actorSprites.Count; i++)
        {
            Debug.Log(actorSprites[i].name);
        }*/
    }

    public void AddTeamCount(int team)
    {
        switch (team)
        {
            case 0:
                teamZeroCount++;
                break;
            case 1:
                teamOneCount++;
                break;
        }
    }

    public void SubtractTeamCount(int team)
    {
        switch (team)
        {
            case 0:
                teamZeroCount--;
                break;
            case 1:
                teamOneCount--;
                break;
        }
    }

    public void LoadPlayerTeam()
    {
        LoadActor(GameManager.instance.player.playerActor, 1);
        LoadActor(GameManager.instance.familiar.playerActor, 0);
    }

    public void LoadActor(TacticActor actorToCopy, int location, int team = 0)
    {
        TacticActor newActor = Instantiate(actorPrefab, transform.position, new Quaternion(0, 0, 0, 0));
        newActor = actorToCopy;
        newActor.InitialLocation(location);
        newActor.team = team;
        AddTeamCount(team);
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
        AddTeamCount(team);
        newActor.SetMap(terrainMap);
        terrainMap.AddActor(newActor);
        newActor.QuickStart();
    }

    public TacticActor ReturnCurrentTarget()
    {
        return terrainMap.ReturnCurrentTarget();
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
            actor.SetSprite(actorSprites[0]);
        }
    }

    public void ReturnToHub()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Hub");
    }

    public int WinningTeam()
    {
        if (teamOneCount > 0 && teamZeroCount <= 0)
        {
            return 1;
        }
        else if (teamOneCount <= 0 && teamZeroCount > 0)
        {
            return 0;
        }
        return -1;
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

    public void BattleBetweenActors(TacticActor attacker, TacticActor attackee, bool counter = true)
    {
        int attackeeLocationType = terrainMap.terrainInfo[attackee.locationIndex];
        // Encourage attacking.
        int attackAdvantage = attacker.attackDamage*6/5;
        // Calculate terrain bonuses at the end then damage each other.
        int attackerPower = attackAdvantage*6/terrainTile.TerrainDefenseBonus(attackeeLocationType);
        attackee.ReceiveDamage(attackerPower);
        if (counter)
        {
            int attackerLocationType = terrainMap.terrainInfo[attacker.locationIndex];
            int attackeePower = attackee.attackDamage*6/terrainTile.TerrainDefenseBonus(attackerLocationType);
            // Penalty for ranged defenders.
            if (attacker.attackRange < attackee.attackRange)
            {
                attackeePower/=2;
            }
            attacker.ReceiveDamage(attackeePower);
        }
    }

    private Sprite SpriteDictionary(string spriteName)
    {
        for (int i = 0; i < actorSprites.Count; i++)
        {
            if (actorSprites[i].name == spriteName)
            {
                return actorSprites[i];
            }
        }
        return null;
    }
}
