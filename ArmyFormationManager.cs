using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmyFormationManager : MonoBehaviour
{
    public List<Sprite> actorSprites;
    public List<FormationTile> formationTiles;
    public List<FormationTile> reserveFighters;
    private bool selected = false;
    private int currentlySelected = -1;
    private bool reserveSelected = false;
    private int currentReserveSelected = -1;
    public ArmyDataManager armyData;

    void Start()
    {
        armyData = GameManager.instance.armyData;
        UpdateFormationTiles();
    }

    private void UpdateReserveFighters()
    {

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

    }

    public void SelectSpot(int index)
    {
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
        if (actorName != "none")
        {
            armyData.availableFighters.Add(actorName);
            armyData.armyFormation[currentlySelected] = "none";
            UpdateFormationTiles();

        }
    }

    public void ReturnToHub()
    {
        GameManager.instance.ReturnToHub();
    }

    private Sprite SpriteDictionary(string actorName)
    {
        if (actorName == "Player")
        {
            return actorSprites[0];
        }
        if (actorName == "Familiar")
        {
            return actorSprites[1];
        }
        for (int i = 0; i < actorSprites.Count; i++)
        {
            if (actorSprites[i].name == actorName)
            {
                return actorSprites[i];
            }
        }
        return null;
    }
}
