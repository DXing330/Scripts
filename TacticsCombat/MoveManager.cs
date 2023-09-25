using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManager : MonoBehaviour
{
    public TerrainPathfinder pathfinder;
    public MoveMenu moveMenu;

    public void UpdateMoveMenu()
    {
        moveMenu.UpdateText();
    }

    private bool Moveable(int location, int dest, int movement)
    {
        pathfinder.AdjacentFromIndex(location);
        if (!pathfinder.adjacentTiles.Contains(dest))
        {
            return false;
        }
        if (pathfinder.moveCostList[dest] > movement)
        {
            return false;
        }
        return true;
    }

    private bool DirectionCheck(int location, int direction)
    {
        return pathfinder.DirectionCheck(location, direction);
    }

    private int GetDestination(int location, int direction)
    {
        // Don't move if you can't move.
        if (!DirectionCheck(location, direction))
        {
            return location;
        }
        return pathfinder.GetDestination(location, direction);
    }

    public void MoveInDirection(TacticActor actor, int direction)
    {
        int destination = GetDestination(actor.locationIndex, direction);
        if (Moveable(actor.locationIndex, destination, actor.movement))
        {
            actor.locationIndex = destination;
            actor.movement -= pathfinder.moveCostList[destination];
        }
    }
}
