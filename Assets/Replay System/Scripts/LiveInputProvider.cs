using UnityEngine;
using UnityEngine.InputSystem;

public class LiveInputProvider : MonoBehaviour, IInputProvider
{
    public static PlayerInput PlayerInput;
    public bool IsFinished => false;

    private InputAction _moveAction;
    private InputAction _jumpAction;

    private Vector2 _move;

    // to refence later, GetInput() will run in FixedUpdate(), so need to capture something from Update() and only result when polled, otherwise input can get lost
    private bool _jumpPressedLatch; 
    private bool _jumpReleasedLatch;

    public InputFrame GetInput(int tick)
    {
        var frame = new InputFrame
        {
            Tick = tick,

            Move = _move,
            JumpWasPressed = _jumpPressedLatch,
            JumpIsHeld = _jumpAction.IsPressed(),
            JumpWasReleased = _jumpReleasedLatch
        };

        _jumpPressedLatch = false;
        _jumpReleasedLatch = false;

        return frame;
    }

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();

        _moveAction = PlayerInput.actions["Move"];
        _jumpAction = PlayerInput.actions["Jump"];
    }

    public void Poll()
    {
        _move = _moveAction.ReadValue<Vector2>();
        if (_jumpAction.WasPressedThisFrame()) _jumpPressedLatch = true;
        if (_jumpAction.WasReleasedThisFrame()) _jumpReleasedLatch = true;
    }

    void Update()
    {
        Poll();
    }
}
