using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class ScalingField : MonoBehaviour
{
    public XRNode HeadPos;
    public Transform HeadSet;
    private Vector3 HeadLastPos = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 PosDiff;
    public static float SetScalingFactor = 3.0f;
    float ScalingFactor;
    float ScalingFactorMultiplier;
    Vector3 RigTransform;
    public static bool ScalingIsTrue = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(HeadSet.position.x);
        float x = HeadSet.position.x;
        float z = HeadSet.position.z;
        if (Mathf.Sqrt(x*x + z*z) < 0.5f){
            ScalingIsTrue = false;
        }
        else{
            ScalingIsTrue = true;
        }

        if (ScalingIsTrue){ //If over point of interest circle, set SF to 1, otherwise leave at set value
            ScalingFactor = SetScalingFactor;
        }
        else{
            ScalingFactor = 1.0f;
        }

        ScalingFactorMultiplier = ScalingFactor - 1.0f; //As by default, real scaling factor is 1 already without scaling field
        PosDiff = HeadSet.position - HeadLastPos;
        RigTransform = transform.position + (PosDiff * ScalingFactorMultiplier); //Multiply XRRig 
        transform.position = new Vector3(RigTransform.x, 0, RigTransform.z); //Set Y transform to 0, apply to rig position
        HeadLastPos = HeadSet.position; //save current head position for next frame update
        // Debug.Log("LastPos:" + HeadLastPos);
    }
}
