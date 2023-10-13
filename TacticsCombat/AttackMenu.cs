using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackMenu : MonoBehaviour
{
    public Image targetSprite;
    public Text targetName;
    public Text targetHealth;
    public TacticActor target;
    public TerrainMap terrainMap;

    public void UpdateTarget(TacticActor newTarget, bool name = false)
    {
        target = newTarget;
        if (target == null)
        {
            ResetTargetInfo();
            return;
        }
        UpdateTargetInfo(name);
    }

    private void ResetTargetInfo()
    {
        Color tempColor = Color.white;
        tempColor.a = 0f;
        targetSprite.color = tempColor;
        targetSprite.sprite = null;
        targetHealth.text = "";
    }

    private void UpdateTargetInfo(bool name = false)
    {
        if (target.health <= 0)
        {
            ResetTargetInfo();
            return;
        }
        Color tempColor = Color.white;
        tempColor.a = 0.6f;
        targetSprite.color = tempColor;
        targetSprite.sprite = target.spriteRenderer.sprite;
        targetHealth.text = target.health.ToString();
        if (name)
        {
            //targetHealth.text = target.typeName;
            targetHealth.text = "";
        }
    }

    public void AttackTarget()
    {
        if (target == null)
        {
            return;
        }
        terrainMap.CurrentActorAttack();
        UpdateTarget(target);
    }

    public void SwitchTarget(bool right = true)
    {
        terrainMap.SwitchTarget(right);
        UpdateTarget(terrainMap.ReturnCurrentTarget());
    }

    public void SwitchTargetForViewing(bool right = true)
    {
        terrainMap.SwitchViewedActor(right);
        UpdateTarget(terrainMap.ReturnCurrentAttackTarget(), true);
    }

    public void StartViewing()
    {
        if (!terrainMap.battleStarted)
        {
            return;
        }
        terrainMap.StartViewingActorInfo();
        UpdateTarget(terrainMap.ReturnCurrentAttackTarget(), true);
    }
}
