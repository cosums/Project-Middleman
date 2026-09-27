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

    public bool JumpWasPressed;
    public bool JumpIsHeld;
    public bool JumpWasReleased;

    public bool SlideWasPressed;
    public bool SlideIsHeld;
    public bool SlideWasReleased;

    public bool PoseWasPressed;
    public bool AttackWasPressed;
}