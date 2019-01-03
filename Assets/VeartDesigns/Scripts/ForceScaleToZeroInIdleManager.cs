using UnityEngine;

public class ForceScaleToZeroInIdleManager : StateMachineBehaviour
{
    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.transform.localScale = Vector3.zero*0.001f;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.transform.localScale = Vector3.one;
    }
}