using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveMenu : MonoBehaviour
{
    public TerrainMap terrainMap;
    public Text movement;
    public Text moveUpCost;
    public Text moveRightCost;
    public Text moveDownCost;
    public Text moveLeftCost;

    public void UpdateText()
    {
        movement.text = "Movement: "+terrainMap.ActorCurrentMovement();
    }
}
