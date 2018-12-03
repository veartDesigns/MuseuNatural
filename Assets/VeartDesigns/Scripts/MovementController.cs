using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{

    public bool DebugGyro;
    public float _secondsToBackToInit = 10f;
    public float MovementeSensibility = 5;
    private Quaternion _oldMovementValue = Quaternion.identity;
    Gyroscope m_Gyro;
    private float _startTime;

    private void Awake(){

        _startTime = Time.time;
    }
    // Update is called once per frame
    void Update()
    {
        m_Gyro = Input.gyro;
        m_Gyro.enabled = true;
        Quaternion GyroMovement = m_Gyro.attitude;
        float currentMovementValue = Quaternion.Angle(m_Gyro.attitude, _oldMovementValue);
        bool moved = false;
        Debug.Log("diff " + currentMovementValue);
        if (currentMovementValue > MovementeSensibility)
        {
            _oldMovementValue = GyroMovement;
            _startTime = Time.time;
            moved = true;
        }

        if (!moved) CheckTime();
    }
    private void CheckTime()
    {

        float currentTime = Time.time;
        float timePassed = currentTime - _startTime;
        Debug.Log("TimePassed " + timePassed);
        if (timePassed >= _secondsToBackToInit)
        {
            _startTime = Time.time;
            MainController.Instance.BackToInitScene();
        }
    }

    void OnGUI()
    {
        if (DebugGyro)
        {
            float currentMovementValue = Quaternion.Angle(m_Gyro.attitude, _oldMovementValue);

            //Output the rotation rate, attitude and the enabled state of the gyroscope as a Label
            GUI.Label(new Rect(500, 300, 200, 40), "Gyro rotation rate " + m_Gyro.rotationRate);
            GUI.Label(new Rect(500, 350, 200, 40), "Gyro attitude" + m_Gyro.attitude);
            GUI.Label(new Rect(500, 400, 200, 40), "Gyro enabled : " + m_Gyro.enabled);
            GUI.Label(new Rect(500, 450, 200, 40), "Gyro diff : " + currentMovementValue);
        }
    }
}
