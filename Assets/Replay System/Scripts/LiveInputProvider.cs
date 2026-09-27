using UnityEngine;
using UnityEngine.InputSystem;

public class LiveInputProvider : MonoBehaviour, IInputProvider
{
    public static PlayerInput PlayerInput;
    public bool IsFinished => false;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _slideAction;
    private InputAction _poseAction;
    private InputAction _attackAction;

    private Vector2 _move;

    // to refence later, GetInput() will run in FixedUpdate(), so need to capture something from Update() and only result when polled, otherwise input can get lost
    private bool _jumpPressedLatch; 
    private bool _jumpReleasedLatch;
    private bool _slidePressedLatch; 
    private bool _slideReleasedLatch;
    private bool _posePressedLatch; 
    private bool _attackPressedLatch;

    public InputFrame GetInput(int tick)
    {
        var frame = new InputFrame
        {
            Tick = tick,

            Move = _move,
            JumpWasPressed = _jumpPressedLatch,
            JumpIsHeld = _jumpAction.IsPressed(),
            JumpWasReleased = _jumpReleasedLatch,

            SlideWasPressed = _slidePressedLatch,
            SlideIsHeld = _slideAction.IsPressed(),
            SlideWasReleased = _slideReleasedLatch,

            AttackWasPressed = _attackPressedLatch,

            PoseWasPressed = _posePressedLatch
        };

        _jumpPressedLatch = false;
        _jumpReleasedLatch = false;
        _jumpPressedLatch = false;
        _jumpReleasedLatch = false;
        _slidePressedLatch = false;
        _slideReleasedLatch = false;
        _posePressedLatch = false;
        _attackPressedLatch = false;

        return frame;
    }

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();

        _moveAction = PlayerInput.actions["Move"];
        _jumpAction = PlayerInput.actions["Jump"];
        _slideAction = PlayerInput.actions["Slide"];
        _poseAction = PlayerInput.actions["Pose"];
        _attackAction = PlayerInput.actions["Attack"];
    }

    public void Poll()
    {
        _move = _moveAction.ReadValue<Vector2>();
        if (_jumpAction.WasPressedThisFrame()) _jumpPressedLatch = true;
        if (_jumpAction.WasReleasedThisFrame()) _jumpReleasedLatch = true;
        if (_slideAction.WasPressedThisFrame()) _slidePressedLatch = true;
        if (_slideAction.WasReleasedThisFrame()) _slideReleasedLatch = true;
        if (_poseAction.WasPressedThisFrame()) _posePressedLatch = true;
        if (_attackAction.WasPressedThisFrame()) _attackPressedLatch = true;
    }

    public void ResetLatches()
    {
        _jumpPressedLatch = false;
        _jumpReleasedLatch = false;
        _slidePressedLatch = false;
        _slideReleasedLatch = false;
        _posePressedLatch = false;
        _attackPressedLatch = false;
    }

    void Update()
    {
        Poll();
    }
}
