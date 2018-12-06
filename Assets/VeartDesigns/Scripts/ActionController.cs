using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionController : MonoBehaviour
{

    public GameObject ARContainer;
    public AllSequenceInfos AllSequenceInfos;
    public DefaultTrackableEventHandler defaultTrackableEventHandler;
    public GameObject ScanMessage;
    public Text ExplanationText;
    public Text SequencePage;
    public GameObject EndPanel;
    public UserActionController UserActionController;
    private int _currentSequence;
    private SequenceInfo _currentSequenceInfo;
    private List<Animator> _currentAnimators;
    private bool _animationRunning;
    private float _animationStartTime;
    private bool _firstTrack = true;
    private bool _oldAnimationRunningState;

    private void Awake()
    {
        ShowEndPanel(false);
    }

    private void Start()
    {
        _currentAnimators = new List<Animator>();

        defaultTrackableEventHandler.TrackingLost += OnTrackingLost;
        defaultTrackableEventHandler.TrackingFound += OnTrackingFound;
    }

    private void OnDestroy()
    {
        defaultTrackableEventHandler.TrackingLost -= OnTrackingLost;
        defaultTrackableEventHandler.TrackingFound -= OnTrackingFound;
    }
    private void Update()
    {
        if(_animationRunning)
        {
            UserActionController.RestartCounterTime();
        }
    }

    public void AnimationStart(){

        _animationRunning = true;
        _animationStartTime = Time.time;
    }
    public void AnimationEnd()
    {
        OnAnimationFinished();
        _animationRunning = false;
    }
    private void OnAnimationFinished()
    {
        Debug.Log("OnAnimationFinished " + _currentSequence + " " + AllSequenceInfos.SequenceInfos.Count);

        if((_currentSequence) >= AllSequenceInfos.SequenceInfos.Count)
        {
            ShowEndPanel(true);
        }

        if(_currentSequence < AllSequenceInfos.SequenceInfos.Count - 1 ){

            AnimateNextButton(true);
        }
    }

    private void AnimateNextButton(bool state)
    {
        Debug.Log("AnimateNextButton "+ state);
    }

    private void ShowEndPanel(bool state)
    {
        EndPanel.SetActive(state);

    }
    public void CloseEndPanel()
    {
        ShowEndPanel(false);
        Debug.Log("Close Panel");
    }
    public void RepeatAnimation(){

        EndPanel.SetActive(false);
        _currentSequence = 0;
        NextSequence();
    }

    private void OnTrackingLost()
    {
        EnableDisableAnimators(false);
        ScanMessage.SetActive(true);
    }

    private void OnTrackingFound()
    {
        if (_firstTrack)
        {
            _firstTrack = false;
            _currentSequence = 0;
            NextSequence();
        }

        EnableDisableAnimators(true);
        ScanMessage.SetActive(false);
    }

    private void EnableDisableAnimators(bool enable)
    {
        if (_currentAnimators == null) return;

        for (int i = 0; i < _currentAnimators.Count; i++)
        {
            Animator animator = _currentAnimators[i];
            animator.enabled = enable;
        }
    }

    public void StartSequence(int sequence)
    {
        if (sequence <= 0)
        {
            _currentSequence = 0;
        }
        if (sequence >= AllSequenceInfos.SequenceInfos.Count)
        {
            _currentSequence = AllSequenceInfos.SequenceInfos.Count-1;
            Debug.Log("StartSequence Blocked " + _currentSequence);
            return;
        }

        Debug.Log("StartSequence " + _currentSequence);

        AnimateNextButton(false);

        SequencePage.text = (_currentSequence+1) + "/" + AllSequenceInfos.SequenceInfos.Count;

        CleanARContainerObjects();

        _currentSequenceInfo = AllSequenceInfos.GetSequenceInfo(_currentSequence);
        ExplanationText.text = MainController.Instance.GetText(_currentSequenceInfo.LanguageTag);
        AnimationsInfo animations = _currentSequenceInfo.Animations;

        List<GameObject> objectsToAnimate = animations.ActionObjects;

        for (int i = 0; i < objectsToAnimate.Count; i++)
        {
            GameObject objectToAnimate = Instantiate(objectsToAnimate[i], ARContainer.transform);
            objectToAnimate.name = "sequence_" + _currentSequence + "_" + i;
            string animationName = animations.AnimationName;
            if (i == 0)
            {
                AnimatorAnnouncer animatorAnnouncer = objectToAnimate.AddComponent<AnimatorAnnouncer>();
                animatorAnnouncer.SetActionController(this);
            }
             Animator animation = objectToAnimate.transform.GetComponentInChildren<Animator>();
            _currentAnimators.Add(animation);
            animation.Play(animationName);
        }
    }

    private void CleanARContainerObjects()
    {
        int containerObjects = ARContainer.transform.childCount;

        for (int i = containerObjects - 1; i >= 0; i--)
        {
            GameObject ARObject = ARContainer.transform.GetChild(i).gameObject;
            ARObject.transform.parent = null;
            Destroy(ARObject);
        }
        _currentAnimators.Clear();
    }

    public void NextSequence()
    {
        StartSequence(_currentSequence);
        _currentSequence++;
    }

    public void BackSequence()
    {
        float sequenceTime = 999;

        if (_currentAnimators != null)
        {
            sequenceTime = _animationStartTime-Time.time;
        }
        Debug.Log("BackSequence sequenceTime " + sequenceTime);

        if (!_animationRunning || sequenceTime < 3f)
        {
            _currentSequence--;
        }

        StartSequence(_currentSequence);
    }

    private float GetAnimationTime(Animator myAnimator)
    {
        float normalizedTime = GetNormalizedAnimationTime(myAnimator);
        AnimatorClipInfo[] myAnimatorClip = myAnimator.GetCurrentAnimatorClipInfo(0);
        return myAnimatorClip[0].clip.length * normalizedTime;
    }

    private float GetNormalizedAnimationTime(Animator myAnimator)
    {
        AnimatorStateInfo animationState = myAnimator.GetCurrentAnimatorStateInfo(0);
        return animationState.normalizedTime;
    }

    public void ReturnToMainMenu()
    {
        MainController.Instance.BackToInitScene();
    }
}
