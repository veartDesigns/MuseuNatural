using UnityEngine;
using UnityEngine.EventSystems;

public class UserActionController : MonoBehaviour
{
    public EventSystem EventSystem;
    private float _secondsToBackToInit = 25f;
    private float _startTime;

    private void Awake(){

        _startTime = Time.time;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()){
                RestartCounterTime();
            }
        }
            CheckTime();
    }

    public void RestartCounterTime(){

//        Debug.Log("RestartCounterTime");

        _startTime = Time.time;
    }

    private void CheckTime()
    {
        float currentTime = Time.time;
        float timePassed = currentTime - _startTime;
        if (timePassed >= _secondsToBackToInit)
        {
            _startTime = Time.time;
            Debug.Log("NO ACTION, BACK TO INIT MENU");
            MainController.Instance.BackToInitScene();
        }
    }
}
