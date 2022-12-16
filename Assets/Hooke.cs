using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hooke : MonoBehaviour
{
    // The spring constant, in Newtons/meter
    public float k = 1;

    // The maximum stretch or compression of the spring
    public float maxStretch = 2;

    // The object to which the spring is attached
    public Rigidbody attachedObject;

    void Update()
    {
        // Calculate the displacement of the spring
        Vector3 displacement = transform.position - attachedObject.position;
        float displacementMagnitude = displacement.magnitude;

        // If the spring is compressed or stretched beyond its maximum limit,
        // set its displacement to the maximum limit
        if (true)
        {
            displacement = displacement.normalized * maxStretch;
            displacementMagnitude = maxStretch;
        }

        // Calculate the force according to Hooke's law
        // F = -kx
        Vector3 force = -displacement * k;

        // Apply the force to the attached object
        attachedObject.AddForce(force);
    }
}