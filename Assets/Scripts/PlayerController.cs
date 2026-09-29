using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(PlayerVisualsController))]
public class PlayerController : MonoBehaviour
{
    [Header("Visuals")]
    PlayerVisualsController playerVisualsController;

    [Header("Walk")]
    public float moveThreshold = 0.25f;
    public float maxWalkSpeed = 20f;
    public float groundAcceleration = 5f;
    public float groundDeceleration = 20f;
    public float airAcceleration = 5f;
    public float airDeceleration = 5f;
    public float wallJumpMoveAcceleration = 5f;
    public float wallJumpMoveDeceleration = 5f;

    [Header("Jump")]
    public float jumpHeight = 6.5f;
    public float jumpHeightCompensationFactor = 1.054f;
    public float timeTilJumpApex = 0.35f;
    public float gravityOnReleaseMultiplier = 2f;
    public float maxFallSpeed = 26f;
    public int maxJumps = 1;

    [Header("Reset Jump Options")]
    public bool resetJumpsOnWallSlide = true;

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
    public LayerMask headBumbpLayer;
    public float groundDetectionRayLength = 0.02f;
    public float headDetectionRayLength = 0.02f;
    public float headWidth = 0.75f;
    public float wallDetectionRayLength = 0.125f;
    public float wallDetectionRayHeightMultiplier = 0.9f;

    [Header("Wall Slide")]
    public float wallSlideSpeed = 5f;
    public float wallSlideDecelerationSpeed = 50f;

    [Header("Wall Jump")]
    public Vector2 wallJumpDirection = new Vector2(-20f, 6.5f);
    public float wallJumpPostBufferTime = 0.125f;
    public float wallJumpGravityOnReleaseMultiplier = 1f;

    [Header("References")]
    [SerializeField] private Collider2D _feetColl;
    [SerializeField] private Collider2D _bodyColl;
    [SerializeField] private Collider2D _attackColl;
    [SerializeField] private Collider2D _slideAttackColl;
    [SerializeField] private Collider2D _jumpThruScanColl;
    public GameObject groundPoint;

    [Header("Slide")]
    public float slideSpeedMultiplier = 1.25f;
    public float slideBufferTime = 0.125f;
    public float slideAcceleration = 3.5f;
    public float slideDeceleration = 0.5f;
    public float slideMaxSpeedTime = 0.2f;
    public float minSlideSpeedToScore = 10f;
    public float minSlideSpeedToHit = 12.5f;

    [Header("Pose")]
    public float poseTime = 1f;
    public float poseCooldown = 3f;
    public GameObject poseParticles;

    [Header("Attack")]
    public float attackTime = 0.2f;
    public float attackCoolDown = 2f;
    public GameObject attackParticles;

    [Header("Score Multipliers")]
    public float insideFrameScore = 0.1f;
    public float airTimeScore = 0.05f;
    public float slideTimeScore = 0.025f;

    [Header("Flat Score Points")]
    public int flipScore;
    public int poseScore;
    public int punchScore;

    public static Action OnPlayerAttack;

    private Rigidbody2D rb;

    //movement variables
    private float horizontalVelocity;
    private bool isFacingRight;
    private float _currentMaxWalkSpeed;

    //collision check vars
    private RaycastHit2D _groundHit;
    private RaycastHit2D _headHit;
    private RaycastHit2D _wallHit;
    private RaycastHit2D _lastWallHit;
    private bool _isGrounded;
    private bool _headBumped;
    private bool _isTouchingWall;
    private Rigidbody2D _currentGorund;
    private int _numOveralppingPlatforms = 0;

    //jump vars
    public float verticalVelocity { get; private set; }
    private bool _isJumping;
    private bool _isFastfalling;
    private bool _isFalling;
    private float _fastFallTime;
    private float _fastFallReleaseSpeed;
    private int _numberOfJumpsUsed;
    private float _airtime;

    //apex vars
    private float _apexPoint;
    private float _timePastApexThreshold;
    private bool _isPastApexThreshold;

    //jump buffer vars
    private float _jumpBufferTimer;
    private bool _jumpReleasedDuringBuffer;

    //coyotoe time vars
    private float _coyoteTimer;

    //jump
    public float Gravity { get; private set; }
    public float initialJumpVelocity { get; private set; }
    public float adjustedJumpHeight { get; private set; }

