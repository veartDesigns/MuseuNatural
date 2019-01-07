using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class IdleStateManager : StateMachineBehaviour
{
    List<Collider> _colliders;
    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _colliders = animator.gameObject.GetComponentsInChildren<Collider>().ToList();

        EnableDisableColliders(true);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnableDisableColliders(false);
    }

    private void EnableDisableColliders(bool enable){

        for (int i = 0; i < _colliders.Count; i++)
        {
            _colliders[i].enabled = enable;
        }
    }
}