using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorManager : MonoBehaviour
{
    public List<Sprite> actorSprites;
    public TacticActor actorPrefab;
    public TerrainMap terrainMap;

    public void GenerateActor(int location, int type = 0, int team = 0)
    {
        TacticActor newActor = Instantiate(actorPrefab, transform.position, new Quaternion(0, 0, 0, 0));
        newActor.InitialLocation(location);
        newActor.team = team;
        newActor.SetMap(terrainMap);
        terrainMap.AddActor(newActor);
    }

    public TacticActor ReturnCurrentTarget()
    {
        return terrainMap.ReturnCurrentTarget();
    }

    public void CurrentActorAttack()
    {
        
    }

    public void ActorAttackActor(TacticActor attacker, TacticActor defender)
    {
        attacker.Attack(defender);
    }
}