    // input reference
    private InputFrame _currentInputFrame;

    //wall jump
    public float WallJumpGravity { get; private set; }
    public float initialWallJumpVelocity { get; private set; }
    public float adjustedWallJumpHeight { get; private set; }

    //wall slide
    private bool _isWallSliding;
    private bool _isWallSlideFalling;

    //other wall jump vars (idk why theyre here)
    private bool _useWallJumpMoveStats;
    private bool _isWallJumping;
    private float _wallJumpTime;
    private float _wallJumpFastFallTime;
    private float _wallJumpFastFallReleaseSpeed;
    private bool _isWallJumpFastFalling;
    private bool _isWallJumpFalling;

    private float _wallJumpPostBufferTimer;

    private float _wallJumpApexPoint;
    private float _timePastWallJumpApexThreshold;
    private bool _isPastWallJumpApexThreshold;

    //slide
    private bool _isSliding = false;
    private float _slideSpeed;
    private float _targetSlideSpeed;
    private float _slideBufferTimer;
    private bool _pastSlideMaxSpeed = false;
    private float _slideMaxSpeedTimer;
    private float _slideTime;
    private float _absoluteMaxSlideSpeed;

    //pose
    private float _poseTimer;
    private float _poseCooldownTimer;
    private bool _isPosing;

    // attack
    private float _attackTimer;
    private float _attackCooldownTimer;
    private bool _isAttacking;


    // references for replay
    private Vector3 _startingPos;

    private void Awake()
    {
        isFacingRight = true;
        rb = GetComponent<Rigidbody2D>();
        playerVisualsController = GetComponent<PlayerVisualsController>();
        _currentMaxWalkSpeed = maxWalkSpeed;
        _absoluteMaxSlideSpeed = maxWalkSpeed * slideSpeedMultiplier;

        CaptureStart();
    }
    private void Update()
    {
        CountTimers();
        //JumpChecks();
        //LandCheck();
        //WallSlideCheck();
        //WallJumpCheck();
        //SlideCheck();
    }

    public void CaptureStart()
    {
        _startingPos = transform.position;
    }

    public void RestoreStart()
    {
        transform.position = _startingPos;
    }

    //private void FixedUpdate()
    //{
    //    CollisionCheck();
    //    Jump();
    //    LandCheck();
    //    WallSlide();

    //     if (_isGrounded)
    //     {
    //         Move(groundAcceleration, groundDeceleration, InputManager.movement);
    //     }
    //     else
    //     {
    //         Move(airAcceleration, airDeceleration, InputManager.movement);
    //     }

    //    ApplyVelocity();
    //}

    // effectively FixedUpdate, so put any changes here
    public void SimulateTick(InputFrame frame)
    {
        _currentInputFrame = frame;
        playerVisualsController.SimulateTick(frame);

        CollisionCheck();
        LandCheck();
        WallSlideCheck();
        WallJumpCheck();
        SlideCheck();
        JumpChecks();
        PoseCheck();
        AttackCheck();
        Jump();
        Fall();
        LandCheck();
        WallSlide();
        WallJump();
        Slide();
        Pose();
        Attack();

        if (AirCheck())
        {
            PointSystem.Instance.AddToMultiplier(airTimeScore, "Air Time!");
        }

        if (_isSliding)
        {
            Slide();
        }
        else
        {
            if (_isGrounded)
            {
                Move(groundAcceleration, groundDeceleration, frame.Move);
            }
            else
            {
                Move(airAcceleration, airDeceleration, frame.Move);
            }
        }

        ApplyVelocity();
    }

    public void ApplyVelocity()
    {
        //clamp fall speed
        verticalVelocity = Mathf.Clamp(verticalVelocity, -maxFallSpeed, 50f);

        if (_isGrounded && _currentGorund != null)
        {
            horizontalVelocity = Mathf.Clamp(horizontalVelocity, -_absoluteMaxSlideSpeed, _absoluteMaxSlideSpeed);
            rb.linearVelocity = new Vector2(horizontalVelocity, verticalVelocity) + _currentGorund.linearVelocity;
        }
        else
        {
            rb.linearVelocity = new Vector2(horizontalVelocity, verticalVelocity);
        }
  
    }

    public bool GetInAir()
    {
        return !_isGrounded;
    }

