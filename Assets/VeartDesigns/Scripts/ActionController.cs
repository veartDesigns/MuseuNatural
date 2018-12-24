using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionController : MonoBehaviour
{
    public float VRMarkerScale = 2.24f;
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
    private ObjectInfo _objectInfo;

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
        ClickOverObject();

        if (_animationRunning)
        {
            UserActionController.RestartCounterTime();
        }

        if (EndPanel.active)
        {
            ScanMessage.SetActive(false);
        }
    }

    private void ClickOverObject()
    {
        Vector2 inputPosition = Vector2.zero;
        bool click = false;
        if (Input.GetMouseButtonDown(0))
        {
            click = true;
            inputPosition = Input.mousePosition;
        }
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase.Equals(TouchPhase.Began))
        {
            click = true;
            inputPosition = Input.touches[0].position;
        }

        if (click)
        {
            Ray ray = Camera.main.ScreenPointToRay(inputPosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000.0f))
            {
                ObjectClicked(hit.transform.gameObject);
            }
        }
    }

    private void ObjectClicked(GameObject objectSelected)
    {
        int containerObjects = ARContainer.transform.childCount;
        ObjectInfo objectInfo = objectSelected.GetComponent<ObjectInfo>();

        if(objectInfo.ObjectType == ObjectType.Lechuza ){

            Debug.Log("ANIMATE THAT " + objectSelected.name);
            objectSelected.GetComponent<Animator>().Play("Protagonist");
            return;
        }

        for (int i = 0; i < _currentAnimators.Count; i++)
        {
            Animator animator = _currentAnimators[i];
            GameObject goAr = animator.gameObject;

            if (objectSelected.Equals(goAr)){
                animator.Play("Protagonist");
                Debug.Log("ANIMATE THAT " + goAr.name);
            }else{

                string animName = "NotProtagonist";
                if(goAr.name == "Lechuza"){
                    animName += "_" + objectInfo.ObjectType;
                }
                Debug.Log("SECOND PLANE  " + goAr.name + "  " + animName);

                animator.Play(animName);
            }
        }

    }

    public void AnimateEgagropila()
    {
        for (int i = 0; i < _currentAnimators.Count; i++)
        {
            Animator animator = _currentAnimators[i];
            GameObject go = animator.gameObject;
            ObjectInfo objectInfo = go.GetComponent<ObjectInfo>();

            if(objectInfo.ObjectType == ObjectType.Egagropila){

                animator.Play("Egagropila");
            }
        }
    }
    public void AnimationStart(ObjectInfo objectInfo){

        //Debug.Log("ANIMATION STARTED");
        _animationRunning = true;
        _animationStartTime = Time.time;
    }
    public void AnimationEnd(ObjectInfo objectInfo)
    {
        _animationRunning = false;

        if (objectInfo == null){
            Debug.Log("ANIMATION ENDED");
            return;
        }else{
            OnAnimationFinished(objectInfo);
        }
    }
    private void OnAnimationFinished(ObjectInfo objectInfo)
    {
        Debug.Log("ANIMATION ENDED " + objectInfo.ObjectType);

        switch (objectInfo.ObjectType){

            case ObjectType.Lechuza:
                break;
            case ObjectType.Mandibula:
                break;
        }
    }

    private void ShowEndPanel(bool state)
    {
        EndPanel.SetActive(state);
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
            StartSequence(_currentSequence);
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
        //EventSystem.SetSelectedGameObject(null);

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

        //SequencePage.text = (_currentSequence+1) + "/" + AllSequenceInfos.SequenceInfos.Count;

        CleanARContainerObjects();

        _currentSequenceInfo = AllSequenceInfos.GetSequenceInfo(_currentSequence);
        ExplanationText.text = MainController.Instance.GetText(_currentSequenceInfo.LanguageTag);
        AnimationsInfo animations = _currentSequenceInfo.Animations;
        List<GameObject> objectsToAnimate = animations.ActionObjects;
        List<GameObject> staticObjects = animations.StaticObjects;

        for (int i = 0; i < objectsToAnimate.Count; i++)
        {
            GameObject prefab = objectsToAnimate[i];
            GameObject objectToAnimate = Instantiate(prefab, ARContainer.transform);
            objectToAnimate.name = prefab.name;
            string animationName = "Idle"; //animations.AnimationName;
            ObjectInfo objectInfo = gameObject.GetComponent<ObjectInfo>();

            AnimatorAnnouncer animatorAnnouncer = objectToAnimate.AddComponent<AnimatorAnnouncer>();
            animatorAnnouncer.SetActionController(this, objectInfo);
           
            Animator animation = objectToAnimate.transform.GetComponentInChildren<Animator>();
            _currentAnimators.Add(animation);
            Debug.Log(objectToAnimate.name + " animationName " + animationName); 
            animation.Play(animationName);

            if(!_trackingActive) EnableDisableRenderer(objectToAnimate, _trackingActive);
        }
        for (int i = 0; i < staticObjects.Count; i++)
        {
            GameObject objectToAnimate = Instantiate(staticObjects[i], ARContainer.transform);
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
