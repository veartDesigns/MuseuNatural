using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizeString : MonoBehaviour {

    public Text Text;
    public LanguageTag LanguageTag;
	// Use this for initialization
	void Start () {

        Text.text = MainController.Instance.GetText(LanguageTag);
	}
}
