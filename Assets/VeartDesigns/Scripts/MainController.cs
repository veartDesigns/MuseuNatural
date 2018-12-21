using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour {

    public LanguageAllInfo LanguageAllInfo;
    private LanguageInfos _languageInfos;

    private static MainController _instance = null;

    private void Awake()
    {
        _instance = this;
        LoadScene(AppScenes.InitialScene);
        SetLanguage(0);
    }

    public static MainController Instance
    {
        get
        {
            return _instance;
        }
    }

    public void SetLanguage(int lang)
    {
        _languageInfos = LanguageAllInfo.GetLanguage((Language)lang);
        Debug.Log("_languageInfos " + _languageInfos.Language);
    }
    public void LoadCreditsScene(){
        LoadScene(AppScenes.CreditsScene);
    }
    public void BackToInitScene()
    {
        LoadScene(AppScenes.InitialScene);
    }
    public void LoadActingScene(){

        LoadScene(AppScenes.ActionScene);
    }
    public void LoadScene(AppScenes scene){

        Debug.Log("LoadScene " + scene);

        SceneManager.LoadScene(scene.ToString());
    }
    public string GetText (LanguageTag languageTag) {

        return _languageInfos.GetStringByTag(languageTag);
    }

}
