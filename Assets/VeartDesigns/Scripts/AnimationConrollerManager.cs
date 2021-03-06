﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationConrollerManager : StateMachineBehaviour {

    private AnimatorAnnouncer animatorAnnouncer;
    private bool _ended = false;
    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animatorAnnouncer = animator.gameObject.transform.GetComponent<AnimatorAnnouncer>();
        animatorAnnouncer.AnnounceStart();
    }

	// OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

        float normalizedTime = GetNormalizedAnimationTime(animator);

        if(normalizedTime >= 1 && !_ended)
        {
            animatorAnnouncer.AnnounceEnd();
            _ended = true;
        }
    }

    private float GetNormalizedAnimationTime(Animator myAnimator)
    {
        AnimatorStateInfo animationState = myAnimator.GetCurrentAnimatorStateInfo(0);
        return animationState.normalizedTime;
    }
    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateMove is called before OnStateMove is called on any state inside this state machine
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called before OnStateIK is called on any state inside this state machine
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateMachineEnter is called when entering a statemachine via its Entry Node
    //override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash){
    //
    //}

    // OnStateMachineExit is called when exiting a statemachine via its Exit Node
    //override public void OnStateMachineExit(Animator animator, int stateMachinePathHash) {
    //
    //}
}
