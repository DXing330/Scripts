using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorJoystick : BasicJoystick
{
    public MapEditor map;

    protected override void Update()
    {
        if (Time.time < lastMove + moveCooldown){return;}
        float joy_x = joystick.Horizontal;
        float joy_y = joystick.Vertical;
        if (joy_x == 0 && joy_y == 0){return;}
        MoveMap(joy_x, joy_y);
        lastMove = Time.time;
    }

    protected void MoveMap(float x, float y)
    {
        if (y == 0){return;}
        if (y > 0)
        {
            if (Mathf.Abs(x) < Mathf.Abs(y))
            {
                map.MoveMap(0);
            }
            else
            {
                if (x > 0)
                {
                    map.MoveMap(1);
                }
                else
                {
                    map.MoveMap(5);
                }
            }
        }
        else
        {
            if (Mathf.Abs(x) < Mathf.Abs(y))
            {
                map.MoveMap(3);
            }
            else
            {
                if (x > 0)
                {
                    map.MoveMap(2);
                }
                else
                {
                    map.MoveMap(4);
                }
            }
        }
    }
}
