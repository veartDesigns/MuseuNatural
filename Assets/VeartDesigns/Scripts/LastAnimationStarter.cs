using UnityEngine;

public class LastAnimationStarter : StateMachineBehaviour
{
    private AnimatorAnnouncer animatorAnnouncer;

    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animatorAnnouncer = animator.gameObject.transform.GetComponent<AnimatorAnnouncer>();
        animatorAnnouncer.AnnounceLastAnimation();
    }
}