using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
   public GameObject MainPrefab;
    // Use this for initialization
    void Awake()
    {
        GameObject main = Instantiate(MainPrefab);
        main.name = "MainScript";

        DontDestroyOnLoad(main);
        Destroy(this);

    }
}
