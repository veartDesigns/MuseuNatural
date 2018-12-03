using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialController : MonoBehaviour
{

    public void SetLanguage(int i)
    {
        MainController.Instance.SetLanguage(i);
    }

    public void LoadActingScene()
    {
        MainController.Instance.LoadActingScene();
    }
    public void LoadCreditsScene()
    {
        MainController.Instance.LoadCreditsScene();
    }
}
