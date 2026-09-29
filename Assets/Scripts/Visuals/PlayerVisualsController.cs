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
    
    static readonly int FlipBool = Animator.StringToHash("isFlipping");
    static readonly int KickTrigger = Animator.StringToHash("kick");
    static readonly int XVelocityFloat = Animator.StringToHash("xVelocity");
    static readonly int JumpBool = Animator.StringToHash("inAir");
    static readonly int SlideBool = Animator.StringToHash("isSliding");

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
            if (playerController.GetInAir()) playerRigAnimator.SetTrigger(KickTrigger);
            if (punchParticle != null) punchParticle.Play();
        }


        // jump
        playerRigAnimator.SetBool(JumpBool, playerController.GetInAir());

        // flip
        bool isFlipping = playerController.GetRemainingJumps() == 0;
        playerRigAnimator.SetBool(FlipBool, isFlipping);

        // Movement
        playerRigAnimator.SetBool(SlideBool, playerController.GetIsSliding());
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


}