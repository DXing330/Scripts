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
    public int armyMaxCapacity = 0;
    public int currentCapacity;

    void Start()
    {
        armyData = GameManager.instance.armyData;
        armyMaxCapacity = 2+GameManager.instance.playerLevel;
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
        currentReserveSelected = -1;
        UpdateReserveFighters();
    }

    private void UpdateReserveFighters()
    {
        int index = currentReservePage * reserveFighters.Count;
        for (int i = 0; i < reserveFighters.Count; i++)
        {
            reserveFighters[i].ResetActorSprite();
            reserveFighters[i].ResetHighlight();
            if (index >= armyData.availableFighters.Count)
            {
                break;
            }
            if (i == currentReserveSelected)
            {
                reserveFighters[i].Highlight();
            }
            reserveFighters[i].UpdateActorSprite(SpriteDictionary(armyData.availableFighters[index]));
            index++;
        }
    }

    private void UpdateFormationTiles()
    {
        for (int i = 0; i < formationTiles.Count; i++)
        {
            formationTiles[i].ResetHighlight();
            formationTiles[i].UpdateActorSprite(SpriteDictionary(armyData.armyFormation[i]));
            if (i == currentlySelected)
            {
                formationTiles[i].Highlight();
            }
        }
    }

    public void SelectReserveSpot(int index)
    {
        currentReserveSelected = index+(currentReservePage * reserveFighters.Count);
        if (currentReserveSelected >= armyData.availableFighters.Count)
        {
            currentReserveSelected = -1;
        }
        if (selected)
        {
            selected = false;
            // Try to switch the army and the reserve members.
            // Never remove the player or familiar.
            string tempString = armyData.armyFormation[currentlySelected];
            if (tempString == "Player" || tempString == "Familiar")
            {
                currentlySelected = -1;
                UpdateFormationTiles();
                return;
            }
            CountCapacity();
            // Don't add anymore units if you're at max capacity.
            if (currentReserveSelected < 0 || (currentCapacity >= armyMaxCapacity && tempString == "none"))
            {
                currentlySelected = -1;
                UpdateFormationTiles();
                return;
            }
            // Otherwise do the switcheroo.
            armyData.armyFormation[currentlySelected] = armyData.availableFighters[currentReserveSelected];
            armyData.availableFighters[currentReserveSelected] = tempString;
            currentlySelected = -1;
            currentReserveSelected = -1;
            UpdateFormationTiles();
            UpdateReserveFighters();
            return;
        }
        if (!reserveSelected)
        {
            reserveSelected = true;
        }
        UpdateFormationTiles();
        UpdateReserveFighters();
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
                currentReserveSelected = -1;
                UpdateReserveFighters();
                return;
            }
            CountCapacity();
            // Don't add anymore units if you're at max capacity.
            if (currentCapacity >= armyMaxCapacity && tempString == "none")
            {
                currentReserveSelected = -1;
                UpdateReserveFighters();
                return;
            }
            armyData.armyFormation[index] = armyData.availableFighters[currentReserveSelected];
            armyData.availableFighters.RemoveAt(currentReserveSelected);
            if (tempString != "none")
            {
                armyData.availableFighters.Add(tempString);
            }
            currentReserveSelected = -1;
            currentlySelected = -1;
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
        UpdateFormationTiles();
        UpdateReserveFighters();
    }

    private void CountCapacity()
    {
        currentCapacity = 0;
        for (int i = 0; i < armyData.armyFormation.Count; i++)
        {
            if (armyData.armyFormation[i] == "none")
            {
                continue;
            }
            currentCapacity++;
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
            currentlySelected = -1;
            UpdateFormationTiles();
            UpdateReserveFighters();
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
