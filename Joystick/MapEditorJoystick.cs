using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorJoystick : BasicJoystick
{
    public MapEditor map;

    protected override void MoveHorizontally(int x)
    {
        if (x > 0)
        {
            map.MoveMap(1);
        }
        else if (x < 0)
        {
            map.MoveMap(3);
        }
    }

    protected override void MoveVertically(int y)
    {
        if (y > 0)
        {
            map.MoveMap(0);
        }
        else if (y < 0)
        {
            map.MoveMap(2);
        }
    }
}
