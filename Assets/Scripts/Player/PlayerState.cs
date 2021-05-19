using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState{

    public State state { get; set; } = State.DEFAULT;
    public enum State{
        DEFAULT,
        MOVE_ON_WALL,
        CLIMB_TO_WALL,
        WALL_JUMP,
        DASH,
        ATTACK
    }
}
