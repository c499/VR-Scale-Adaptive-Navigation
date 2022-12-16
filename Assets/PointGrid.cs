using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointGrid : MonoBehaviour
{
    // Number of points in each dimension of the grid
    public int numPointsX = 10;
    public int numPointsY = 10;
    private int numLinesX;
    private int numLinesY;
    public float restLength = 0.1f;
    public float intensity = 1.0f;

    // Spacing between points
    public float pointSpacing = 0.2f;

    // Prefab for the point game objects
    public GameObject pointPrefab;

    // Array to store the points
    GameObject[,] points;
    float[,] linesHorizontalK;
    float[,] linesVerticalK;

    void Start()
    {
        // Initialize the points array
        points = new GameObject[numPointsX, numPointsY];
        linesHorizontalK = new float [numPointsX, numPointsY];
        linesVerticalK = new float [numPointsX, numPointsY];

        numLinesX = numPointsX - 1;
        numLinesY = numPointsY - 1;

        // Create the point game objects and add them to the scene
        for (int y = 0; y < numPointsY; y++)
        {
            for (int x = 0; x < numPointsX; x++)
            {
                // Calculate the position of the point
                Vector3 pointPosition = new Vector3(x * pointSpacing, 0.0f, y * pointSpacing);

                // Create the point game object and add it to the scene
                GameObject point = Instantiate(pointPrefab, pointPosition, Quaternion.identity);
                point.name = "Point " + x + "," + y;
                point.transform.parent = transform;

                // Add the point to the points array
                points[x, y] = point;
            }
        }

        for (int y = 0; y < numLinesX; y++)
        {
            for (int x = 0; x < numPointsY; x++) //All Horizontal lines
            {
                Vector3 lineCentre = new Vector3((pointSpacing / 2 + pointSpacing * x), 0, y);
                linesHorizontalK[x,y] = kFromXY(lineCentre);
                //Debug.Log(linesHorizontalK[x,y]);
            }
        }

        for (int y = 0; y < numLinesY; y++)
        {
            for (int x = 0; x < numPointsX; x++) //All Vertical lines
            {
                Vector3 lineCentre = new Vector3(x, 0, (pointSpacing / 2 + pointSpacing * y));
                linesVerticalK[x,y] = kFromXY(lineCentre);
            }
        }

    }

    void Update() 
    {
        //GetPoint(3,4).transform.position += new Vector3(0.1f,0f,0f);
        for (int y = 1; y < (numPointsY - 1); y++)
        {
            for (int x = 1; x < (numPointsX - 1); x++) //Runs for every point that isn't at the edge
            {
                //GetPoint(x,y).transform.position += new Vector3(0f,0.01f,0f);
                Vector3 self = GetPoint(x,y).transform.position;
                Vector3 up = GetPoint(x,y+1).transform.position;
                Vector3 right = GetPoint(x+1,y).transform.position;
                Vector3 down = GetPoint(x,y-1).transform.position;
                Vector3 left = GetPoint(x-1,y).transform.position;
                float k = 0.01f;
                Vector3[] dirSum = {up, right, down, left};
                Vector3 totalForce = new Vector3(0.0f,0.0f,0.0f);
                foreach (Vector3 dir in dirSum)
                {
                    if (dir == up){
                        k = linesVerticalK[x,y];
                        Debug.Log(k);
                    }
                    else if (dir == right){
                        k = linesHorizontalK[x,y];
                    }
                    else if (dir == down){
                        k = linesVerticalK[x,y-1];
                    }
                    else if (dir == left){
                        k = linesHorizontalK[x-1, y];
                    }
                    totalForce += VectorForce(self, dir, k);
                }
                Debug.Log(totalForce);
                GetPoint(x,y).transform.position += totalForce;
                
            }
        }
        for (int y = 0; y < (numPointsY); y++)
        {
            for (int x = 0; x < (numPointsX); x++) //draws the lines between each dot to show where the springs are
            {
                if (x < (numPointsX-1)){
                Debug.DrawLine(GetPoint(x,y).transform.position,GetPoint(x+1,y).transform.position,Color.white, 0f);
                }
                if (y < (numPointsY-1)){
                Debug.DrawLine(GetPoint(x,y).transform.position,GetPoint(x,y+1).transform.position,Color.white, 0f);
                }
                // if ((y < (numPointsY-1)) && (x < (numPointsX-1))){
                // Debug.DrawLine(GetPoint(x,y).transform.position,GetPoint(x+1,y+1).transform.position,Color.white, 0f);
                // }
            }
        }
    }

    // Function to retrieve a point at a specific index in the grid
    public GameObject GetPoint(int x, int y)
    {
        return points[x, y];
    }

    private float kFromXY(Vector3 lineCentre)
    {
        Vector3 centre = new Vector3(4.5f, 0f, 4.5f);
        Vector3 posXY = lineCentre - centre;
        float magK = posXY.magnitude;
        magK = 8 / ((magK + 1) * (magK + 1));
        return magK;
    }

    public Vector3 VectorForce(Vector3 original, Vector3 compared, float k) //Hooke's law is applied, the intensity variable serves a similar function as changing mass, but inversed
    {
        Vector3 difference = original - compared;
        Vector3 vecDirection = difference.normalized;
        float mag = difference.magnitude;
        float x = mag - restLength;
        float force = -k * x * intensity;
        Vector3 applyForce = force * vecDirection;
        return applyForce;
    }
}