using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ControllerUIText : MonoBehaviour
{
    public TextMeshPro ScalingText;
    // Update is called once per frame
    void Update()
    {
        string ScalingFactorText = ScalingField.SetScalingFactor.ToString();
        string ScalingTrueText = ScalingField.ScalingIsTrue.ToString();
        ScalingText.text = "Scaling: " + ScalingFactorText + "\n" + ScalingTrueText;
    }
}
