using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOrderPanel : MonoBehaviour
{
    private int turnOrderCount = 6;
    public List<FormationTile> turnOrder;
    public List<int> actorIndices;
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
        actorIndices.Clear();
        int updateIndex = 0;
        // Could be a problem if too many dead actors, you won't show all the actors.
        for (int i = 0; i < Mathf.Min(terrainMap.actors.Count, turnOrderCount); i++)
        {
            int actorIndex = (currentTurnIndex+i)%terrainMap.actors.Count;
            // Don't draw dead actors.
            if (!terrainMap.actors[actorIndex].Actable())
            {
                currentTurnIndex++;
                i--;
                continue;
            }
            UpdateTurnSprite(updateIndex, terrainMap.actors[actorIndex].spriteRenderer.sprite);
            actorIndices.Add(actorIndex);
            updateIndex++;
        }
    }

    public int ReturnActorIndex(int imageIndex)
    {
        if (imageIndex >= actorIndices.Count)
        {
            return -1;
        }
        return actorIndices[imageIndex];
    }

    public List<TacticActor> InitiativeThreadedByTeam(List<TacticActor> allActors)
    {
        List<TacticActor> sortedActors = new List<TacticActor>();
        // Split them into teams.
        List<TacticActor> playerTeam = new List<TacticActor>();
        List<TacticActor> enemyTeam = new List<TacticActor>();
        for (int i = 0; i < allActors.Count; i++)
        {
            switch (allActors[i].team)
            {
                case 0:
                    playerTeam.Add(allActors[i]);
                    break;
                case 1:
                    enemyTeam.Add(allActors[i]);
                    break;
            }
        }
        // Sort them by team.
        playerTeam = SortByInitiative(playerTeam);
        enemyTeam = SortByInitiative(enemyTeam);
        // Recombine them.
        if (playerTeam[0].initiative >= enemyTeam[0].initiative)
        {
            if (playerTeam.Count >= enemyTeam.Count)
            {
                for (int j = 0; j < enemyTeam.Count; j++)
                {
                    sortedActors.Add(playerTeam[j]);
                    sortedActors.Add(enemyTeam[j]);
                }
                if (playerTeam.Count == enemyTeam.Count)
                {
                    return sortedActors;
                }
                // Add the rest of the player team.
                for (int k = enemyTeam.Count; k < playerTeam.Count; k++)
                {
                    sortedActors.Add(playerTeam[k]);
                }
            }
            else
            {
                for (int j = 0; j < playerTeam.Count; j++)
                {
                    sortedActors.Add(playerTeam[j]);
                    sortedActors.Add(enemyTeam[j]);
                }
                // Add the rest of the enemy team.
                for (int k = playerTeam.Count; k < enemyTeam.Count; k++)
                {
                    sortedActors.Add(enemyTeam[k]);
                }
            }
        }
        else
        {
            if (enemyTeam.Count >= playerTeam.Count)
            {
                for (int j = 0; j < playerTeam.Count; j++)
                {
                    sortedActors.Add(enemyTeam[j]);
                    sortedActors.Add(playerTeam[j]);
                }
                if (playerTeam.Count == enemyTeam.Count)
                {
                    return sortedActors;
                }
                for (int k = playerTeam.Count; k < enemyTeam.Count; k++)
                {
                    sortedActors.Add(playerTeam[k]);
                }
            }
            else
            {
                for (int j = 0; j < enemyTeam.Count; j++)
                {
                    sortedActors.Add(enemyTeam[j]);
                    sortedActors.Add(playerTeam[j]);
                }
                for (int k = enemyTeam.Count; k < playerTeam.Count; k++)
                {
                    sortedActors.Add(playerTeam[k]);
                }
            }
        }
        return sortedActors;
    }

    private List<TacticActor> SortByInitiative(List<TacticActor> actors)
    {
        List<int> initiatives = new List<int>();
        for (int i = 0; i < actors.Count; i++)
        {
            initiatives.Add(actors[i].baseInitiative);
        }
        return QuickSortByStat(actors, initiatives, 0, actors.Count-1);
    }

    // Highest to lowest, since highest initiative goes first.
    private List<TacticActor> QuickSortByStat(List<TacticActor> actors, List<int> stats, int leftIndex, int rightIndex)
    {
        int i = leftIndex;
        int j = rightIndex;
        int pivotStat = stats[leftIndex];
        while (i <= j)
        {
            while (stats[i] > pivotStat)
            {
                i++;
            }
            while (stats[j] < pivotStat)
            {
                j--;
            }
            if (i <= j)
            {
                int temp = stats[i];
                stats[i] = stats[j];
                stats[j] = temp;
                TacticActor tempActor = actors[i];
                actors[i] = actors[j];
                actors[j] = tempActor;
                i++;
                j--;
            }
        }
        if (leftIndex < j)
        {
            QuickSortByStat(actors, stats, leftIndex, j);
        }
        if (i < rightIndex)
        {
            QuickSortByStat(actors, stats, i, rightIndex);
        }
        return actors;
    }
}
