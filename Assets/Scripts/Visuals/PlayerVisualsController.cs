using System.Collections;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerVisualsController : MonoBehaviour
{
    [SerializeField] Animator playerRigAnimator;

    [Header("Particles")]
    [SerializeField] ParticleSystem runParticle;
    [SerializeField] ParticleSystem slideParticle;
    [SerializeField] ParticleSystem punchParticle;
    [SerializeField] ParticleSystem jumpParticle;
    [SerializeField] ParticleSystem flipParticle;
    [SerializeField] ParticleSystem kickParticle;

    private PlayerController playerController;

    static readonly int IsAttackingBool = Animator.StringToHash("isAttacking");
    static readonly int XVelocityFloat = Animator.StringToHash("xVelocity");
    static readonly int InAirBool = Animator.StringToHash("inAir");
    static readonly int FlipBool = Animator.StringToHash("isFlipping");
    static readonly int SlideBool = Animator.StringToHash("isSliding");

    private bool inAir;
    private bool isFlipping;
    private bool isSliding;


    InputFrame _currentInputFrame;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void SimulateTick(InputFrame frame)
    {
        _currentInputFrame = frame;

        UpdatePlayerAnimation();
        UpdatePlayerParticles();
    }



    private void UpdatePlayerAnimation()
    {
        var f = _currentInputFrame;
        
        if (f.AttackWasPressed)
        {
            StartCoroutine(AttackBoolSequence());
        }

        // jump
        if (inAir != playerController.GetInAir())
        {
            inAir = playerController.GetInAir();
            playerRigAnimator.SetBool(InAirBool, inAir);

            if (playerController.GetRemainingJumps() >= 0)
            {
                ParticleSystem particle = Instantiate(jumpParticle);
                particle.transform.position = GetGroundPosition();
            }
        }

        // flip
        if (isFlipping != (playerController.GetRemainingJumps() == 0))
        {
            isFlipping = (playerController.GetRemainingJumps() == 0);
            playerRigAnimator.SetBool(FlipBool, isFlipping);

            if (playerController.GetInAir())
            {
                ParticleSystem particle = Instantiate(flipParticle);
                particle.transform.position = playerController.GetFeetCollider().transform.position;
                particle.transform.SetParent(playerController.GetFeetCollider().transform);
            }
        }
        
        // Movement
        if (isSliding != playerController.GetIsSliding())
        {
            isSliding = playerController.GetIsSliding();
            playerRigAnimator.SetBool(SlideBool, isSliding);
        }
        playerRigAnimator.SetFloat(XVelocityFloat, Mathf.Abs(f.Move.x));
    }

    private void UpdatePlayerParticles()
    {

        // Ground particles
        if (playerController.GetInAir()) return;
        if (playerController.GetPlayerSpeed() < 1f)
        {
            DisableParticle(runParticle);
            DisableParticle(slideParticle);
            return;
        }

        // Sliding Particles
        if (_currentInputFrame.SlideWasPressed)
        {
            slideParticle.Emit(10);
        }
        if (playerController.GetIsSliding())
        { 
            EnableParticle(slideParticle);
            DisableParticle(runParticle);
            return;
        }

        // Running particles
        if (Mathf.Abs(_currentInputFrame.Move.x) > 0.01f) EnableParticle(runParticle);
    }

    private Vector3 GetGroundPosition()
    {
        Vector3 origin = playerController.groundPoint.transform.position;

        // Start slightly above the feet so the ray doesn't begin inside the ground
        RaycastHit2D hit = Physics2D.Raycast(origin + Vector3.up * 0.5f, Vector2.down, 10f, playerController.groundLayer);
        if (hit.collider != null)
        {
            return hit.point;
        }

        // Fallback if nothing was hit
        print("nothing hit");
        return origin;
    }

    private void EnableParticle(ParticleSystem particle)
    {
        if (!particle.isPlaying)
            particle.Play();
    }

    private void DisableParticle(ParticleSystem particle)
    {
        if (particle.isPlaying)
            particle.Stop();
    }

    private IEnumerator AttackBoolSequence()
    {
        playerRigAnimator.SetBool(IsAttackingBool, true);
        yield return new WaitForSeconds(0.4f);
        playerRigAnimator.SetBool(IsAttackingBool, false);
    }


}