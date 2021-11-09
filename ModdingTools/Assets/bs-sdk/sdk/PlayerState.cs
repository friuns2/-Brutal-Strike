using System;
using UnityEngine;

[Serializable]
public class PlayerState
{
    //[HideInInspector]
    public string Name;
    //[HideInInspector]
    public PlayerStateEnum playerState;
    public float speed = 1;
    public float controllerHeight = 1;
    public float skinOffset = 0;
    public AnimationCurve speedToAnim = AnimationCurve.Linear(0, 0, 1, 1); 
    public static implicit operator PlayerStateEnum(PlayerState t)
    {

        if (t == null) return PlayerStateEnum.Walk;
        return t.playerState;
    }
}

public enum PlayerStateEnum
{
    Walk = 0,
    Prone = 1,
    Crouch = 2,
    Run = 3,
    InAir = 4,
    Knocked = 5,
    Executing = 6,
    Healing=7,Dead=8
}