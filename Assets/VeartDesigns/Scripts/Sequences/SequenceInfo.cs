using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class SequenceInfo
{
    public LanguageTag LanguageTag;
    public AnimationsInfo Animations;
}
[Serializable]
public class AnimationsInfo
{
    public List<GameObject> ActionObjects;
    public string AnimationName;
}