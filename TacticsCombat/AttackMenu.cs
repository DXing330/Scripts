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

    public void UpdateTarget(TacticActor newTarget)
    {
        target = newTarget;
        if (target == null)
        {
            ResetTargetInfo();
            return;
        }
        UpdateTargetInfo();
    }

    private void ResetTargetInfo()
    {
        Color tempColor = Color.white;
        tempColor.a = 0f;
        targetSprite.color = tempColor;
        targetSprite.sprite = null;
        targetHealth.text = "";
    }

    private void UpdateTargetInfo()
    {
        if (target.health <= 0)
        {
            ResetTargetInfo();
            return;
        }
        Color tempColor = Color.white;
        tempColor.a = 0.6f;
        targetSprite.color = tempColor;
        targetSprite.sprite = target.image.sprite;
        targetHealth.text = target.health.ToString();
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
}
