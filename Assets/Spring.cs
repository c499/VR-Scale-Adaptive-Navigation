using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    // float y = 0;
    public float restLength = 0;
    //float velocity = 0;
    public float k = 1;
    float x = 0;
    float force = 0;
    public Rigidbody attachedObject;


    // Start is called before the first frame update
    void Start()
    {
        //attachedObject.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        x = attachedObject.position.y - restLength;
        force = - k * x; //mass is ignored, can multiply by m later

        attachedObject.AddForce(new Vector3(0, force, 0));
    }
}
