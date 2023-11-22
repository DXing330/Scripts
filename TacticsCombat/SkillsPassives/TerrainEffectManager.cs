using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainEffectManager : MonoBehaviour
{
    public void AffectActorOnTerrain(TacticActor actor, int terrainType)
    {
        switch (terrainType)
        {
            case 7:
                ChasmTerrain(actor);
                break;
        }
    }

    private void ChasmTerrain(TacticActor actor)
    {
        Debug.Log(actor.typeName+" is on a chasm.");
        // Fly or die.
        if (actor.movementType == 1){return;}
        else
        {
            actor.ReceiveDamage(actor.health + actor.defense);
        }
    }
}
