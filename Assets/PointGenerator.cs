using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointGenerator : MonoBehaviour
{
    public int numPoints = 100; // number of points to generate
    public float squareSize = 5.0f; // size of the square

    void Start()
    {
        // calculate the spacing between points
        float pointSpacing = squareSize / Mathf.Sqrt(numPoints);

        // generate the points
        for (int x = 0; x < Mathf.Sqrt(numPoints); x++)
        {
            for (int y = 0; y < Mathf.Sqrt(numPoints); y++)
            {
                // calculate the point position
                Vector3 pointPos = new Vector3(x * pointSpacing, 0, y * pointSpacing);

                // instantiate a new game object at the point position
                GameObject point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                point.transform.position = pointPos;
                point.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            }
        }
    }
}