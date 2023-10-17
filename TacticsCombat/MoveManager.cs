using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManager : MonoBehaviour
{
    public TerrainPathfinder pathfinder;
    public MoveMenu moveMenu;

    public void UpdateMoveMenu()
    {
        moveMenu.UpdateMovementText();
    }

    private int DetermineMoveCost(int moveType, int destination)
    {
        int cost = 1;
        switch (moveType)
        {
            case 0:
                cost = pathfinder.moveCostList[destination];
                break;
            case 1:
                cost = pathfinder.flyingMoveCosts[destination];
                break;
            case 2:
                cost = pathfinder.ridingMoveCosts[destination];
                break;
            case 3:
                cost = pathfinder.swimmingMoveCosts[destination];
                break;
            case 4:
                cost = pathfinder.scoutingMoveCosts[destination];
                break;
        }
        return cost;
    }

    private bool Moveable(TacticActor actor, int dest)
    {
        pathfinder.AdjacentFromIndex(actor.locationIndex);
        if (!pathfinder.adjacentTiles.Contains(dest))
        {
            return false;
        }
        int distance = DetermineMoveCost(actor.movementType, dest);
        if (distance > actor.movement)
        {
            return actor.CheckIfDistanceIsCoverable(distance);
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
        if (Moveable(actor, destination))
        {
            actor.locationIndex = destination;
            actor.movement -= DetermineMoveCost(actor.movementType, destination);
        }
    }
}
