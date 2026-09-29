using UnityEngine;

public class DisableOnExit : StateMachineBehaviour
{
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Disables the GameObject that holds the Animator component
        animator.gameObject.SetActive(false);
    }
}