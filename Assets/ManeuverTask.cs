using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManeuverTask : MonoBehaviour
{
    public GameObject spherePrefab;
    public Transform HeadSet;
    private Vector3[] points = new Vector3[6];
    private GameObject sphere;

    public Color color = Color.white;
    public float width = 0.2f;
    private LineRenderer lineRenderer;

    int index;

    void Start()
    {
        //Create line renderer to go from point A to point B
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, new Vector3(0, 0, 0));


        Debug.Log("Initialized ManeuverTask");
        //Create the 6 points
        points[0] = new Vector3(4, 0, 0);
        points[1] = new Vector3(2, 0, 3.464102f);
        points[2] = new Vector3(-2, 0, 3.464102f);
        points[3] = new Vector3(-4, 0, 0);
        points[4] = new Vector3(-2, 0, -3.464102f);
        points[5] = new Vector3(2, 0, -3.464102f);

        //Spawn a sphere on a random point
        int randomIndex = Random.Range(0, points.Length);
        index = randomIndex;
        sphere = Instantiate(spherePrefab, points[randomIndex], Quaternion.identity);
        lineRenderer.SetPosition(1, points[randomIndex]);
    }

    void Update()
    {
        float Hx = HeadSet.position.x;
        float Hz = HeadSet.position.z;
        //Debug.Log(index);
        float DistX = Hx - points[index].x;
        float DistZ = Hz - points[index].z; 
        float Dist = Mathf.Sqrt(DistX*DistX + DistZ*DistZ);
        
        if (Dist < 0.865)
        {
            lineRenderer.SetPosition(0, points[index]);
            Destroy(sphere);
            int randomIndex = Random.Range(0, points.Length);
            if (randomIndex != index)
            {
                index = randomIndex;
                sphere = Instantiate(spherePrefab, points[randomIndex], Quaternion.identity);
                lineRenderer.SetPosition(1, points[index]);
            }

        }
    }

}