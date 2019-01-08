using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class ScaleSetterManager : MonoBehaviour
{
    public AllSequenceInfos AllSequenceInfos;
    public InputField InputField;

    public void SetScale()
    {
        float scale = float.Parse(InputField.text, CultureInfo.InvariantCulture.NumberFormat);
        AllSequenceInfos.ActionContainerSize = scale;
        Destroy(gameObject);
    }
}
