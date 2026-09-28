using UnityEngine;

public class PlayerVisualsController : MonoBehaviour
{
    [SerializeField] Animator playerRigAnimator;
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


        playerRigAnimator.SetFloat(xVelocityFloat, Mathf.Abs(f.Move.x));
    }

    private void Update()
    {
        Debug.Log("Jumping: " + playerController.GetInAir() + " Sliding: " + playerController.GetIsSliding());
        playerRigAnimator.SetBool(JumpBool, playerController.GetInAir());
        playerRigAnimator.SetBool(SlideBool, playerController.GetIsSliding());
    }


}