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

    private bool CheckUserMovements()
    {
        Vector3 dir = Vector3.zero;
        dir.x = -Input.acceleration.y;
        dir.z = Input.acceleration.x;
        dir.y = Input.acceleration.z;

        float roundedX = Mathf.Round(dir.x * 100) / 100;
        float roundedY = Mathf.Round(dir.y * 100) / 100; ;
        float roundedZ = Mathf.Round(dir.z * 100) / 100; ;

        float allRounds = roundedX + roundedY+ roundedZ;
        float difference = allRounds - (_oldAccelerometer.x + _oldAccelerometer.y + _oldAccelerometer.z);
        difference = Mathf.Abs(difference);
        bool movingDevice = Mathf.Abs(difference) >= Sensibility;

        DebugGyro.text = "Is Moving:" + movingDevice + "\n"
            + "difference " + difference;
            
        _oldAccelerometer = new Vector3(roundedX, roundedY, roundedZ);

        return movingDevice;
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
