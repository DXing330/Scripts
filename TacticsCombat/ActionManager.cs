using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public TerrainMap terrainMap;
    public GameObject moveArrows;
    public MoveMenu moveMenu;
    public GameObject battleMenu;
    public AttackMenu attackMenu;
    public GameObject skillSelect;
    public SkillMenu skillMenu;
    public GameObject returnButton;
    public GameObject moveButton;
    public GameObject attackButton;
    public GameObject skillButton;
    public int state = 0;

    public void ChangeState(int newState)
    {
        state = newState;
        UpdateState();
        AdjustButtons();
    }

    private void AdjustButtons()
    {
        switch (state)
        {
            case 0:
                attackButton.SetActive(true);
                moveButton.SetActive(true);
                skillButton.SetActive(true);
                returnButton.SetActive(false);
                break;
            case 1:
                attackButton.SetActive(false);
                moveButton.SetActive(false);
                skillButton.SetActive(false);
                returnButton.SetActive(true);
                break;
            case 2:
                attackButton.SetActive(false);
                moveButton.SetActive(false);
                skillButton.SetActive(false);
                returnButton.SetActive(true);
                break;
            case 3:
                attackButton.SetActive(false);
                moveButton.SetActive(false);
                skillButton.SetActive(false);
                returnButton.SetActive(true);
                break;
        }
    }

    private void UpdateState()
    {
        switch (state)
        {
            case 0:
                terrainMap.ActorStopMoving();
                DisableMovement();
                DisableAttack();
                DisableSkills();
                break;
            case 1:
                terrainMap.ActorStartMoving();
                EnableMovement();
                DisableAttack();
                DisableSkills();
                break;
            case 2:
                terrainMap.ActorStartAttacking();
                attackMenu.UpdateTarget(terrainMap.ReturnCurrentTarget());
                DisableMovement();
                EnableAttack();
                DisableSkills();
                break;
            case 3:
                terrainMap.ActorStartUsingSkills();
                skillMenu.UpdateSkill(terrainMap.ReturnCurrentSkill());
                DisableMovement();
                DisableAttack();
                EnableSkills();
                break;
        }
    }

    private void EnableMovement()
    {
        moveArrows.SetActive(true);
        moveMenu.UpdateText();
    }

    private void DisableMovement()
    {
        moveArrows.SetActive(false);
    }

    private void EnableAttack()
    {
        battleMenu.SetActive(true);
    }

    private void DisableAttack()
    {
        battleMenu.SetActive(false);
    }

    private void EnableSkills()
    {
        skillSelect.SetActive(true);
    }

    private void DisableSkills()
    {
        skillSelect.SetActive(false);
    }
}
