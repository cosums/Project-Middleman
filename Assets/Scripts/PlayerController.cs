using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Walk")]
    public float maxWalkSpeed = 20f;
    public float groundAcceleration = 5f;
    public float groundDeceleration = 20f;
    public float airAcceleration = 5f;
    public float airDeceleration = 5f;

    [Header("Jump")]
    public float jumpHeight = 6.5f;
    public float jumpHeightCompensationFactor = 1.054f;
    public float timeTilJumpApex = 0.35f;
    public float gravityOnReleaseMultiplier = 2f;
    public float maxFallSpeed = 26f;
    public int maxJumps = 1;

    [Header("Jump Cut")]
    public float timeForUpwardsCancel = 0.027f;

    [Header("Jump Apex")]
    public float apexThreshold = 0.97f;
    public float apexHangTime = 0.075f;

    [Header("Jump Buffer")]
    public float jumpBufferTime = 0.125f;

    [Header("Jump Coyote Time")]
    public float jumpCoyoteTime = 0.1f;

    [Header("Ground/Collision Checks")]
    public LayerMask groundLayer;
    public float groundDetectionRayLength = 0.02f;
    public float headDetectionRayLength = 0.02f;
    public float headWidth = 0.75f;

    [Header("References")]
    [SerializeField] private Collider2D _feetColl;
    [SerializeField] private Collider2D _bodyColl;

    private Rigidbody2D rb;

    //movement variables
    private Vector2 _moveVelocity;
    private bool isFacingRight;

    //collision check vars
    private RaycastHit2D _groundHit;
    private RaycastHit2D _headHit;
    private bool _isGrounded;
    private bool _headBumped;

    //jump vars
    public float verticalVelocity { get; private set; }
    private bool _isJumping;
    private bool _isFastfalling;
    private bool _isFalling;
    private float _fastFallTime;
    private float _fastFallReleaseSpeed;
    private int _numberOfJumpsUsed;

    //apex vars
    private float _apexPoint;
    private float _timePastApexThreshold;
    private bool _isPastApexThreshold;

    //jump buffer vars
    private float _jumpBufferTimer;
    private bool _jumpReleasedDuringBuffer;

    //coyotoe time vars
    private float _coyoteTimer;

    public float Gravity { get; private set; }
    public float initialJumpVelocity { get; private set; }
    public float adjustedJumpHeight { get; private set; }

    private void Awake()
    {
        isFacingRight = true;
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        CountTimers();
        JumpChecks();
    }

    private void FixedUpdate()
    {
        CollisionCheck();
        Jump();

        if (_isGrounded)
        {
            Move(groundAcceleration, groundDeceleration, InputManager.movement);
        }
        else
        {
            Move(airAcceleration, airDeceleration, InputManager.movement);
        }
    }

    #region Movement

    private void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
        Vector2 targetVelocity = new Vector2(moveInput.x, 0f) * maxWalkSpeed;
        if (moveInput != Vector2.zero)
        {
            TurnCheck(moveInput);
            _moveVelocity = Vector2.Lerp(_moveVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(_moveVelocity.x, rb.linearVelocity.y);
        }
        else
        {
            _moveVelocity = Vector2.Lerp(_moveVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(_moveVelocity.x, rb.linearVelocityY);
        }
    }

    private void TurnCheck(Vector2 moveInput)
    {
        if (isFacingRight && moveInput.x < 0)
        {
            Turn(false);
        }
        else if (!isFacingRight && moveInput.x > 0)
        {
            Turn(true);
        }
    }

    private void Turn(bool turnRight)
    {
        if (turnRight)
        {
            isFacingRight = true;
            transform.Rotate(0f, 180f, 0f);
        }
        else
        {
            isFacingRight = false;
            transform.Rotate(0f, -180f, 0f);
        }
    }

    #endregion
    #region Collision Checks
    private void BumpedHead()
    {
        Vector2 boxCastOrigin = new Vector2(_feetColl.bounds.center.x, _bodyColl.bounds.max.y);
        Vector2 boxCastSize = new Vector2(_feetColl.bounds.size.x * headWidth, headDetectionRayLength);

        _headHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.up, headDetectionRayLength, groundLayer);
        if (_headHit.collider != null)
        {
            _headBumped = true;
        }
        else
        {
            _headBumped = false;
        }

    }
    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(_feetColl.bounds.center.x, _feetColl.bounds.min.y);
        Vector2 boxCastSize = new Vector2(_feetColl.bounds.size.x, groundDetectionRayLength);

        _groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, groundDetectionRayLength, groundLayer);
        if (_groundHit.collider != null)
        {
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }
    }

    private void CollisionCheck()
    {
        IsGrounded();
        BumpedHead();
    }

    #endregion
    #region Jump
    private void InitiateJump(int numberOfJumpsUsed)
    {
        if (!_isJumping)
        {
            _isJumping = true;
        }
        _jumpBufferTimer = 0f;
        _numberOfJumpsUsed += numberOfJumpsUsed;
        verticalVelocity = initialJumpVelocity;
    }

    private void Jump()
    {
        //Apply gravity while jumping
        if (_isJumping)
        {
            //Check for head bump
            if (_headBumped)
            {
                _isFastfalling = true;
            }

            //Gravity on ascending
            if (verticalVelocity >= 0f)
            {
                //Apex controls
                _apexPoint = Mathf.InverseLerp(initialJumpVelocity, 0f, verticalVelocity);

                if (_apexPoint > apexThreshold)
                {
                    if (!_isPastApexThreshold) {
                        _isPastApexThreshold = true;
                        _timePastApexThreshold = 0f;
                        print("apex");
                    }

                    if (_isPastApexThreshold)
                    {
                        _timePastApexThreshold += Time.fixedDeltaTime;
                        if (_timePastApexThreshold < apexHangTime)
                        {
                            verticalVelocity = 0f;
                        }
                        else
                        {
                            verticalVelocity = -0.01f;
                            _isJumping = false;
                        }
                    }
                }
            
                //Gravity on ascending but not past apex threshold
                else
                {
                    verticalVelocity += Gravity * Time.fixedDeltaTime;
                    if (_isPastApexThreshold)
                    {
                        _isPastApexThreshold = false;
                    }
                }
            }
        }

        //Gravity on descending
        else if (!_isFastfalling)
        {
            verticalVelocity += Gravity * gravityOnReleaseMultiplier * Time.fixedDeltaTime;
        }

        else if (verticalVelocity < 0f)
        {
            if (!_isFalling)
            {
                _isFalling = true;

            }
        }

        //Jump cut
        if (_isFastfalling)
        {
            if (_fastFallTime >= timeForUpwardsCancel)
            {
                verticalVelocity += Gravity * gravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if (_fastFallTime < timeForUpwardsCancel)
            {
                verticalVelocity = Mathf.Lerp(_fastFallReleaseSpeed, 0f, (_fastFallTime / timeForUpwardsCancel));
            }

            _fastFallTime += Time.fixedDeltaTime;
        }

        //Normal gravity while falling
        if (!_isGrounded && !_isJumping)
        {
            if (!_isFalling)
            {
                _isFalling = true;
            }
            verticalVelocity += Gravity * Time.fixedDeltaTime;
        }

        //Clamp fall speed
        verticalVelocity = Mathf.Clamp(verticalVelocity, -maxFallSpeed, 50f);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, verticalVelocity);
    }

    private void JumpChecks()
    {
        //Jump button pressed
        if (InputManager.JumpWasPressed)
        {
            _jumpBufferTimer = jumpBufferTime;
            _jumpReleasedDuringBuffer = false;
        }

        //Jump button released
        if (InputManager.JumpWasReleased)
        {
            if (_jumpBufferTimer > 0f)
            {
                _jumpReleasedDuringBuffer = true;
            }

            if (_isJumping && verticalVelocity > 0f)
            {
                if (_isPastApexThreshold)
                {
                    _isPastApexThreshold = true;
                    _isFastfalling = true;
                    _fastFallTime = timeForUpwardsCancel;
                    verticalVelocity = 0f;
                }
                else
                {
                    _isFastfalling = true;
                    _fastFallReleaseSpeed = verticalVelocity;
                }
            }
        }

        //Initiate jump with jump buffering and coyote time
        if (_jumpBufferTimer > 0f && !_isJumping && (_isGrounded || _coyoteTimer > 0f))
        {
            InitiateJump(1);
            
            if (_jumpReleasedDuringBuffer)
            {
                _isFastfalling = true;
                _fastFallReleaseSpeed = verticalVelocity;
            }
        }

        //Double jump
        else if (_jumpBufferTimer > 0f && _isJumping && _numberOfJumpsUsed < maxJumps)
        {
            _isFastfalling = true;
            InitiateJump(1);
        }

        //Air jump after coyote time lapses
        else if (_jumpBufferTimer > 0f && _isFalling && _numberOfJumpsUsed < maxJumps - 1)
        {
            InitiateJump(2);
            _isFastfalling = false;
        }

        //Landed
        if ((_isJumping || _isFalling) && _isGrounded && verticalVelocity <= 0f)
        {
            _isJumping = false;
            _isFalling = false;
            _isFastfalling = false;
            _fastFallTime = 0f;
            _isPastApexThreshold = false;
            _numberOfJumpsUsed = 0;

            verticalVelocity = Physics2D.gravity.y;
        }

    }

    private void OnValidate()
    {
        CalculateValues();
    }

    private void OnEnable()
    {
        CalculateValues();
    }

    private void CalculateValues()
    {
        adjustedJumpHeight = jumpHeight * jumpHeightCompensationFactor;
        Gravity = -(2f * adjustedJumpHeight) / Mathf.Pow(timeTilJumpApex, 2f);
        initialJumpVelocity = Mathf.Abs(Gravity) * timeTilJumpApex;
    }

    #endregion
    #region Timers

    private void CountTimers()
    {
        _jumpBufferTimer -= Time.deltaTime;

        if (!_isGrounded)
        {
            _coyoteTimer -= Time.deltaTime;
        }
        else
        {
            _coyoteTimer = jumpCoyoteTime;
        }
    }

    #endregion
}
