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
    public GameObject viewButton;
    public GameObject viewMenu;
    public int state = 0;
    public TacticActor currentActor;

    public void ChangeState(int newState)
    {
        if (!terrainMap.battleStarted)
        {
            return;
        }
        currentActor = terrainMap.ReturnCurrentTurnActor();
        switch (newState)
        {
            case 0:
                break;
            case 1:
                if (currentActor.movement <= 0){return;}
                break;
            case 2:
                if (currentActor.actionsLeft <= 0){return;}
                break;
            case 3:
                if (currentActor.actionsLeft <= 0){return;}
                if (currentActor.activeSkillNames.Count <= 0){return;}
                break;
        }
        state = newState;
        UpdateState();
        AdjustButtons();
    }

    private void AdjustButtons()
    {
        if (state == 0)
        {
            viewButton.SetActive(true);
            attackButton.SetActive(true);
            moveButton.SetActive(true);
            skillButton.SetActive(true);
            returnButton.SetActive(false);
        }
        else
        {
            viewButton.SetActive(false);
            attackButton.SetActive(false);
            moveButton.SetActive(false);
            skillButton.SetActive(false);
            returnButton.SetActive(true);
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
                DisableViewing();
                break;
            case 1:
                terrainMap.ActorStartMoving();
                EnableMovement();
                DisableAttack();
                DisableSkills();
                DisableViewing();
                break;
            case 2:
                terrainMap.ActorStartAttacking();
                attackMenu.UpdateTarget(terrainMap.ReturnCurrentTarget());
                DisableMovement();
                EnableAttack();
                DisableSkills();
                DisableViewing();
                break;
            case 3:
                terrainMap.ActorStartUsingSkills();
                skillMenu.UpdateSkill(terrainMap.ReturnCurrentSkill());
                DisableMovement();
                DisableAttack();
                EnableSkills();
                DisableViewing();
                break;
            case 4:
                terrainMap.StartViewingActorInfo();
                DisableMovement();
                DisableAttack();
                DisableSkills();
                EnableViewing();
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

    private void EnableViewing()
    {
        viewMenu.SetActive(true);
    }

    private void DisableViewing()
    {
        viewMenu.SetActive(false);
    }
}
