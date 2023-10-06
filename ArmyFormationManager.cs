using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmyFormationManager : MonoBehaviour
{
    public ActorSprites actorSprites;
    public List<FormationTile> formationTiles;
    public List<FormationTile> reserveFighters;
    private bool selected = false;
    public int currentlySelected = -1;
    private bool reserveSelected = false;
    public int currentReserveSelected = -1;
    private int currentReservePage = 0;
    public ArmyDataManager armyData;

    void Start()
    {
        armyData = GameManager.instance.armyData;
        UpdateFormationTiles();
        UpdateReserveFighters();
    }

    public void SwitchPage(bool right = true)
    {
        if (right)
        {
            if ((currentReservePage+1)*reserveFighters.Count < armyData.availableFighters.Count)
            {
                currentReservePage++;
            }
            else
            {
                currentReservePage = 0;
            }
        }
        else
        {
            if (currentReservePage > 0)
            {
                currentReservePage--;
            }
            else
            {
                currentReservePage = armyData.availableFighters.Count/reserveFighters.Count;
            }
        }
        UpdateReserveFighters();
    }

    private void UpdateReserveFighters()
    {
        int index = currentReservePage * reserveFighters.Count;
        for (int i = 0; i < reserveFighters.Count; i++)
        {
            reserveFighters[i].ResetActorSprite();
            if (index >= armyData.availableFighters.Count)
            {
                break;
            }
            reserveFighters[i].UpdateActorSprite(SpriteDictionary(armyData.availableFighters[index]));
            index++;
        }
    }

    private void UpdateFormationTiles()
    {
        for (int i = 0; i < formationTiles.Count; i++)
        {
            formationTiles[i].UpdateActorSprite(SpriteDictionary(armyData.armyFormation[i]));
        }
    }

    public void SelectReserveSpot(int index)
    {
        if (!reserveSelected)
        {
            reserveSelected = true;
        }
        currentReserveSelected = index+(currentReservePage * reserveFighters.Count);
        if (currentReserveSelected >= armyData.availableFighters.Count)
        {
            currentReserveSelected = -1;
        }
    }

    public void SelectSpot(int index)
    {
        if (reserveSelected)
        {
            reserveSelected = false;
            if (currentReserveSelected < 0)
            {
                return;
            }
            string tempString = armyData.armyFormation[index];
            if (tempString == "Player" || tempString == "Familiar")
            {
                return;
            }
            armyData.armyFormation[index] = armyData.availableFighters[currentReserveSelected];
            armyData.availableFighters.RemoveAt(currentReserveSelected);
            if (tempString != "none")
            {
                armyData.availableFighters.Add(tempString);
            }
            UpdateFormationTiles();
            UpdateReserveFighters();
            return;
        }
        if (!selected)
        {
            selected = true;
            currentlySelected = index;
        }
        else
        {
            selected = false;
            string tempString = armyData.armyFormation[currentlySelected];
            armyData.armyFormation[currentlySelected] = armyData.armyFormation[index];
            armyData.armyFormation[index] = tempString;
            UpdateFormationTiles();
            currentlySelected = -1;
        }
    }

    public void RemoveFromSpot()
    {
        if (currentlySelected < 0)
        {
            return;
        }
        string actorName = armyData.armyFormation[currentlySelected];
        if (actorName == "Player" || actorName == "Familiar")
        {
            return;
        }
        if (actorName != "none" && actorName.Length >= 3)
        {
            armyData.availableFighters.Add(actorName);
            armyData.armyFormation[currentlySelected] = "none";
            UpdateFormationTiles();
            UpdateReserveFighters();
            currentlySelected = -1;
            selected = false;
        }
    }

    public void ReturnToHub()
    {
        GameManager.instance.ReturnToHub();
    }

    private Sprite SpriteDictionary(string actorName)
    {
        return actorSprites.SpriteDictionary(actorName);
    }
}
