using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOrderPanel : MonoBehaviour
{
    private int turnOrderCount = 6;
    public List<FormationTile> turnOrder;
    public TerrainMap terrainMap;

    private void ResetTurnOrder()
    {
        for (int i = 0; i < turnOrderCount; i++)
        {
            turnOrder[i].ResetActorSprite();
        }
    }

    private void UpdateTurnSprite(int index, Sprite newSprite)
    {
        turnOrder[index].UpdateActorSprite(newSprite);
    }

    public void UpdateTurnOrder(int currentTurnIndex)
    {
        ResetTurnOrder();
        int updateIndex = 0;
        // Could be a problem if too many dead actors, you won't show all the actors.
        for (int i = 0; i < Mathf.Min(terrainMap.actors.Count, turnOrderCount); i++)
        {
            int actorIndex = (currentTurnIndex+i)%terrainMap.actors.Count;
            // Don't draw dead actors.
            if (terrainMap.actors[actorIndex].health <= 0)
            {
                continue;
            }
            UpdateTurnSprite(updateIndex, terrainMap.actors[actorIndex].spriteRenderer.sprite);
            updateIndex++;
        }
    }
}
