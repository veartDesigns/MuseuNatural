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
    public Text SequencePage;
    public GameObject EndPanel;
    public UserActionController UserActionController;
    public Button NextButton;
    public EventSystem EventSystem;

    public Animator ClickUI;
    public RectTransform ClickUITransform;
    private int _currentSequence = -1;
    private SequenceInfo _currentSequenceInfo;
    private List<Animator> _currentAnimators;
    private bool _animationRunning;
    private float _animationStartTime;
    private bool _firstTrack = true;
    private bool _trackingActive;
    private Coroutine _nextButtonCoroutine;
    private ObjectInfo _objectClickedInfo;

    private void Awake()
    {
        ShowEndPanel(false);
        ARContainer.transform.localScale *= AllSequenceInfos.ActionContainerSize;
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

        if(_trackingActive){

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
                ClickUI.Play("ClickObject");
                ClickUITransform.position = inputPosition;
                ObjectClicked(hit.transform.gameObject);
            }
        }
    }

    private void ObjectClicked(GameObject objectSelected)
    {
        Debug.Log("ObjectClicked " + objectSelected.name + " " + ClickUI);

        int containerObjects = ARContainer.transform.childCount;
        ObjectInfo objectClickedInfo = objectSelected.transform.GetComponentInChildren<ObjectInfo>();

        if (objectClickedInfo.ObjectType == ObjectType.Lechuza)
        {
            Debug.Log("ANIMATE THAT " + objectSelected.name);
            objectSelected.transform.parent.GetComponent<Animator>().Play("Protagonist");
            return;
        }
        _objectClickedInfo = objectClickedInfo;

        for (int i = 0; i < _currentAnimators.Count; i++)
        {
            Animator animator = _currentAnimators[i];
            ObjectInfo goInfo = animator.gameObject.GetComponentInChildren<ObjectInfo>();

            if (goInfo.ObjectType == _objectClickedInfo.ObjectType)
            {
                animator.Play("Protagonist");
                Debug.Log("ANIMATE THAT " + goInfo.ObjectType);
            }
            else
            {

                string animName = "NotProtagonist";

                if (goInfo.ObjectType == ObjectType.Lechuza)
                {
                    animName += "_" + _objectClickedInfo.ObjectType;
                    animator.Play(animName);
                }
                else if(goInfo.ObjectType != ObjectType.Egagropila && 
                         goInfo.ObjectType != ObjectType.MandibulaLiron &&
                         goInfo.ObjectType != ObjectType.MandibulaRaton &&
                         goInfo.ObjectType != ObjectType.MandibulaMusaraña){

                    Debug.Log("SECOND PLANE  " + goInfo.ObjectType + "  " + animName);
                    animator.Play(animName);
                }
            }
        }
    }

    public void AnimateEgagropila()
    {
        for (int i = 0; i < _currentAnimators.Count; i++)
        {
            Animator animator = _currentAnimators[i];
            GameObject go = animator.gameObject;
            ObjectInfo objectInfo = go.GetComponentInChildren<ObjectInfo>();


            if (objectInfo.ObjectType == ObjectType.Egagropila)
            {
                Debug.Log("ANIMATE EGAGROPILA");
                animator.Play("Egagropila");
            }
            if (objectInfo.ObjectType == ObjectType.OtrosHuesos)
            {
                Debug.Log("ANIMATE OTROS HUESOS");
                animator.Play("ExplodeHuesos");
            }
            if (objectInfo.ObjectType == ObjectType.MandibulaLiron ||
               objectInfo.ObjectType == ObjectType.MandibulaRaton ||
               objectInfo.ObjectType == ObjectType.MandibulaMusaraña)
            {
                Debug.Log("ANIMATE MANDIBULA");
                animator.Play("Mandibula"+ _objectClickedInfo.ObjectType);
            }
        }
    }
    public void AnnounceLastAnimation(){

        for (int i = 0; i < _currentAnimators.Count; i++)
        {
            Animator animator = _currentAnimators[i];
            GameObject go = animator.gameObject;
            ObjectInfo objectInfo = go.GetComponentInChildren<ObjectInfo>();

            if (_objectClickedInfo.ObjectType == objectInfo.ObjectType)
            {
                Debug.Log("ANIMATE LAST " + _objectClickedInfo.ObjectType);
                animator.Play("NotProtagonist");
            }
        }
    }

    public void AnnounceEndOfStory(){
        Debug.Log("THHE EEEEND ");
        for (int i = 0; i < _currentAnimators.Count; i++)
        {
            Animator animator = _currentAnimators[i];
            animator.Play("Idle");
        }
    }

    public void AnimationStart(ObjectInfo objectInfo)
    {
        //Debug.Log("ANIMATION STARTED");
        _animationRunning = true;
        _animationStartTime = Time.time;
    }
    public void AnimationEnd(ObjectInfo objectInfo)
    {
        _animationRunning = false;

        if (objectInfo == null)
        {
            Debug.Log("ANIMATION ENDED");
            return;
        }
        else
        {
            OnAnimationFinished(objectInfo);
        }
    }
    private void OnAnimationFinished(ObjectInfo objectInfo)
    {
        Debug.Log("ANIMATION ENDED " + objectInfo.ObjectType);

        switch (objectInfo.ObjectType)
        {

            case ObjectType.Lechuza:

                break;

            case ObjectType.MandibulaRaton:
            case ObjectType.MandibulaLiron:
            case ObjectType.MandibulaMusaraña:

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
            _currentSequence = AllSequenceInfos.SequenceInfos.Count - 1;
            return;
        }

        Debug.Log("StartSequence " + _currentSequence);

        //SequencePage.text = (_currentSequence+1) + "/" + AllSequenceInfos.SequenceInfos.Count;

        CleanARContainerObjects();

        _currentSequenceInfo = AllSequenceInfos.GetSequenceInfo(_currentSequence);
        //ExplanationText.text = MainController.Instance.GetText(_currentSequenceInfo.LanguageTag);
        AnimationsInfo animations = _currentSequenceInfo.Animations;
        List<GameObject> objectsToAnimate = animations.ActionObjects;
        List<GameObject> staticObjects = animations.StaticObjects;

        for (int i = 0; i < objectsToAnimate.Count; i++)
        {
            GameObject prefab = objectsToAnimate[i];
            GameObject objectToAnimate = Instantiate(prefab, ARContainer.transform);
            objectToAnimate.name = prefab.name;
            string animationName = "Idle"; //animations.AnimationName;
            ObjectInfo objectInfo = objectToAnimate.GetComponentInChildren<ObjectInfo>();
            AnimatorAnnouncer animatorAnnouncer = objectToAnimate.AddComponent<AnimatorAnnouncer>();
            animatorAnnouncer.SetActionController(this, objectInfo);

            Animator animation = objectToAnimate.transform.GetComponentInChildren<Animator>();
            _currentAnimators.Add(animation);
            Debug.Log(objectToAnimate.name + " animationName " + animationName);
            animation.Play(animationName);
        }
        for (int i = 0; i < staticObjects.Count; i++)
        {
            GameObject objectToAnimate = Instantiate(staticObjects[i], ARContainer.transform);
        }
        EnableDisableAnimators(_trackingActive);
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
