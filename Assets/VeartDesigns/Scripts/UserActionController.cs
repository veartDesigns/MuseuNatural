using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UserActionController : MonoBehaviour
{
    public EventSystem EventSystem;
    public Text DebugGyro;
    private float _secondsToBackToInit = 25f;
    private float _startTime;
    private Vector3 _oldAccelerometer;
    private float Sensibility = 0.02f;

    private void Awake(){

        _startTime = Time.time;
    }

    void Update()
    {
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
