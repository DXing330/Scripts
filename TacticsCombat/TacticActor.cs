using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TacticActor : MonoBehaviour
{
    // 0 is player's team, other teams are NPCs.
    public int team = 0;
    // Race/class/etc.
    public string typeName;
    public int locationIndex;
    public int level;
    //private int movementType = 0;
    public int health;
    public int baseHealth = 20;
    // Not sure if we need an initiative tracker it might make things more complex.
    //private int initiative = 0;
    public int baseEnergy = 5;
    public int energy;
    public int baseMovement = 3;
    public int baseActions = 1;
    public int actionsLeft;
    public int attackRange = 1;
    public int baseAttack = 10;
    public int attackDamage;
    public int baseDefense = 5;
    public int defense;
    public int movement;
    private int destinationIndex;
    private TacticActor attackTarget;
    public SpriteRenderer spriteRenderer;
    public List<int> currentPath;
    public TerrainMap terrainMap;
    public List<string> buffDebuffNames;
    public List<int> buffDebuffsDurations;
    public List<string> passiveNames;
    public List<string> activeSkillNames;
    public TacticActiveSkill activeSkill;
    //public List<TacticActiveSkill> activeSkills;

    void Start()
    {
        health = baseHealth;
        movement = baseMovement;
    }

    // Since the text is loading before start is called.
    public void QuickStart()
    {
        health = baseHealth;
        movement = baseMovement;
    }

    public void SetBaseStats(string baseStats, int newLevel = 1)
    {
        level = newLevel;
        string[] newBase = baseStats.Split("|");
        baseHealth = int.Parse(newBase[0]);
        baseMovement = int.Parse(newBase[1]);
        baseAttack = int.Parse(newBase[2]);
        baseDefense = int.Parse(newBase[3]);
        baseEnergy = int.Parse(newBase[4]);
        baseActions = int.Parse(newBase[5]);
    }

    public void SetSprite(Sprite newSprite)
    {
        spriteRenderer.sprite = newSprite;
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
        health -= Mathf.Max(amount - defense, 1);
        if (health <= 0)
        {
            Death();
        }
    }

    public void RegainHealth(int amount)
    {
        health += amount;
        if (health > baseHealth)
        {
            health = baseHealth;
        }
    }

    public void LoseEnergy(int amount)
    {
        energy -= Mathf.Min(energy, amount);
    }

    public void GainEnergy(int amount)
    {
        energy += amount;
    }

    public void ActivateSkill(int skillIndex)
    {
        actionsLeft--;
        activeSkill.LoadSkill(activeSkillNames[skillIndex]);
        LoseEnergy(activeSkill.cost);
    }

    public void LoadSkill(int skillIndex)
    {
        activeSkill.LoadSkill(activeSkillNames[skillIndex]);
    }

    public bool CheckActions()
    {
        if (actionsLeft < 1)
        {
            return false;
        }
        return true;
    }

    public bool CheckSkillActivatable(int skillIndex)
    {
        if (actionsLeft < 1)
        {
            return false;
        }
        LoadSkill(skillIndex);
        if (energy < activeSkill.cost)
        {
            return false;
        }
        return true;
    }

    // Player attack.
    public void Attack(TacticActor target)
    {
        if (target ==  null || actionsLeft <= 0)
        {
            return;
        }
        // Check if target is in attack range?
        target.ReceiveDamage(attackDamage);
        actionsLeft--;
    }

    // NPC attack.
    private void AttackTarget()
    {
        while (actionsLeft > 0)
        {
            Attack(attackTarget);
            actionsLeft--;
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
        attackDamage =baseAttack;
        movement = baseMovement;
        actionsLeft = baseActions;
        defense = baseDefense;
        energy = Mathf.Min(energy+1, baseEnergy);
        // Deal with buffs/debuffs/passives.
        for (int i = 0; i < buffDebuffNames.Count; i++)
        {
            /*buffDebuffs[i].AffectActor(this);
            buffDebuffs[i].duration--;
            if (buffDebuffs[i].duration <= 0)
            {
                buffDebuffs.RemoveAt(i);
            }*/
        }
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
