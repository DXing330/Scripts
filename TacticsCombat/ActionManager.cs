using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ActionManager : MonoBehaviour
{
    public TMP_Text actionsLeft;
    public TerrainMap terrainMap;
    public List<GameObject> actionMenus;
    protected void DisableMenus(int menuIndex = -1)
    {
        for (int i = 0; i < actionMenus.Count; i++)
        {
            actionMenus[i].SetActive(false);
        }
        if (menuIndex > 0){EnableMenu(menuIndex - 1);}
    }
    protected void EnableMenu(int menuIndex)
    {
        if (menuIndex < 0 || menuIndex >= actionMenus.Count){return;}
        actionMenus[menuIndex].SetActive(true);
    }
    public MoveMenu moveMenu;
    public AttackMenu attackMenu;
    public SkillMenu skillMenu;
    public GameObject returnButton;
    public GameObject moveButton;
    public GameObject attackButton;
    public GameObject skillButton;
    public GameObject viewButton;
    public int state = 0;
    public TacticActor currentActor;

    public void UpdateActionsLeft()
    {
        actionsLeft.text = "";
        currentActor = terrainMap.ReturnCurrentTurnActor();
        if (currentActor == null || currentActor.actionsLeft <= 0)
        {
            return;
        }
        for (int i = 0; i < currentActor.actionsLeft; i++)
        {
            actionsLeft.text += " o ";
        }
    }

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
                UpdateActionsLeft();
                break;
            case 1:
                if (currentActor.movement <= 0 && currentActor.actionsLeft <= 0){return;}
                break;
            case 2:
                if (currentActor.actionsLeft <= 0){return;}
                break;
            case 3:
                if (currentActor.actionsLeft <= 0 || currentActor.energy <= 0){return;}
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
        DisableMenus(state);
        switch (state)
        {
            case 0:
                terrainMap.ViewCurrentActor();
                break;
            case 1:
                terrainMap.ActorStartMoving();
                break;
            case 2:
                terrainMap.ActorStartAttacking();
                attackMenu.UpdateTarget(terrainMap.ReturnCurrentTarget());
                break;
            case 3:
                terrainMap.ActorStartUsingSkills();
                skillMenu.skillList.SetActor(currentActor);
                // Use the current actor to get a list of skill names.
                skillMenu.UpdateSkill(terrainMap.ReturnCurrentSkill());
                break;
            case 4:
                terrainMap.StartViewingActorInfo();
                break;
        }
    }

    public void CheckIfAttackAgain()
    {
        UpdateActionsLeft();
        if (currentActor == null)
        {
            ChangeState(0);
            return;
        }
        if (currentActor.CheckActionsToAttack())
        {
            return;
        }
        else
        {
            ChangeState(0);
        }
    }
}
