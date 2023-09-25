using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TacticActor : MonoBehaviour
{
    // 0 is player's team, other teams are NPCs.
    public int team = 0;
    // Race/class/etc.
    public int type = 0;
    public int locationIndex;
    //private int movementType = 0;
    public int health;
    public int maxHealth = 6;
    // Not sure if we need an initiative tracker it might make things more complex.
    //private int initiative = 0;
    private int maxMovement = 5;
    private int maxAttacks = 12;
    private int attacksLeft;
    public int attackRange = 3;
    private int attackDamage = 1;
    public int movement;
    private int destinationIndex;
    private TacticActor attackTarget;
    public Image image;
    public List<int> currentPath;
    public TerrainMap terrainMap;

    void Start()
    {
        health = maxHealth;
        movement = maxMovement;
    }

    public void InitialLocation(int location)
    {
        locationIndex = location;
        destinationIndex = location;
    }

    private void Death()
    {
        terrainMap.RemoveActor(this);
        Destroy(gameObject);
    }

    public void ReceiveDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Death();
        }
    }

    public void RegainHealth(int amount)
    {
        health += amount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    public void Attack(TacticActor target)
    {
        if (target ==  null || attacksLeft <= 0)
        {
            return;
        }
        // Check if target is in attack range?
        target.ReceiveDamage(attackDamage);
        attacksLeft--;
    }

    private void AttackTarget()
    {
        while (attacksLeft > 0)
        {
            Attack(attackTarget);
            attacksLeft--;
        }
    }

    private void AttackAction()
    {
        if (terrainMap.pathFinder.CalculateDistance(locationIndex, attackTarget.locationIndex) <= attackRange)
        {
            AttackTarget();
        }
    }

    private void UpdateTarget(TacticActor newTarget)
    {
        attackTarget = newTarget;
    }

    public void SetMap(TerrainMap newMap)
    {
        terrainMap = newMap;
    }

    private void GetPath()
    {
        currentPath = terrainMap.pathFinder.FindPathIndex(locationIndex, destinationIndex);
    }

    public void NPCStartTurn()
    {
        StartTurn();
        // Pick a target, based on goals.
        CheckGoal();
        GetPath();
        MoveAction();
        //AttackAction();
    }

    public void StartTurn()
    {
        movement = maxMovement;
        attacksLeft = maxAttacks;
    }

    private void CheckGoal()
    {
        // Randomly move around if you don't have a target.
        if (terrainMap.CheckAdjacency(locationIndex, destinationIndex))
        {
            UpdateDest(terrainMap.RandomDestination(locationIndex));
        }
        // Attack your target if you have one.
        // If you're injured then start looking for targets? Depends on the type of AI.
    }

    public void UpdateDest(int newDest)
    {
        destinationIndex = newDest;
    }

    public void MoveAction()
    {
        if (currentPath[0] == locationIndex)
        {
            return;
        }
        if (Moveable())
        {
            MoveAction();
            terrainMap.UpdateOnActorTurn();
        }
    }

    private bool Moveable()
    {
        if (currentPath.Contains(locationIndex))
        {
            // Move to the next step on the path.
            int cIndex = currentPath.IndexOf(locationIndex);
            if (CheckDistance(currentPath[cIndex-1]))
            {
                locationIndex = currentPath[cIndex-1];
                return true;
            }
        }
        else
        {
            // Start from the end of the path.
            if (CheckDistance(currentPath[^1]))
            {
                locationIndex = currentPath[^1];
                return true;
            }
        }
        return false;
    }

    private bool CheckDistance(int index)
    {
        int distance = terrainMap.ReturnMoveCost(index);
        //Debug.Log(index);
        //Debug.Log(distance);
        if (distance <= movement)
        {
            movement -= distance;
            return true;
        }
        return false;
    }

}
