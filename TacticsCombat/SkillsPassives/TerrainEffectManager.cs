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
        // Fly or die.
        if (actor.movementType == 1)
        {
            actor.LoseEnergy(2);
            // If fliers run out of energy they die.
            if (actor.energy <= 0)
            {
                actor.ReceiveDamage(actor.health + actor.defense);
            }
        }
        else
        {
            actor.ReceiveDamage(actor.health + actor.defense);
        }
    }
}
