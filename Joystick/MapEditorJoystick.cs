using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorJoystick : BasicJoystick
{
    public MapEditor map;

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

    protected void MoveMap(float x, float y)
    {

    }
}
