using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmyFormationManager : MonoBehaviour
{
    public List<FormationTile> formationTiles;

    public void ReturnToHub()
    {
        GameManager.instance.ReturnToHub();
    }
}
