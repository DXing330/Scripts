using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicJoystick : MonoBehaviour
{
    public Joystick joystick;
    public float moveCooldown = 0.2f;
    protected float lastMove = 0f;


    // Update is called once per frame
    protected virtual void Update()
    {
        if (Time.time < lastMove + moveCooldown){return;}
        int joy_x = (int) joystick.Horizontal;
        int joy_y = (int) joystick.Vertical;
        if (joy_x == 0 && joy_y == 0){return;}
        MoveHorizontally(joy_x);
        MoveVertically(joy_y);
        lastMove = Time.time;
    }

    protected virtual void MoveHorizontally(int x)
    {
        if (x > 0)
        {
        }
        else if (x < 0)
        {
        }
    }

    protected virtual void MoveVertically(int y)
    {
        if (y > 0)
        {
        }
        else if (y < 0)
        {
        }
    }
}
