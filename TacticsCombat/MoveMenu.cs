using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoveMenu : MonoBehaviour
{
    public TerrainMap terrainMap;
    public TMP_Text movement;
    public Text moveUpCost;
    public Text moveRightCost;
    public Text moveDownCost;
    public Text moveLeftCost;

    public void UpdateMovementText()
    {
        movement.text = "Move Speed: "+terrainMap.ActorCurrentMovement();
    }
}
