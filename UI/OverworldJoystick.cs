using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldJoystick : BasicJoystick
{
    public Map map;

    protected override void Update()
    {
        if (Time.time < lastMove + moveCooldown){return;}
        int joy_x = (int) joystick.Horizontal;
        int joy_y = (int) joystick.Vertical;
        if (joy_x == 0 && joy_y == 0){return;}
        MoveMap(joy_x, joy_y);
        lastMove = Time.time;
    }

    protected void MoveMap(int x, int y)
    {
        if (y == 0){return;}
        if (y > 0)
        {
            if (x == 0)
            {
                map.MoveMap(0);
            }
            else if (x > 0)
            {
                map.MoveMap(1);
            }
            else if (x < 0)
            {
                map.MoveMap(5);
            }
        }
        else
        {
            if (x == 0)
            {
                map.MoveMap(3);
            }
            else if (x > 0)
            {
                map.MoveMap(2);
            }
            else if (x < 0)
            {
                map.MoveMap(4);
            }
        }
    }
}
