using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBetweenActors : MonoBehaviour
{
    public bool ActorsBattle(TacticActor attacker, TacticActor defender, bool counter = true, bool flanked = false, int skillPowerMultipler = 0, int defenderBonus = 6, int attackerDefenseBonus = 6, int distance = 1)
    {
        // Encourage attacking.
        int attackAdvantage = 1;
        int attackPower = attacker.attackDamage;
        // Check for flanking/ally support.
        if (flanked){attackAdvantage++;}
        attackPower = attackPower*6/defenderBonus;
        int attackDamage = attacker.GenerateAttackDamage(attackAdvantage, attackPower);
        attackDamage = attacker.ApplyAttackingPassives(attackDamage, defender);
        // Skill damage happens at the very end, making it the most powerful.
        if (skillPowerMultipler > 10)
        {
            attackDamage += attackDamage*skillPowerMultipler/10 - attackDamage;
        }
        // Trigger on attack, on hit passives here.
        // TODO
        defender.ReceiveAttackPassives(attacker, distance);
        defender.ReceiveDamage(attackDamage, attacker.currentDirection);
        if (counter)
        {
            int defenderPower = defender.attackDamage*6/attackerDefenseBonus;
            // Penalty for ranged defenders.
            if (attacker.currentAttackRange < defender.currentAttackRange){defenderPower/=2;}
            int attackerHealth = attacker.health;
            int attackerDefense = attacker.defense;
            int defenderDamageDealt = defender.GenerateAttackDamage(0, defenderPower);
            attacker.ReceiveDamage(defenderDamageDealt, defender.currentDirection);
            if (defenderDamageDealt - attackerDefense >= attackerHealth || attackerHealth <= 1){return true;}
            else if (attackerHealth <= 1 && defenderDamageDealt > attackerDefense/2){return true;}
        }
        return false;
    }
}
