using UnityEngine;

public class IdleStateManager : StateMachineBehaviour
{
    BoxCollider _boxCollider;
    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _boxCollider = animator.gameObject.GetComponent<BoxCollider>();
        _boxCollider.enabled = true;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _boxCollider.enabled = false;
    }
}