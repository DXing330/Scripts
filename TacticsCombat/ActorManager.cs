using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActorManager : MonoBehaviour
{
    public List<Sprite> actorSprites;
    public TacticActor actorPrefab;
    public TerrainMap terrainMap;
    public List<TacticActiveSkill> allSkills;
    private int teamOneCount = 0;
    private int teamZeroCount = 0;

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

    public void GenerateActor(int location, int type = 0, int team = 0)
    {
        TacticActor newActor = Instantiate(actorPrefab, transform.position, new Quaternion(0, 0, 0, 0));
        newActor.InitialLocation(location);
        UpdateActorSprite(newActor, type);
        newActor.team = team;
        AddTeamCount(team);
        newActor.SetMap(terrainMap);
        terrainMap.AddActor(newActor);
    }

    public TacticActor ReturnCurrentTarget()
    {
        return terrainMap.ReturnCurrentTarget();
    }

    private void UpdateActorSprite(TacticActor actor, int spriteIndex)
    {
        actor.SetSprite(actorSprites[spriteIndex]);
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
        int type = tacticActor.type;
        // health|move|attack|defense|energy|actions
        // excel time.
    }

    // Work on this.
    public void AddBuffDebuff(TacticActor tacticActor, string name)
    {

    }
}
