using UnityEngine;

public interface IInputProvider 
{
    public InputFrame GetInput(int tick);
    public bool IsFinished {get; }
}

public struct InputFrame
{
    public int Tick;
    public Vector2 Move;
    public bool Jumping;
    public bool Sliding;
    public bool Posing;
    public bool Attacking;
}