using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LanguageInfo", menuName = "Language/Scriptable")]
public class LanguageInfos : ScriptableObject {

    public Language Language;
    public List<LanguageInfo> languageInfos;
    public string GetStringByTag(LanguageTag languageTag){

        for (int i = 0; i < languageInfos.Count; i++){

            LanguageInfo languageInfo = languageInfos[i];
            if(languageInfo.LanguageTag == languageTag){
                return languageInfo.Text;
            }
        }
        Debug.LogWarning("Language Tag " + languageTag + " not found in "+ Language);
        return languageTag.ToString();
    }
}