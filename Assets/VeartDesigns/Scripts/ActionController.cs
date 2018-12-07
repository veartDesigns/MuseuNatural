using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionController : MonoBehaviour
{
    public Text debugQuality;

    public GameObject ARContainer;
    public AllSequenceInfos AllSequenceInfos;
    public DefaultTrackableEventHandler defaultTrackableEventHandler;
    public GameObject ScanMessage;
    public Text ExplanationText;
    public Text SequencePage;
    public GameObject EndPanel;
    public UserActionController UserActionController;
    public Button NextButton;
    public EventSystem EventSystem;
    private int _currentSequence = -1;
    private SequenceInfo _currentSequenceInfo;
    private List<Animator> _currentAnimators;
    private bool _animationRunning;
    private float _animationStartTime;
    private bool _firstTrack = true;
    private bool _trackingActive;
    private Coroutine _nextButtonCoroutine;

    private void Awake()
    {
        ShowEndPanel(false);

    }

    private void Start()
    {
        _currentAnimators = new List<Animator>();

        defaultTrackableEventHandler.TrackingLost += OnTrackingLost;
        defaultTrackableEventHandler.TrackingFound += OnTrackingFound;

        int qualityLevel = QualitySettings.GetQualityLevel();
        debugQuality.text = QualitySettings.names[qualityLevel];

    }

    private void OnDestroy()
    {
        DesuscribeToTracker();
    }
    private void Update()
    {
        if(_animationRunning)
        {
            UserActionController.RestartCounterTime();
        }

        if(EndPanel.active){
            ScanMessage.SetActive(false);
        }
    }

    public void AnimationStart(){
        Debug.Log("ANIMATION STARTED");
        _animationRunning = true;
        _animationStartTime = Time.time;
    }
    public void AnimationEnd()
    {
        Debug.Log("ANIMATION ENDED");
        OnAnimationFinished();
        _animationRunning = false;
    }
    private void OnAnimationFinished()
    {
        if((_currentSequence) >= AllSequenceInfos.SequenceInfos.Count-1)
        {
            ShowEndPanel(true);
        }

        if(_currentSequence < AllSequenceInfos.SequenceInfos.Count - 1 ){

            AnimateNextButton(true);
        }
    }

    private void AnimateNextButton(bool state)
    {
        Debug.Log("AnimateNextButton " + state);
        if (state) _nextButtonCoroutine = StartCoroutine(AnimateButton());
    }

    private IEnumerator AnimateButton()
    {
        while (true)
        {
            EventSystem.SetSelectedGameObject(NextButton.gameObject);
            yield return new WaitForSeconds(0.4f);
            EventSystem.SetSelectedGameObject(null);
            yield return new WaitForSeconds(0.4f);
        }
        yield break;
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
        _currentSequence = -1;
        NextSequence();
    }

    private void OnTrackingLost()
    {
        _trackingActive = false;

        EnableDisableAnimators(false);
        ScanMessage.SetActive(true);
    }

    private void OnTrackingFound()
    {
        _trackingActive = true;

        if (_firstTrack)
        {
            _firstTrack = false;
            _currentSequence = -1;
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
        EventSystem.SetSelectedGameObject(null);

        if (sequence <= 0)
        {
            _currentSequence = 0;
        }
        if (sequence >= AllSequenceInfos.SequenceInfos.Count)
        {
            _currentSequence = AllSequenceInfos.SequenceInfos.Count-1;
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

            if(!_trackingActive) EnableDisableRenderer(objectToAnimate, _trackingActive);
        }

        EnableDisableAnimators(_trackingActive);
    }

    private void EnableDisableRenderer(GameObject objectToAnimate,bool enable)
    {
        Renderer[] renderers = objectToAnimate.transform.GetComponentsInChildren<Renderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            renderer.enabled = enable;
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
        if(_nextButtonCoroutine != null) StopCoroutine(_nextButtonCoroutine);

        _currentSequence++;
        StartSequence(_currentSequence);
    }

    public void BackSequence()
    {
        float sequenceTime = 999;

        if (_currentAnimators != null)
        {
            sequenceTime = Time.time - _animationStartTime;
        }
        Debug.Log("BackSequence  " + _animationRunning + "  "+ sequenceTime + " "+ _currentSequence);

        if (!_animationRunning || sequenceTime < 3f)
        {
            Debug.Log("FORCE REAL BACK");
            _currentSequence--;

        }else{
            Debug.Log("PLAY SAME");
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
        DesuscribeToTracker();
        MainController.Instance.BackToInitScene();
    }

    private void DesuscribeToTracker()
    {
        defaultTrackableEventHandler.TrackingLost -= OnTrackingLost;
        defaultTrackableEventHandler.TrackingFound -= OnTrackingFound;
    }
}
