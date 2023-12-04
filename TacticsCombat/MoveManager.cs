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
        pathfinder.RecurviseAdjacency(actor.locationIndex);
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

    private int LineCheck(int locOne, int locTwo)
    {
        return pathfinder.SameLine(locOne, locTwo);
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

    private void ForceMovement(TacticActor actor, int direction)
    {
        // Need to check if the direction is occupied or out of bounds.
        // This checks if it's in bounds.
        int destination = GetDestination(actor.locationIndex, direction);
        // This checks if it's occupied.
        if (pathfinder.CheckTileMoveable(actor, destination))
        {
            actor.locationIndex = destination;
        }
    }

    public void Displace(TacticActor displacer, TacticActor displacedActor, int displacementPower, string pushorpull = "Push")
    {
        int push = 1;
        if (pushorpull != "Push")
        {
            push = 0;
        }
        int line = LineCheck(displacer.locationIndex, displacedActor.locationIndex);
        int power = displacementPower + (displacer.weight) - (displacedActor.weight);
        if (power <= 0){return;}
        if (line < 0){return;}
        else if (line == 0)
        {
            DisplaceHorizontally(displacedActor, displacer.locationIndex, power, push);
        }
        else if (line == 1)
        {
            DisplaceVertically(displacedActor, displacer.locationIndex, power, push);
        }
    }

    private void DisplaceHorizontally(TacticActor displaced, int displacerLocation, int power, int direction)
    {
        // Check if the displacer is left of the target.
        if (pathfinder.Left(displacerLocation, displaced.locationIndex))
        {
            // Push them right.
            if (direction == 1)
            {
                for (int i = 0; i < power; i++)
                {
                    ForceMovement(displaced, 1);
                }
            }
            // Pull them left.
            else
            {
                for (int i = 0; i < power; i++)
                {
                    ForceMovement(displaced, 3);
                }
            }
        }
        // Otherwise the displacer started right of the target.
        else
        {
            // Push them left.
            if (direction == 1)
            {
                for (int i = 0; i < power; i++)
                {
                    ForceMovement(displaced, 3);
                }
            }
            // Pull them right.
            else
            {
                for (int i = 0; i < power; i++)
                {
                    ForceMovement(displaced, 1);
                }
            }
        }
    }

    private void DisplaceVertically(TacticActor displaced, int displacerLocation, int power, int direction)
    {
        // Check if the displacer is above of the target.
        if (pathfinder.Up(displacerLocation, displaced.locationIndex))
        {
            // Push them down.
            if (direction == 1)
            {
                for (int i = 0; i < power; i++)
                {
                    ForceMovement(displaced, 2);
                }
            }
            // Pull them up.
            else
            {
                for (int i = 0; i < power; i++)
                {
                    ForceMovement(displaced, 0);
                }
            }
        }
        // Otherwise the displacer started below the target.
        else
        {
            // Push them up.
            if (direction == 1)
            {
                for (int i = 0; i < power; i++)
                {
                    ForceMovement(displaced, 0);
                }
            }
            // Pull them down.
            else
            {
                for (int i = 0; i < power; i++)
                {
                    ForceMovement(displaced, 2);
                }
            }
        }
    }

    public void MoveActorToTile(TacticActor actor, int location, int moveCost)
    {
        actor.locationIndex = location;
        actor.movement -= moveCost;
        while (actor.movement < 0)
        {
            actor.UseActionsOnMovement();
        }
    }

    public void Teleport(TacticActor actor, int location)
    {
        actor.locationIndex = location;
    }
}
