using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldMoveJoystick : BasicJoystick
{
    public OverworldMap map;

    protected override void MoveHorizontally(int x)
    {
        if (x > 0)
        {
            map.MovePlayer(1);
        }
        else if (x < 0)
        {
            map.MovePlayer(3);
        }
    }

    protected override void MoveVertically(int y)
    {
        if (y > 0)
        {
            map.MovePlayer(0);
        }
        else if (y < 0)
        {
            map.MovePlayer(2);
        }
    }
}
