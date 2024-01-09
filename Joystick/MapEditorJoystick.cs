using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorJoystick : BasicJoystick
{
    public Map map;

    // Update is called once per frame
    protected override void Update()
    {
        if (Time.time < lastMove + moveCooldown){return;}
        int joy_x = (int) joystick.Horizontal;
        int joy_y = (int) joystick.Vertical;
        if (joy_x == 0 && joy_y == 0){return;}
        MoveHorizontally(joy_x);
        MoveVertically(joy_y);
        lastMove = Time.time;
    }

    protected override void MoveHorizontally(int x)
    {
        if (x > 0)
        {
            map.MoveMap(1);
        }
        else if (x < 0)
        {
            map.MoveMap(4);
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
            map.MoveMap(3);
        }
    }
}
