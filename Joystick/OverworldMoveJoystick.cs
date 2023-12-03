using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldMoveJoystick : MonoBehaviour
{
    public OverworldMap map;
    public Joystick joystick;
    public float moveCooldown = 0.2f;
    private float lastMove = 0f;


    void Update()
    {
        if (Time.time < lastMove + moveCooldown){return;}
        int joy_x = (int) joystick.Horizontal;
        int joy_y = (int) joystick.Vertical;
        if (joy_x == 0 && joy_y == 0){return;}
        MoveMapHorizontally(joy_x);
        MoveMapVertically(joy_y);
        lastMove = Time.time;
    }

    private void MoveMapHorizontally(int x)
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

    private void MoveMapVertically(int y)
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
