using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class ScalingField : MonoBehaviour
{
    public XRNode HeadPos;
    public Transform HeadSet;
    public Transform RigPos;
    private Vector3 HeadLastPos = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 PosDiff;
    public static float SetScalingFactor = 7.0f;
    public static float ScalingFactor;
    
    Vector3 RigTransform;
    // Start is called before the first frame update
    void Start()
    {
        SetScalingFactor = SetScalingFactor - 1.0f;

    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(HeadSet.position.x);
        float Hx = HeadSet.position.x;
        float Hz = HeadSet.position.z;
        ScalingFactor = SetScalingFactor;
        var fields = GameObject.FindGameObjectsWithTag("navifields");
        foreach (GameObject field in fields)
        {
            //distance between headset and centre of circle on both axes
            float DistX = Hx - field.transform.position.x;
            float DistZ = Hz - field.transform.position.z; 
            float Dist = Mathf.Sqrt(DistX*DistX + DistZ*DistZ);
            float InnerR = 0.25f * field.transform.localScale.x;
            float OuterR = 0.5f * field.transform.localScale.x;
            if (Dist >= OuterR){
                
            }
            else if (Dist <= InnerR){
                ScalingFactor = 0.0f;
                
            }
            else if(Dist < OuterR && Dist > InnerR){
                float GradualScaling = (Dist - InnerR) * (1 / InnerR) * (SetScalingFactor);
                if (GradualScaling < ScalingFactor){
                    ScalingFactor = GradualScaling;
                }
                
            }

        }
        // Debug.Log("Final Scaling Factor =" + ScalingFactor);
        PosDiff = HeadSet.position - HeadLastPos;
        RigTransform = transform.position + (PosDiff * ScalingFactor); //Multiply XRRig 
        transform.position = new Vector3(RigTransform.x, 0, RigTransform.z); //Set Y transform to 0, apply to rig position
        HeadLastPos = HeadSet.position; //save current head position for next frame update
        // Debug.Log("LastPos:" + HeadLastPos);
    }
}
