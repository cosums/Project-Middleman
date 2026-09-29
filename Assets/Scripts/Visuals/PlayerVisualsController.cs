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

    private PlayerController playerController;

    static readonly int IsAttackingBool = Animator.StringToHash("isAttacking");
    static readonly int AttackTrigger = Animator.StringToHash("attack");
    static readonly int XVelocityFloat = Animator.StringToHash("xVelocity");
    static readonly int InAirBool = Animator.StringToHash("inAir");
    static readonly int FlipBool = Animator.StringToHash("isFlipping");
    static readonly int SlideBool = Animator.StringToHash("isSliding");

    private bool inAir;
    private bool isFlipping;
    private bool isSliding;
    private bool isAttacking;


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
            playerRigAnimator.SetTrigger(AttackTrigger);
            StartCoroutine(AttackBoolSequence(true));

        }

        // jump
        if (inAir != playerController.GetInAir())
        {
            inAir = playerController.GetInAir();
            playerRigAnimator.SetBool(InAirBool, inAir);
        }

        // flip
        if (isFlipping != (playerController.GetRemainingJumps() == 0))
        {
            isFlipping = (playerController.GetRemainingJumps() == 0);
            playerRigAnimator.SetBool(FlipBool, isFlipping);
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

    private IEnumerator AttackBoolSequence(bool enable)
    {
        if (isAttacking == enable) yield break;

        Debug.Log("attack bool sequence");

        isAttacking = enable;
        playerRigAnimator.SetBool(IsAttackingBool, true);

        yield return new WaitForSeconds(0.3f);

        playerRigAnimator.SetBool(IsAttackingBool, !isAttacking);
        isAttacking = !isAttacking;
    }


}