using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LanguageAllInfo", menuName = "Language/allScriptable")]

public class LanguageAllInfo : ScriptableObject
{
    public List<LanguageInfos> languageInfos;

    public LanguageInfos GetLanguage(Language language)
    {
        for (int i = 0; i < languageInfos.Count; i++)
        {
            if (languageInfos[i].Language == language)
            {
                return languageInfos[i];
            }
        }
        Debug.LogError("Language " + language + " not found in LanguageAllInfo");
        return null;
    }
}
