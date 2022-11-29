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
        float SF = ScalingField.ScalingFactor + 1.0f;
        SF = Mathf.Round(SF * 10f) / 10f;
        string ScalingFactorText = SF.ToString();
        ScalingText.text = "Scaling: " + ScalingFactorText;
    }
}
