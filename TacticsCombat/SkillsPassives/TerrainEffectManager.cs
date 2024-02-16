using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainEffectManager : MonoBehaviour
{
    public List<int> terrainInfo;
    public List<int> tileEffects;
    public int fireBaseDamage = 6;

    public bool TerrainTypeAffectable(int terrainType, int terrainEffect)
    {
        switch (terrainEffect)
        {
            // Fire can't start on water tiles.
            case 0:
                if (terrainType == 3 || terrainType == 4){return false;}
                return true;
        }
        return true;
    }

    public void SetTerrainInfo(List<int> newTiles)
    {
        terrainInfo = newTiles;
    }

    public void SetTileEffects(List<int> newEffects)
    {
        tileEffects = newEffects;
    }

    public List<int> UpdateTileEffects(List<int> newTiles, List<int> newEffects)
    {
        SetTerrainInfo(newTiles);
        SetTileEffects(newEffects);
        for (int i = 0; i < tileEffects.Count; i++)
        {
            CheckOnTileEffect(i);
        }
        return tileEffects;
    }

    protected void CheckOnTileEffect(int tileNumber)
    {
        if (tileEffects[tileNumber] < 0){return;}
        // Fire has a chance of going out every turn.
        switch (tileEffects[tileNumber])
        {
            case 0:
                CheckOnFire(tileNumber);
                break;
        }
    }

    protected void ResetTileEffect(int tileNumber)
    {
        tileEffects[tileNumber] = -1;
    }

    protected void CheckOnFire(int tileNumber)
    {
        switch (terrainInfo[tileNumber])
        {
            // Fire burns well on forests.
            case 1:
                return;
            // Fire can't exist on water.
            case 3:
                ResetTileEffect(tileNumber);
                return;
            case 4:
                ResetTileEffect(tileNumber);
                return;
        }
        // Otherwise, random chance that it goes out.
        int cont = Random.Range(0, fireBaseDamage);
        // 0 -> goes out.
        if (cont == 0)
        {
            ResetTileEffect(tileNumber);
            return;
        }
        // Otherwise chance it spreads?
        // Deal with it later.
    }

    // This is the base tile effect, more permanent, ie chasms.
    public void BaseTerrainEffect(TacticActor actor, int terrainType)
    {
        switch (terrainType)
        {
            case 7:
                ChasmTerrain(actor);
                break;
        }
    }

    protected void ChasmTerrain(TacticActor actor)
    {
        ActionLog.instance.AddActionLog(actor.ReturnName()+" is over a chasm.");
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

    // These are more temporary tile effects, like fire, burns but goes out.
    public void SpecialTerrainEffect(TacticActor actor, int locationEffect)
    {
        if (locationEffect < 0){return;}
        switch (locationEffect)
        {
            case 0:
                FireEffect(actor);
                break;
        }
    }

    protected void FireEffect(TacticActor actor)
    {
        // Bigger things get more burns?
        ActionLog.instance.AddActionLog(actor.ReturnName()+" is surrounded by fire.");
        int multiplier = Mathf.Max(1, actor.size);
        int damage = Random.Range(fireBaseDamage*multiplier/2, fireBaseDamage*multiplier*3/2);
        // Fire comes from all sides and is damage type 1.
        actor.ReceiveDamage(damage + actor.defense, -1, 1);
        // Suffocation?
    }
}