    public int GetRemainingJumps()
    {
        return maxJumps - _numberOfJumpsUsed;
    }

    public bool GetIsSliding()
    {
        return _isSliding;
    }

    public float GetPlayerSpeed()
    {
        return Mathf.Abs(rb.linearVelocityX);
    }

    #region Movement

    private void Move(float acceleration, float deceleration, Vector2 moveInput)
    {
        float targetVelocity = moveInput.x * maxWalkSpeed;
        if (Mathf.Abs(moveInput.x) >= moveThreshold)
        {
            TurnCheck(moveInput);
            horizontalVelocity = Mathf.Lerp(horizontalVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }
        else if (Mathf.Abs(moveInput.x) < moveThreshold)
        {
            horizontalVelocity = Mathf.Lerp(horizontalVelocity, 0f, deceleration * Time.fixedDeltaTime);
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
    #region Land/Fall

    private void LandCheck()
    {
        //Landed
        if ((_isJumping || _isFalling || _isWallJumping || _isWallSliding || _isWallSlideFalling || _isWallJumpFalling || _isSliding) && _isGrounded && verticalVelocity <= 0f)
        {
            ResetJumpValues();
            StopWallSlide();
            ResetWallJumpValues();

            _numberOfJumpsUsed = 0;

            verticalVelocity = Physics2D.gravity.y;
        }
    }
    private void Fall()
    {
        //Normal gravity while falling
        if (!_isGrounded && !_isJumping && !_isWallJumping && !_isWallSliding)
        {
            if (!_isFalling)
            {
                _isFalling = true;
            }
            verticalVelocity += Gravity * Time.fixedDeltaTime;
        }
    }

    private bool AirCheck()
    {
        if ((_isFalling || _isJumping || _isWallJumping || _isWallJumpFalling || _isFastfalling || _isWallJumpFastFalling) && !_isGrounded)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion
    #region Collision Checks
    private void BumpedHead()
    {
        Vector2 boxCastOrigin = new Vector2(_feetColl.bounds.center.x, _bodyColl.bounds.max.y);
        Vector2 boxCastSize = new Vector2(_feetColl.bounds.size.x * headWidth, headDetectionRayLength);

        _headHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.up, headDetectionRayLength, headBumbpLayer);
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

        // Ignore a ground hit against a jump-thru platform we're currently overlapping/passing through
        if (_groundHit.collider != null &&
            _groundHit.collider.gameObject.layer == LayerMask.NameToLayer("Jumpthru Platform Trigger") &&
            _numOveralppingPlatforms > 0)
        {
            _groundHit = default;
            return;
        }

        if (_groundHit.collider != null)
        {
            _isGrounded = true;
            _currentGorund = _groundHit.collider.gameObject.GetComponent<Rigidbody2D>();
        }
        else
        {
            _isGrounded = false;
            _currentGorund = null;
        }
    }

    private void IsTouchingWall()
    {
        float originEndPoint = 0f;
        if (isFacingRight)
        {
            originEndPoint = _bodyColl.bounds.max.x;
        }
        else
        {
            originEndPoint = _bodyColl.bounds.min.x;
        }
        
        float adjustedHeight = _bodyColl.bounds.size.y * wallDetectionRayHeightMultiplier;

        Vector2 boxCastOrigin = new Vector2(originEndPoint, _bodyColl.bounds.center.y);
        Vector2 boxCastSize = new Vector2(wallDetectionRayLength, adjustedHeight);

        _wallHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, transform.right, wallDetectionRayLength, groundLayer);
        if (_wallHit.collider != null && _numOveralppingPlatforms == 0)
        {
            _lastWallHit = _wallHit;
            _isTouchingWall = true;
        }
        else
        {
            _isTouchingWall = false;
        }
    }

    private void CollisionCheck()
    {
        LayerMask playerLayer = LayerMask.NameToLayer("Player");
        LayerMask platformLayer = LayerMask.NameToLayer("Jumpthru Platform");

        if (verticalVelocity > 0)
        {
            Physics2D.IgnoreLayerCollision(playerLayer, platformLayer, true);
        }
        else if (_numOveralppingPlatforms == 0)
        {
            Physics2D.IgnoreLayerCollision(playerLayer, platformLayer, false);
        }

        IsGrounded();
        BumpedHead();
        IsTouchingWall();

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Jumpthru Platform Trigger"))
        {
            _numOveralppingPlatforms++;
            print(_numOveralppingPlatforms);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Jumpthru Platform Trigger"))
        {
            _numOveralppingPlatforms--;
            print(_numOveralppingPlatforms);
        }
    }

    public Collider2D GetFeetCollider()
    {
        return _feetColl;
    }

    #endregion
    #region Jump

    private void ResetJumpValues()
    {
        _isJumping = false;
        _isFalling = false;
        _isFastfalling = false;
        _fastFallTime = 0f;
        _isPastApexThreshold = false;
    }
    private void InitiateJump(int numberOfJumpsUsed)
    {
        if (!_isJumping)
        {
            _isJumping = true;
        }

        ResetWallJumpValues();

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
                else if (!_isFastfalling)
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

        

        //Clamp fall speed
        verticalVelocity = Mathf.Clamp(verticalVelocity, -maxFallSpeed, 50f);
    }

    private void JumpChecks()
    {
        if (_isWallJumping && _wallJumpTime <= 0f)
        {
            return; // wall jump already handled this press
        }

        //Jump button pressed
        if (_currentInputFrame.JumpWasPressed)
        {
            Debug.Log("Jump pressed");
            if (_isWallSlideFalling && _wallJumpPostBufferTimer >= 0f)
            {
                return;
            }

            else if (_isWallSliding || (_isTouchingWall && !_isGrounded))
            {
                return;
            }

                _jumpBufferTimer = jumpBufferTime;
            _jumpReleasedDuringBuffer = false;
        }

        //Jump button released
        if (_currentInputFrame.JumpWasReleased)
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
        else if (_jumpBufferTimer > 0f && !_isGrounded && !_isTouchingWall && _numberOfJumpsUsed < maxJumps)
        {
            InitiateJump(1);
            PointSystem.Instance.AddToComboScore(flipScore, "Flip!");
        }

        //Air jump after coyote time lapses
        //else if (_jumpBufferTimer > 0f && !_isWallSlideFalling && _isFalling && _numberOfJumpsUsed < maxJumps - 1)
        //{
        //    InitiateJump(2);
        //    _isFastfalling = false;
        //}     

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

        adjustedWallJumpHeight = wallJumpDirection.y * jumpHeightCompensationFactor;
        WallJumpGravity = -(2f * adjustedWallJumpHeight) / Mathf.Pow(timeTilJumpApex, 2f);
        initialWallJumpVelocity = Mathf.Abs(WallJumpGravity) * timeTilJumpApex;
    }

    #endregion
    #region Wall Slide

    private void WallSlideCheck()
    {
        if (_isTouchingWall && !_isGrounded && _numOveralppingPlatforms == 0)
        {
            if (verticalVelocity < 0 && !_isWallSliding)
            {
                ResetJumpValues();
                ResetWallJumpValues();
                
                _isWallSlideFalling = false;
                _isWallSliding = true;

                if (resetJumpsOnWallSlide)
                {
                    _numberOfJumpsUsed = 0;
                }
            }
        }

        else if (_isWallSliding && !_isTouchingWall && !_isGrounded && !_isWallSlideFalling)
        {
            _isWallSlideFalling = true;
            StopWallSlide();
        }
        else
        {
            StopWallSlide();
        }
    }

    private void StopWallSlide()
    {
        if (_isWallSliding)
        {
            _isWallSliding = false;
        }
    }

    private void WallSlide()
    {
        if (_isWallSliding)
        {
            verticalVelocity = Mathf.Lerp(verticalVelocity, -wallSlideSpeed, wallSlideDecelerationSpeed * Time.fixedDeltaTime);
        }
    }

    #endregion
    #region Wall Jump
    private void WallJumpCheck()
    {
        if (ShouldApplyWallJumpBuffer())
        {
            _wallJumpPostBufferTimer = wallJumpPostBufferTime;
        }

        //wall jump fast falling
        if (_currentInputFrame.JumpWasReleased && !_isWallSliding && !_isTouchingWall && _isWallJumping)
        {
            if (verticalVelocity > 0f)
            {
                if (_isPastWallJumpApexThreshold)
                {
                    _isPastWallJumpApexThreshold = false;
                    _isWallJumpFastFalling = true;
                    _wallJumpFastFallTime = timeForUpwardsCancel;

                    verticalVelocity = 0f;
                }
                else
                {
                    _isWallJumpFastFalling = true;
                    _wallJumpFastFallReleaseSpeed = verticalVelocity;
                }
            }
        }

        //actual jump with post wall jump buffer time
        if (_currentInputFrame.JumpWasPressed && _wallJumpPostBufferTimer > 0f)
        {
            InitiateWallJump();
            _wallJumpPostBufferTimer = 0f;
        }
    }

    private void InitiateWallJump()
    {
        if (!_isWallJumping)
        {
            _isWallJumping = true;
            _useWallJumpMoveStats = true;
        }

        StopWallSlide();
        ResetJumpValues();
        _wallJumpTime = 0f;

        verticalVelocity = initialWallJumpVelocity;

        int dirMultiplier = 0;
        Vector2 hitPoint = _lastWallHit.collider.ClosestPoint(_bodyColl.bounds.center);

        if (hitPoint.x > transform.position.x)
        {
            dirMultiplier = -1;
        }
        else
        {
            dirMultiplier = 1;
        }

        horizontalVelocity = Mathf.Abs(wallJumpDirection.x) * dirMultiplier;
    }

    private void WallJump()
    {
        //apply wall jump gravity
        if (_isWallJumping)
        {
            _wallJumpTime += Time.fixedDeltaTime;
            if (_wallJumpTime >= timeTilJumpApex)
            {
                _useWallJumpMoveStats = false;
            }

            //hit head
            if (_headBumped)
            {
                _isWallJumpFastFalling = true;
                _useWallJumpMoveStats = false;
            }

            if (_isPastWallJumpApexThreshold)
            {
                _timePastWallJumpApexThreshold += Time.fixedDeltaTime;
                if (_timePastWallJumpApexThreshold < apexHangTime)
                {
                    verticalVelocity = 0f;
                }
                else
                {
                    verticalVelocity = -0.01f;
                    _isWallJumping = false;
                }
            }

            //gravity  while ascending
            if (verticalVelocity >= 0f)
            {
                //apex controls
                _wallJumpApexPoint = Mathf.InverseLerp(initialWallJumpVelocity, 0f, verticalVelocity);

                if (_wallJumpApexPoint > apexThreshold)
                {
                    if (!_isPastWallJumpApexThreshold)
                    {
                        _isPastWallJumpApexThreshold = true;
                        _timePastWallJumpApexThreshold = 0f;
                    }

                    
                }

                //gravity while ascending but not past apex threshold
                else if (!_isWallJumpFastFalling)
                {
                    verticalVelocity += WallJumpGravity * Time.fixedDeltaTime;

                    if (_isPastWallJumpApexThreshold)
                    {
                        _isPastWallJumpApexThreshold = false;
                    }
                }
            }

            //gravity on descending
            else if (!_isWallJumpFastFalling)
            {
                verticalVelocity += WallJumpGravity * Time.fixedDeltaTime;
            }

            else if (verticalVelocity < 0f)
            {
                if (!_isWallJumpFalling)
                {
                    _isWallJumpFalling = true;
                }
            }
        }

        //handle wall jump cut time
        if (_isWallJumpFastFalling)
        {
            if (_wallJumpFastFallTime >= timeForUpwardsCancel)
            {
                verticalVelocity += WallJumpGravity * wallJumpGravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if (_wallJumpFastFallTime < timeForUpwardsCancel)
            {
                verticalVelocity = Mathf.Lerp(_wallJumpFastFallReleaseSpeed, 0f, (_wallJumpFastFallTime / timeForUpwardsCancel));
            }

            _wallJumpFastFallTime += Time.fixedDeltaTime;
        }
    }

    private bool ShouldApplyWallJumpBuffer()
    {
        return !_isGrounded && _numOveralppingPlatforms == 0 && (_isTouchingWall || _isWallSliding);
    }

    private void ResetWallJumpValues()
    {
        _isWallSlideFalling = false;
        _useWallJumpMoveStats = false;
        _isWallJumping = false;
        _isWallJumpFastFalling = false;
        _isWallJumpFalling = false;
        _isPastWallJumpApexThreshold = false;

        _wallJumpFastFallTime = 0f;
        _wallJumpTime = 0f;
    }

    #endregion
    #region Slide

    private void SlideCheck()
    {
        if (_currentInputFrame.SlideWasPressed && _isGrounded && !_isSliding)
        {
            InitiateSlide();
        }
        else if (_currentInputFrame.SlideWasPressed)
        {
            _slideBufferTimer = slideBufferTime;
        }
        else if (_currentInputFrame.SlideIsHeld && _slideBufferTimer > 0f && _isGrounded && !_isSliding)
        {
            InitiateSlide();
        }
        else if (!_currentInputFrame.SlideIsHeld)
        {
            _isSliding = false;
            ResetSlideValues();
        }
    }
    
    private void InitiateSlide()
    {
        _isSliding = true;
        _slideSpeed = horizontalVelocity;
        _targetSlideSpeed = _slideSpeed * slideSpeedMultiplier;
        _slideMaxSpeedTimer = slideMaxSpeedTime;
        _slideTime = 0;
    }

    private void Slide()
    {
        if (_isSliding)
        {
            if (Mathf.Abs(horizontalVelocity) > minSlideSpeedToScore)
            {
                PointSystem.Instance.AddToMultiplier(slideTimeScore * Time.fixedDeltaTime, "Sliding!");
            }
            
            if (!_pastSlideMaxSpeed && _isGrounded)
            {
                _slideSpeed = Mathf.Lerp(_slideSpeed, _targetSlideSpeed, slideAcceleration * Time.fixedDeltaTime);
                if (Mathf.Abs(horizontalVelocity - _targetSlideSpeed) < 0.1f)
                {
                    _pastSlideMaxSpeed = true;
                }
            }
            else if (_slideMaxSpeedTimer > 0f && _isGrounded)
            {
                _slideMaxSpeedTimer -= Time.fixedDeltaTime;
             
            }
            else if (_isGrounded && !_isJumping && _slideMaxSpeedTimer <= 0f)
            {
                _slideSpeed = Mathf.Lerp(_slideSpeed, 0f, slideDeceleration * Time.fixedDeltaTime);
            }

            horizontalVelocity = _slideSpeed;
            _slideTime += Time.fixedDeltaTime;
        } 
    }

    private void ResetSlideValues()
    {
        _isSliding = false;
        _pastSlideMaxSpeed = false;
        _slideBufferTimer = -1f;
        _slideMaxSpeedTimer = slideMaxSpeedTime;
        _currentMaxWalkSpeed = maxWalkSpeed;
    }

    #endregion
    #region Pose
    public void PoseCheck()
    {
        if (_currentInputFrame.PoseWasPressed && !_isSliding && !_isTouchingWall)
        {
            _isPosing = true;
            GameObject particles = GameObject.Instantiate(poseParticles);
            particles.transform.position = this.transform.position;
            particles.transform.SetParent(this.transform);
            PointSystem.Instance.AddToComboScore(poseScore, "Strike a Pose!");
        }
        else if (!_currentInputFrame.PoseWasPressed)
        {
            _isPosing = false;
        }
    }

    public void Pose()
    {
        if (_isPosing)
        {
            // TODO posing
            Debug.Log("Strike a pose!");
        }
    }
    #endregion

    #region Attack
    public void AttackCheck()
    {
        if ((_currentInputFrame.AttackWasPressed && !_isSliding) || (_isSliding && Mathf.Abs(horizontalVelocity) >= minSlideSpeedToHit))
        {
            _isAttacking = true;
        }
        else if (!_currentInputFrame.AttackWasPressed)
        {
            _isAttacking = false;
        }
    }

    public void Attack()
    {
        if (_isAttacking)
        {
            if (_isSliding)
            {
                _slideAttackColl.enabled = true;
                _attackColl.enabled = false;
            }
            
            // TODO attacking
            Debug.Log("Hit something!");
            OnPlayerAttack?.Invoke();
        }
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

        if (!ShouldApplyWallJumpBuffer())
        {
            _wallJumpPostBufferTimer -= Time.deltaTime;
        }

        if (_slideBufferTimer > 0f)
        {
            _slideBufferTimer -= Time.deltaTime;
        }

        if (_poseCooldownTimer > 0f)
        {
            _poseCooldownTimer -= Time.deltaTime;
        }

        if (_attackCooldownTimer > 0f)
        {
            _attackCooldownTimer -= Time.deltaTime;
        }        
    }

    #endregion
}
