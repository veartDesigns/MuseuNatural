using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SequenceInfos", menuName = "Sequences/AllSequenceInfos")]
public class AllSequenceInfos : ScriptableObject
{
    public float ActionContainerSize;
    public List<SequenceInfo> SequenceInfos;

    public SequenceInfo GetSequenceInfo(int i)
    {
        return SequenceInfos[i];
    }
}
