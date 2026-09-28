using System.Diagnostics.Tracing;
using UnityEngine;

public class PlayerVisualsController : MonoBehaviour
{
    [SerializeField] Animator playerRigAnimator;

    [Header("Particles")]
    [SerializeField] ParticleSystem runParticle;
    [SerializeField] ParticleSystem slideParticle;
    [SerializeField] ParticleSystem punchParticle;

    private PlayerController playerController;
    
    static readonly int PoseTrigger = Animator.StringToHash("Pose");
    static readonly int AttackTrigger = Animator.StringToHash("Attack");
    static readonly int xVelocityFloat = Animator.StringToHash("xVelocity");
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

        if (f.PoseWasPressed) playerRigAnimator.SetTrigger(PoseTrigger);
        if (f.AttackWasPressed)
        {
            playerRigAnimator.SetTrigger(AttackTrigger);
            if (punchParticle != null) punchParticle.Play();
        }

        playerRigAnimator.SetBool(JumpBool, playerController.GetInAir());
        playerRigAnimator.SetBool(SlideBool, playerController.GetIsSliding());
        playerRigAnimator.SetFloat(xVelocityFloat, Mathf.Abs(f.Move.x));
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