using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionController : MonoBehaviour {

    public GameObject ARContainer;
    public AllSequenceInfos AllSequenceInfos;
    public DefaultTrackableEventHandler defaultTrackableEventHandler;
    public GameObject ScanMessage;
    public Text ExplanationText; 

    private int _currentSequence;
    private SequenceInfo _currentSequenceInfo;
    Animator[] _currentAnimators;

    private bool _firstTrack = true;

    private void Start()
    {
        defaultTrackableEventHandler.TrackingLost += OnTrackingLost;
        defaultTrackableEventHandler.TrackingFound += OnTrackingFound;
    }

    private void OnDestroy()
    {
        defaultTrackableEventHandler.TrackingLost -= OnTrackingLost;
        defaultTrackableEventHandler.TrackingFound -= OnTrackingFound;
    }

    private void OnTrackingLost()
    {
        EnableDisableAnimators(false);
        ScanMessage.SetActive(true);
    }

    private void OnTrackingFound()
    {
        if(_firstTrack){

            _firstTrack = false;
            StartSequence(_currentSequence);
        }

        EnableDisableAnimators(true);
        ScanMessage.SetActive(false);
    }

    private void EnableDisableAnimators(bool enable)
    {
        if (_currentAnimators == null) return;

        for (int i = 0; i < _currentAnimators.Length; i++)
        {
            Animator animator = _currentAnimators[i];
            animator.enabled = enable;
        }
    }


    public void StartSequence(int sequence)
    {
        if (sequence < 0) 
        {
            Debug.Log("It's first sequence");
            _currentSequence = 0; 
        }
        if (sequence >= AllSequenceInfos.SequenceInfos.Count)
        {
            Debug.Log("It's LAST sequence");
            _currentSequence = AllSequenceInfos.SequenceInfos.Count-1;
            return;
        }

        Debug.Log("SEQUENCE " + _currentSequence + " of " + AllSequenceInfos.SequenceInfos.Count);

        CleanARContainerObjects();

        _currentSequenceInfo = AllSequenceInfos.GetSequenceInfo(_currentSequence);
        ExplanationText.text = MainController.Instance.GetText(_currentSequenceInfo.LanguageTag);
        AnimationsInfo animations = _currentSequenceInfo.Animations;

        List<GameObject> objectsToAnimate = animations.ActionObjects;

        for (int i = 0; i < objectsToAnimate.Count; i++)
        {
            GameObject objectToAnimate = Instantiate(objectsToAnimate[i]);
            objectToAnimate.name = "sequence_" + _currentSequence + "_" + i;
            objectToAnimate.transform.parent = ARContainer.transform;
            string animationName = animations.AnimationName;
            Animator animation = objectToAnimate.transform.GetComponent<Animator>();
            animation.Play(animationName);

        }
        _currentAnimators = ARContainer.GetComponentsInChildren<Animator>();
    }

    private void CleanARContainerObjects()
    {
        int containerObjects = ARContainer.transform.childCount;

        for(int i= containerObjects-1; i >= 0; i-- ){
            GameObject ARObject = ARContainer.transform.GetChild(i).gameObject;
            ARObject.transform.parent = null;
            Destroy(ARObject);
        }
    }

    public void NextSequence(){

        _currentSequence++;
        StartSequence(_currentSequence);
    }

    public void BackSequence(){

        float sequenceTime = 999;
        bool animationRunning = true;

        if (_currentAnimators != null) {

            sequenceTime = GetAnimationTime(_currentAnimators[0]);
            animationRunning = _currentAnimators[0].GetCurrentAnimatorStateInfo(0).normalizedTime < 1;
        }

        if (!animationRunning || sequenceTime < 3f ){
            _currentSequence--;
            Debug.Log("REAL BACK SEQUENCE");
        }else{
            Debug.Log("START SAME SEQUENCE");
        }

        StartSequence(_currentSequence);
    }

    private float GetAnimationTime(Animator myAnimator)
    {
        AnimatorStateInfo animationState = myAnimator.GetCurrentAnimatorStateInfo(0);
        AnimatorClipInfo[] myAnimatorClip = myAnimator.GetCurrentAnimatorClipInfo(0);

        return myAnimatorClip[0].clip.length * animationState.normalizedTime;
    }

    public void ReturnToMainMenu(){
        MainController.Instance.BackToInitScene();
    }
}
