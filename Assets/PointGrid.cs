using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointGrid : MonoBehaviour
{
    // Number of points in each dimension of the grid
    public int numPointsX = 20;
    public int numPointsY = 20;
    public float firstX = 0.0f;
    public float firstY = 0.0f;
    public float secondX = 5.0f;
    public float secondY = 5.0f;
    private int numLinesX;
    private int numLinesY;
    public float restLength = 0.05f;
    public float intensity = 8.0f;

    // Spacing between points
    private float pointSpacingX;
    private float pointSpacingY;

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

        pointSpacingX = (secondX - firstX) / (numPointsX - 1);
        pointSpacingY = (secondY - firstY) / (numPointsY - 1);

        numLinesX = numPointsX - 1;
        numLinesY = numPointsY - 1;

        // Create the point game objects and add them to the scene
        for (int y = 0; y < numPointsY; y++)
        {
            for (int x = 0; x < numPointsX; x++)
            {
                // Calculate the position of the point
                Vector3 pointPosition = new Vector3(firstX + (x * pointSpacingX), 0.0f, firstY + (y * pointSpacingY));

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
            for (int x = 0; x < numPointsY; x++) //Determine k value for all Horizontal lines
            {
                Vector3 lineCentre = new Vector3(firstX + (pointSpacingX / 2 + pointSpacingX * x), 0, firstY + y * pointSpacingY);
                linesHorizontalK[x,y] = kFromXY(lineCentre);
                //Debug.Log(linesHorizontalK[x,y]);
            }
        }

        for (int y = 0; y < numLinesY; y++)
        {
            for (int x = 0; x < numPointsX; x++) //Determine k value for all Vertical lines
            {
                Vector3 lineCentre = new Vector3(firstX + x * pointSpacingX, 0, firstY + (pointSpacingY / 2 + pointSpacingY * y));
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
                        // Debug.Log(k);
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
                // Debug.Log(totalForce);
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
        // Vector3 centre = new Vector3(4.5f, 0f, 4.5f);
        // Vector3 posXY = lineCentre - centre;
        // float magK = posXY.magnitude;
        // magK = 8 / ((magK + 1) * (magK + 1));
        // return magK;
        var fields = GameObject.FindGameObjectsWithTag("navifields");
        float k = 0.5f;
        float maxK = 2f;
        foreach (GameObject field in fields)
        {
            float DistX = lineCentre.x - field.transform.position.x;
            float DistZ = lineCentre.z - field.transform.position.z; 
            float Dist = Mathf.Sqrt(DistX*DistX + DistZ*DistZ);
            float InnerR = 0.5f * field.transform.localScale.x;
            float OuterR = 0.75f * field.transform.localScale.x;
            if (Dist >= OuterR){
                
            }
            else if (Dist <= InnerR){
                k = maxK;
                
            }
            else if(Dist < OuterR && Dist > InnerR){
                // float gradualK = (Dist - InnerR) * (1 / InnerR) * (maxK);
                // if (gradualK < k){
                //     k = gradualK;
                // }
                k = maxK;
            }
            
        }
        Debug.Log(k);

        return k;
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