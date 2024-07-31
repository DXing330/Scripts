using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TacticActor : AllStats
{
    // 0 is player's team, other teams are NPCs.
    public int team = 0;
    public bool delayed = false;
    public bool delayable = true;
    // Race/class/etc.
    public string realName = "";
    public string typeName;
    public string ReturnName()
    {
        if (realName.Length > 1)
        {
            return realName;
        }
        return typeName;
    }
    public int weight;
    public string species;
    public int locationIndex;
    public int level;
    public int movementType = 0;
    public List<string> movementCosts;
    public void SetMoveCosts(List<string> moveCosts)
    {
        movementCosts = moveCosts;
    }
    public int ReturnMoveCostForTile(int tileType)
    {
        if (tileType >= movementCosts.Count || tileType < 0)
        {
            // Out of bounds error.
            return 999;
        }
        int cost = int.Parse(movementCosts[tileType]);
        //If base cost is less than 0 its impassable.
        if (cost < 0)
        {
            return 999;
        }
        if (passiveSkillNames.Count <= 0){return cost;}
        for (int i = 0; i < passiveSkillNames.Count; i++)
        {
            terrainMap.actorManager.LoadPassiveData(passiveSkill, passiveSkillNames[i]);
            if (passiveSkill.timing != 5){continue;}
            if (passiveSkill.MovingConditions(tileType))
            {
                cost = passiveSkill.AffectMoveCost(cost);
            }
        }
        return cost;
    }
    public int health;
    public int ReturnHealth()
    {
        if (health <= 0){return baseHealth;}
        return health;
    }
    public void ReceiveAttackPassives(TacticActor attacker, int distance = 1)
    {
        if (passiveSkillNames.Count <= 0){return;}
        for (int i = 0; i < passiveSkillNames.Count; i++)
        {
            terrainMap.actorManager.LoadPassiveData(passiveSkill, passiveSkillNames[i]);
            if (passiveSkill.timing != 3){continue;}
            if (passiveSkill.AttackedConditions(this, attacker, distance))
            {
                passiveSkill.AffectActor(attacker);
            }
        }
    }
    protected int ReceiveDamagePassives(int amount, int direction = -1, int type = 0)
    {
        if (passiveSkillNames.Count <= 0){return amount;}
        for (int i = 0; i < passiveSkillNames.Count; i++)
        {
            terrainMap.actorManager.LoadPassiveData(passiveSkill, passiveSkillNames[i]);
            if (passiveSkill.timing != 4){continue;}
            if (passiveSkill.DamagedConditions(this, amount, direction, type))
            {
                amount = passiveSkill.AffectDamage(amount, this);
            }
        }
        return amount;
    }
    public void ReceiveDamage(int amount, int direction = -1, int type = 0)
    {
        // Apply any defensive passives.
        amount = ReceiveDamagePassives(amount, direction, type);
        // Ignore damage that's too weak?
        if (defense/2 > amount)
        {
            terrainMap.actionLog.AddActionLog(typeName+" takes 0 DMG.");
            return;
        }
        int damage = Mathf.Max(amount - defense, 1);
        health -= damage;
        terrainMap.moraleTracker.TakeDamage(damage, baseHealth, team);
        terrainMap.actionLog.AddActionLog(typeName+" takes "+damage+" DMG.");
        ChangeAI();
        if (health <= 0){Death();}
    }
    public int energy;
    public int ReturnEnergy()
    {
        if (energy <= 0){return baseEnergy;}
        return energy;
    }
    public int currentMovespeed;
    public int actionsLeft;
    public int actionsToAttack = 1;
    public int currentAttackRange;
    public int attackDamage;
    public int ApplyAttackingPassives(int damage, TacticActor defender)
    {
        if (passiveSkillNames.Count <= 0){return damage;}
        for (int i = 0; i < passiveSkillNames.Count; i++)
        {
            terrainMap.actorManager.LoadPassiveData(passiveSkill, passiveSkillNames[i]);
            if (passiveSkill.timing != 2){continue;}
            if (passiveSkill.AttackingConditions(this, defender))
            {
                damage = passiveSkill.AffectDamage(damage, defender);
            }
        }
        return damage;
    }
    public int GenerateAttackDamage(int advantage = 0, int attackPower = -1)
    {
        // Most attacks have no advatnage.
        advantage += attackAdvantage;
        // Unless specified, use your normal attack damage.
        if (attackPower <= 0){attackPower = attackDamage;}
        // Can have advantage or disadvantage.
        if (advantage != 0)
        {
            bool stronger = false;
            if (advantage > 0){stronger = true;}
            return RollDamageXTimes(attackPower, Mathf.Abs(advantage), stronger);
        }
        // Basic attacks deal on average half of your max attack power.
        int damage = Random.Range(attackPower/2, attackPower*3/2);
        return damage;
    }
    protected int RollDamageXTimes(int damage, int rolls, bool advantage)
    {
        int lowerBound = damage/2;
        int upperBound = damage*3/2;
        if (advantage)
        {
            lowerBound += rolls;
            upperBound += rolls;
        }
        else
        {
            lowerBound -= rolls;
            upperBound -= rolls;
        }
        int baseDamage = Random.Range(lowerBound, upperBound);
        int newRoll = 0;
        for (int i = 0; i < rolls; i++)
        {
            newRoll = Random.Range(lowerBound, upperBound);
            if (advantage && newRoll > baseDamage){baseDamage = newRoll;}
            else if (!advantage && newRoll < baseDamage){baseDamage = newRoll;}
        }
        if (baseDamage < 0){return 0;}
        return baseDamage;
    }
    public int defense;
    public int movement;
    public int initiative;
    public int attackAdvantage = 0;
    public int counterAttacksLeft = 1;
    public int currentDirection = -1;
    public void ChangeDirection(int newDirection)
    {
        // Later can have passives/etc. that mess with this.
        currentDirection = newDirection;
    }
    // 0 is offensive, 1 is passive, 2 is fleeing
    public int AIType = 0;
    public int destinationIndex;
    public TacticActor attackTarget;
    public SpriteRenderer spriteRenderer;
    // Path to target.
    public List<int> currentPath;
    // Tiles moved on turn.
    public List<int> turnPath;
    public TerrainMap terrainMap;
    public List<string> buffDebuffNames;
    public List<int> buffDebuffsDurations;
    public List<string> passiveSkillNames;
    public void LoadPassiveSkills(List<string> newSkills)
    {
        passiveSkillNames.Clear();
        for (int i = 0; i < newSkills.Count; i++)
        {
            if (newSkills[i].Length <= 0){continue;}
            passiveSkillNames.Add(newSkills[i]);
        }
    }
    public List<string> activeSkillNames;
    public void LoadActiveSkills(List<string> newSkills)
    {
        for (int i = 0; i < newSkills.Count; i++)
        {
            if (newSkills[i].Length <= 0){continue;}
            activeSkillNames.Add(newSkills[i]);
        }
    }
    public List<string> temporaryPassives;
    public void GainTempPassive(string newSkill)
    {
        temporaryPassives.Add(newSkill);
        passiveSkillNames.Add(newSkill);
    }
    public List<string> temporaryActives;
    public void GainTempActive(string newSkill)
    {
        temporaryActives.Add(newSkill);
        activeSkillNames.Add(newSkill);
    }
    protected void ResetTempSkills()
    {
        // Remove any temporary passives/actives.
        if (temporaryPassives.Count <= 0 && temporaryActives.Count <= 0){return;}
        string skillName = "";
        for (int i = 0; i < temporaryPassives.Count; i++)
        {
            if (temporaryPassives.Count <= 0){continue;}
            skillName = temporaryPassives[i];
            for (int j = 0; j < passiveSkillNames.Count; j++)
            {
                if (passiveSkillNames[j] == skillName)
                {
                    passiveSkillNames.RemoveAt(j);
                }
            }
        }
        for (int i = 0; i < temporaryActives.Count; i++)
        {
            if (temporaryActives.Count <= 0){continue;}
            skillName = temporaryActives[i];
            for (int j = 0; j < activeSkillNames.Count; j++)
            {
                if (activeSkillNames[j] == skillName)
                {
                    activeSkillNames.RemoveAt(j);
                }
            }
        }
        temporaryActives.Clear();
        temporaryPassives.Clear();
    }
    public string npcMoveSkill = "none";
    public string npcAttackSkill = "none";
    public string npcSupportSkill = "none";
    public TacticActiveSkill activeSkill;
    public TacticBuffsStatuses buffDebuff;
    public TacticPassiveSkill passiveSkill;

    // Since the text is loading before start is called.
    public void QuickStart(TacticActor quickStartActor = null)
    {
        health = baseHealth;
        weight = size;
        currentMovespeed = baseMovement;
        energy = baseEnergy;
        attackDamage = baseAttack;
        defense = baseDefense;
        currentAttackRange = attackRange;
        initiative = baseInitiative;
        if (quickStartActor == null){return;}
        health = Mathf.Min(health, quickStartActor.health);
        energy = Mathf.Min(energy, quickStartActor.energy);
    }

    public void CopyStats(TacticActor actorToCopy)
    {
        level = actorToCopy.level;
        team = actorToCopy.team;
        locationIndex = actorToCopy.locationIndex;
        typeName = actorToCopy.typeName;
        baseHealth = actorToCopy.baseHealth;
        health = Mathf.Min(baseHealth, actorToCopy.ReturnHealth());
        baseMovement = actorToCopy.baseMovement;
        baseAttack = actorToCopy.baseAttack;
        baseDefense = actorToCopy.baseDefense;
        baseEnergy = actorToCopy.baseEnergy;
        energy = Mathf.Min(baseEnergy, actorToCopy.ReturnEnergy());
        baseActions = actorToCopy.baseActions;
        attackRange = actorToCopy.attackRange;
        movementType = actorToCopy.moveType;
        movementCosts = actorToCopy.movementCosts;
        activeSkillNames = actorToCopy.activeSkillNames;
        passiveSkillNames = actorToCopy.passiveSkillNames;
        npcMoveSkill = actorToCopy.npcMoveSkill;
        npcAttackSkill = actorToCopy.npcAttackSkill;
        npcSupportSkill = actorToCopy.npcSupportSkill;
        size = actorToCopy.size;
        species = actorToCopy.species;
        baseInitiative = actorToCopy.baseInitiative;
        terrainMap = actorToCopy.terrainMap;
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
            delayable = false;
            return;
        }
        delayable = true;
        weight = size;
        attackDamage = baseAttack;
        actionsLeft = baseActions;
        defense = baseDefense;
        currentMovespeed = baseMovement;
        currentAttackRange = attackRange;
        //energy = Mathf.Min(energy+1, baseEnergy);
        counterAttacksLeft = Mathf.Min(counterAttacksLeft+1, 1);
        attackAdvantage = 0;
        // After getting a full round of access to temp skills/passives remove them.
        ResetTempSkills();
        // These might give you temporary active/passive skills.
        ApplyBuffDebuffEffects();
        // Then go through any start of turn passives?
        TurnPassives(0);
        movement = 0;
    }

    public void TurnPassives(int timing = 0)
    {
        if (passiveSkillNames.Count <= 0){return;}
        // Go through all the passives.
        for (int i = passiveSkillNames.Count - 1; i > -1; i--)
        {
            if (passiveSkillNames[i].Length < 1)
            {
                passiveSkillNames.RemoveAt(i);
                continue;
            }
            terrainMap.actorManager.LoadPassiveData(passiveSkill, passiveSkillNames[i]);
            // Try to apply the start of turn ones.
            if (passiveSkill.timing != timing){continue;}
            if (passiveSkill.StartTurnConditions(this)){passiveSkill.AffectActor(this);}
        }
    }

    public void EndTurn()
    {
        TurnPassives(1);
    }

    public void Death()
    {
        Color tempColor = Color.white;
        tempColor.a = 0f;
        //spriteRenderer.sprite = null;
        spriteRenderer.color = tempColor;
        actionsLeft = 0;
        terrainMap.moraleTracker.TeamMemberDies(team, baseHealth);
        //Destroy(gameObject);
    }

    public void ChangeAI()
    {
        if (AIType == 1)
        {
            AIType = 0;
            terrainMap.AlertEnemyTeam();
        }
        if (species == "Beast")
        {
            if (health < baseHealth/2 && AIType != 2)
            {
                AIType = 2;
                // Buff damage.
                baseAttack += baseAttack/5;
            }
            if (health > baseHealth/2 && AIType == 2)
            {
                AIType = 0;
                // Stop rage mode.
                baseAttack -= baseAttack/6;
            }
        }
    }

    public void AlertedByAlly()
    {
        AIType = 0;
    }

    public void RegainHealth(int amount)
    {
        if (health > baseHealth){return;}
        health += amount;
        terrainMap.actionLog.AddActionLog(typeName+" regains "+amount+" HP.");
        ChangeAI();
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

    public void ApplyBuffDebuffEffects()
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

    public void NPCLoadSkill(int type)
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
        if (!delayable){return false;}
        if (movement <= 0 && actionsLeft <= 0){return false;}
        if (health <= 0){return false;}
        return true;
    }

    public bool Actable()
    {
        // Only the dead and things that can never act.
        if (health <= 0 || baseActions <= 0){return false;}
        return true;
    }

    public bool CheckActions(int actionCost = 1)
    {
        if (actionsLeft < actionCost){return false;}
        return true;
    }

    public bool CheckSkillActivatable()
    {
        if (activeSkill.skillName.Length < 3){return false;}
        // No such thing as a skill that costs zero energy.
        if (actionsLeft < activeSkill.ReturnActionCost(this) || energy < activeSkill.cost || activeSkill.cost <= 0)
        {
            return false;
        }
        // Need to be facing a direction to use cone skills.
        if (activeSkill.targetingShape == "Cone" && currentDirection < 0){return false;}
        // Can't gain a skill/passive you already have.
        if (activeSkill.effect == "TempPassive" && passiveSkillNames.Contains(activeSkill.effectSpecifics))
        {
            return false;
        }
        else if (activeSkill.effect == "TempActive" && activeSkillNames.Contains(activeSkill.effectSpecifics))
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

    public void AttackAction()
    {
        if (actionsLeft < actionsToAttack || health <= 0){return;}
        NPCLoadSkill(1);
        // Try to hit the first target.
        if (terrainMap.pathFinder.CalculateDistance(locationIndex, attackTarget.locationIndex) <= currentAttackRange && attackTarget.health > 0)
        {
            if (attackTarget == null){return;}
            if (CheckSkillActivatable())
            {
                terrainMap.NPCActivateSkill(attackTarget);
                ActivateSkill();
                CheckIfAttackAgain();
                return;
            }
            terrainMap.NPCActorAttack(attackTarget);
            actionsLeft -= actionsToAttack;
        }
        // Otherwise look for another target.
        else
        {
            attackTarget = terrainMap.ReturnEnemyInRange(this);
            if (attackTarget == null){return;}
            if (CheckSkillActivatable())
            {
                terrainMap.NPCActivateSkill(attackTarget);
                ActivateSkill();
                CheckIfAttackAgain();
                return;
            }
            terrainMap.NPCActorAttack(attackTarget);
            actionsLeft -= actionsToAttack;
        }
        // Keep attacking until you're out of actions.
        CheckIfAttackAgain();
    }

    public void CheckIfAttackAgain()
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

    public void SupportAction()
    {
        if (actionsLeft <= 0 || health <= 0){return;}
        NPCLoadSkill(2);
        if (CheckSkillActivatable())
        {
            // Support skills are always cast on oneself?
            // We can make different AI's later.
            terrainMap.NPCActivateSkill(this);
            ActivateSkill();
        }
    }

    public void UpdateTarget(TacticActor newTarget)
    {
        attackTarget = newTarget;
    }

    public void SetMap(TerrainMap newMap)
    {
        terrainMap = newMap;
    }

    public void GetPath()
    {
        currentPath = terrainMap.pathFinder.FindPathIndex(this, destinationIndex);
        // Cornered animals will fight back.
        if (currentPath.Count <= 0 && AIType == 2)currentPath = terrainMap.pathFinder.FindPathIndex(this, attackTarget.locationIndex);
        turnPath.Clear();
    }

    // Can skip some steps in simulated battles.
    public void NPCStartTurn(bool real = true)
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
        MoveAlongPath(false);
        // Check dps skill.
        AttackAction();
        SupportAction();
        MoveAlongPath(real);
    }

    public void CheckGoal()
    {
        // Randomly move around if you don't have a target.
        // Look for a target in range.
        UpdateTarget(terrainMap.FindClosestEnemyInAttackRange());
        // Otherwise go for the closest enemy.
        if (attackTarget == null)
        {
            UpdateTarget(terrainMap.FindNearestEnemy());
        }
        switch (AIType)
        {
            case 0:
                // Aggresive means you run towards the enemy.
                UpdateDest(attackTarget.locationIndex);
                break;
            case 1:
                // Passive means you don't move.
                UpdateDest(locationIndex);
                break;
            case 2:
                // Defensive means run away from the closest enemy.
                UpdateDest(terrainMap.FindFurthestTileFromTarget(this, attackTarget));
                break;
        }
        //UpdateDest(attackTarget.locationIndex);
        // Attack your target if you have one.
        // If you're injured then start looking for targets? Depends on the type of AI.
    }

    public void CheckEnemyInRange()
    {

    }

    public void UpdateDest(int newDest)
    {
        destinationIndex = newDest;
    }

    public void MoveAction(bool real = true)
    {
        if (Moveable())
        {
            MoveAction();
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

    public void MoveAlongPath(bool real = true)
    {
        if (turnPath.Count > 0)
        {
            if (!real)
            {
                if (turnPath.Count == 1)
                {
                    ChangeDirection(terrainMap.pathFinder.DirectionBetweenLocations(locationIndex, turnPath[0]));
                }
                else
                {
                    ChangeDirection(terrainMap.pathFinder.DirectionBetweenLocations(turnPath[turnPath.Count - 2], turnPath[^1]));
                }
                locationIndex = turnPath[^1];
                terrainMap.TriggerMovingEffects(this, locationIndex);
                return;
            }
            StartCoroutine(ShowMovementPath());
        }
    }

    IEnumerator ShowMovementPath()
    {
        //terrainMap.interactable = false;
        for (int i = 0; i < turnPath.Count; i++)
        {
            locationIndex = turnPath[i];
            terrainMap.UpdateOnActorTurn();
            yield return new WaitForSeconds(.1f);
        }
        //terrainMap.interactable = true;
    }

    public bool Moveable()
    {
        // Don't move if you can't.
        if (currentPath.Count <= 0 || currentPath[0] == locationIndex || health <= 0 || actionsLeft <= 0)
        {
            return false;
        }
        // Don't move if you can attack your target.
        if (terrainMap.CheckTargetInRange(this, attackTarget))
        {
            return false;
        }
        if (currentPath.Contains(locationIndex) && turnPath.Count <= 0)
        {
            // Move to the next step on the path.
            int cIndex = currentPath.IndexOf(locationIndex);
            if (CheckDistance(currentPath[cIndex-1]))
            {
                turnPath.Add(currentPath[cIndex-1]);
                //locationIndex = currentPath[cIndex-1];
                return true;
            }
        }
        else if (turnPath.Count > 0)
        {
            // Move to the next step on the path.
            int cIndex = currentPath.IndexOf(turnPath[^1]);
            // Don't move if you already reached the end.
            if (cIndex <= 0)
            {
                return false;
            }
            if (CheckDistance(currentPath[cIndex-1]))
            {
                turnPath.Add(currentPath[cIndex-1]);
                return true;
            }
        }
        else
        {
            // Start from the end of the path.
            if (CheckDistance(currentPath[^1]))
            {
                turnPath.Add(currentPath[^1]);
                //locationIndex = currentPath[^1];
                return true;
            }
        }
        return false;
    }

    public bool CheckDistance(int index)
    {
        // Might need direction calcs here too.
        int distance = ReturnMoveCostForTile(terrainMap.pathFinder.terrainInfo[index]);
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
            return movement + (currentMovespeed * (actionsLeft - actionsToAttack));
        }
        return currentMovespeed * (baseActions - actionsToAttack);
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