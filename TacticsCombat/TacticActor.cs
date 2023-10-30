using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TacticActor : MonoBehaviour
{
    // 0 is player's team, other teams are NPCs.
    public int team = 0;
    // Basic enemies drop 1 gold.
    public int dropType = 2;
    public int dropAmount = 1;
    public bool delayed = false;
    // Race/class/etc.
    public string typeName;
    public int locationIndex;
    public int level;
    public int movementType = 0;
    public int health;
    public int baseHealth = 20;
    // Not sure if we need an initiative tracker it might make things more complex.
    //private int initiative = 0;
    public int baseEnergy = 5;
    public int energy;
    public int baseMovement = 3;
    public int currentMovespeed;
    public int baseActions = 2;
    public int actionsLeft;
    public int attackRange = 1;
    public int actionsToAttack = 1;
    public int currentAttackRange;
    public int baseAttack = 10;
    public int attackDamage;
    public int baseDefense = 5;
    public int defense;
    public int movement;
    // 0 is offensive, 1 is defensive
    public int AIType;
    private int destinationIndex;
    private TacticActor attackTarget;
    public SpriteRenderer spriteRenderer;
    public List<int> currentPath;
    public TerrainMap terrainMap;
    public List<string> buffDebuffNames;
    public List<int> buffDebuffsDurations;
    public List<string> passiveNames;
    public List<string> activeSkillNames;
    public string npcMoveSkill;
    public string npcAttackSkill;
    public string npcSupportSkill;
    public TacticActiveSkill activeSkill;
    public TacticBuffsStatuses buffDebuff;

    void Start()
    {
        health = baseHealth;
    }

    // Since the text is loading before start is called.
    public void QuickStart()
    {
        health = baseHealth;
        currentMovespeed = baseMovement;
        energy = baseEnergy;
        attackDamage = baseAttack;
        defense = baseDefense;
        currentAttackRange = attackRange;
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

    public void CopyStats(TacticActor actorToCopy)
    {
        level = actorToCopy.level;
        typeName = actorToCopy.typeName;
        baseHealth = actorToCopy.baseHealth;
        baseMovement = actorToCopy.baseMovement;
        baseAttack = actorToCopy.baseAttack;
        baseDefense = actorToCopy.baseDefense;
        baseEnergy = actorToCopy.baseEnergy;
        baseActions = actorToCopy.baseActions;
        attackRange = actorToCopy.attackRange;
        movementType = actorToCopy.movementType;
        activeSkillNames = actorToCopy.activeSkillNames;
        SetSprite(actorToCopy.spriteRenderer.sprite);
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

    public void StartTurn()
    {
        if (delayed)
        {
            delayed = false;
            return;
        }
        attackDamage = baseAttack;
        actionsLeft = baseActions;
        defense = baseDefense;
        currentMovespeed = baseMovement;
        currentAttackRange = attackRange;
        energy = Mathf.Min(energy+1, baseEnergy);
        ApplyBuffDebuffEffects();
        movement = 0;
    }

    private void Death()
    {
        Color tempColor = Color.white;
        tempColor.a = 0f;
        spriteRenderer.sprite = null;
        spriteRenderer.color = tempColor;
        actionsLeft = 0;
        terrainMap.ActorDied();
        //Destroy(gameObject);
    }

    public void TriggerAggro()
    {
        if (AIType == 1)
        {
            AIType = 0;
        }
    }

    public void ReceiveDamage(int amount)
    {
        TriggerAggro();
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

    public void ActivateSkill()
    {
        UseActionsBesidesMovement(activeSkill.ReturnActionCost(this));
        LoseEnergy(activeSkill.cost);
    }

    private void ApplyBuffDebuffEffects()
    {
        if (buffDebuffNames.Count <= 0)
        {
            return;
        }
        for (int i = 0; i < buffDebuffNames.Count; i++)
        {
            buffDebuff.LoadEffectName(buffDebuffNames[i]);
            buffDebuff.AffectActor(this);
            buffDebuffsDurations[i]--;
            if (buffDebuffsDurations[i] <= 0)
            {
                buffDebuffsDurations.RemoveAt(i);
                buffDebuffNames.RemoveAt(i);
            }
        }
        /*for (int j = 0; j < buffDebuffsDurations.Count; j++)
        {
            buffDebuffsDurations[i]--;
            if (buffDebuffsDurations[i] <= 0)
            {
                buffDebuffsDurations.RemoveAt(i);
                buffDebuffNames.RemoveAt(i);
            }
        }*/
    }

    public void ApplyNewlyAddedBuffDebuffEffect()
    {
        buffDebuff.LoadEffectName(buffDebuffNames[^1]);
        buffDebuff.AffectActor(this);
    }

    public string LoadSkillName(int skillIndex)
    {
        return (activeSkillNames[skillIndex]);
    }

    private void NPCLoadSkill(int type)
    {
        // 0 = move, 1 = attack, 2 = support
        string skillName = "";
        switch (type)
        {
            case 0:
                skillName = npcMoveSkill;
                break;
            case 1:
                skillName = npcAttackSkill;
                break;
            case 2:
                skillName = npcSupportSkill;
                break;
        }
        terrainMap.actorManager.LoadSkillData(activeSkill, skillName);
    }

    public bool Delayable()
    {
        if (movement <= 0 && actionsLeft <= 0)
        {
            return false;
        }
        return true;
    }

    public bool CheckActions(int actionCost = 1)
    {
        if (actionsLeft < actionCost)
        {
            return false;
        }
        return true;
    }

    public bool CheckSkillActivatable()
    {
        // No such thing as a skill that costs zero energy.
        if (actionsLeft < activeSkill.ReturnActionCost(this) || energy < activeSkill.cost || activeSkill.cost <= 0)
        {
            return false;
        }
        return true;
    }

    public bool CheckIfAnySkillActivateable()
    {
        int cost = energy + 1;
        for (int i = 0; i < activeSkillNames.Count; i++)
        {
            terrainMap.actorManager.LoadSkillData(activeSkill, activeSkillNames[i]);
            if (activeSkill.cost < cost)
            {
                cost = activeSkill.cost;
            }
        }
        if (cost <= energy)
        {
            return true;
        }
        return false;
    }

    private void AttackAction()
    {
        if (actionsLeft < 1 || health <= 0)
        {
            return;
        }
        NPCLoadSkill(1);
        // Try to hit the first target.
        if (terrainMap.pathFinder.CalculateDistance(locationIndex, attackTarget.locationIndex) <= currentAttackRange)
        {
            if (CheckSkillActivatable())
            {
                terrainMap.NPCActivateSkill(attackTarget.locationIndex);
                ActivateSkill();
                CheckIfAttackAgain();
                return;
            }
            terrainMap.NPCActorAttack(attackTarget);
            actionsLeft -= 1;
        }
        // Otherwise look for another target.
        else
        {
            attackTarget = terrainMap.ReturnEnemyInRange(this);
            if (attackTarget == null){return;}
            if (CheckSkillActivatable())
            {
                terrainMap.NPCActivateSkill(attackTarget.locationIndex);
                ActivateSkill();
                CheckIfAttackAgain();
                return;
            }
            terrainMap.NPCActorAttack(attackTarget);
            actionsLeft -= 1;
        }
        // Keep attacking until you're out of actions.
        CheckIfAttackAgain();
    }

    private void CheckIfAttackAgain()
    {
        if (actionsLeft >= actionsToAttack)
        {
            AttackAction();
        }
    }

    public bool CheckActionsToAttack()
    {
        if (actionsLeft >= actionsToAttack)
        {
            return true;
        }
        return false;
    }

    private void SupportAction()
    {
        if (actionsLeft <= 0 || health <= 0)
        {
            return;
        }
        NPCLoadSkill(2);
        if (CheckSkillActivatable())
        {
            // Support skills are always cast on oneself?
            // We can make different AI's later.
            terrainMap.NPCActivateSkill(locationIndex);
            ActivateSkill();
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
        if (AIType == 1)
        {
            SupportAction();
            return;
        }
        // Pick a target, based on goals.
        CheckGoal();
        GetPath();
        MoveAction();
        // Check dps skill.
        AttackAction();
        SupportAction();
    }

    private void CheckGoal()
    {
        // Randomly move around if you don't have a target.
        // Look for a target in range.
        UpdateTarget(terrainMap.FindClosestEnemyInAttackRange());
        if (attackTarget == null)
        {
            UpdateTarget(terrainMap.FindNearestEnemy());
        }
        // Otherwise go for the closest enemy.
        UpdateDest(attackTarget.locationIndex);
        // Attack your target if you have one.
        // If you're injured then start looking for targets? Depends on the type of AI.
    }

    private void CheckEnemyInRange()
    {

    }

    public void UpdateDest(int newDest)
    {
        destinationIndex = newDest;
    }

    public void MoveAction()
    {
        if (Moveable())
        {
            MoveAction();
            terrainMap.UpdateOnActorTurn();
        }
        /*else
        {
            // If you can't move anymore but the target is still not in range then try to use a movement skill.
            if (!terrainMap.CheckTargetInRange(locationIndex, attackTarget, attackRange))
            {
                NPCLoadSkill(0);
                if (CheckSkillActivatable())
                {
                    terrainMap.NPCActivateSkill(locationIndex);
                    ActivateSkill();
                    MoveAction();
                }
            }
        }*/
    }

    private bool Moveable()
    {
        if (currentPath.Count <= 0 || currentPath[0] == locationIndex || health <= 0 || actionsLeft <= 0)
        {
            return false;
        }
        if (terrainMap.CheckTargetInRange(this, attackTarget))
        {
            return false;
        }
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
        int distance = terrainMap.ReturnMoveCost(index, movementType);
        if (distance > movement)
        {
            CheckIfDistanceIsCoverable(distance);
        }
        if (distance <= movement)
        {
            movement -= distance;
            return true;
        }
        return false;
    }

    public bool CheckIfDistanceIsCoverable(int distance)
    {
        int max_movement = movement + (currentMovespeed * actionsLeft);
        if (max_movement >= distance)
        {
            while (movement < distance && actionsLeft > 0)
            {
                UseActionsOnMovement();
            }
            return true;
        }
        return false;
    }

    public int ReturnMaxPossibleDistance(bool current = false)
    {
        if (!current)
        {
            return currentMovespeed * baseActions;
        }
        return movement + (currentMovespeed * actionsLeft);
    }

    public int MaxMovePerTurn()
    {
        // Doesn't work if there are some effects decreasing actions.
        return currentMovespeed * baseActions;
    }

    public int MaxMovementWhileAttacking(bool current = false)
    {
        if (current)
        {
            return movement + (currentMovespeed * (actionsLeft - 1));
        }
        return currentMovespeed * (baseActions - 1);
    }

    public void UseActionsOnMovement()
    {
        actionsLeft--;
        movement += currentMovespeed;
    }

    public void UseActionsBesidesMovement(int actionCost)
    {
        actionsLeft -= actionCost;
    }

}
