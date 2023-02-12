using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class PointGrid : MonoBehaviour
{

    // Number of points in each dimension of the grid
    public int numPointsX = 20;
    public int numPointsY = 20;
    public float firstX = 0.0f;
    public float firstY = 0.0f;
    public float secondX = 5.0f;
    public float secondY = 5.0f;
    public float minK = 0.5f;
    public float maxK = 2f;
    public float fieldR = 0.75f;
    public float innerFieldR = 0.5f;
    private int numLinesX;
    private int numLinesY;
    public float restLength = 0.05f;
    public float intensity = 8.0f;
    public static int frameCount = 0;
    public int framesToSimulate = 100;

    // Variables for the playable space grid and scaling
    public XRNode HeadPos;
    public Transform HeadSet; //Assign Main Camera
    public Transform RigPos; //Assign XRRig
    public float playableFirstX = 0.0f;
    public float playableFirstY = 0.0f;
    public float playableSecondX = 5.0f;
    public float playableSecondY = 5.0f;

    // Spacing between points
    private float pointSpacingX;
    private float pointSpacingY;

    // Prefab for the point game objects
    public GameObject pointPrefab;

    // Array to store the points
    GameObject[,] points;
    public Vector3[,] pointsVector;
    private bool hasConvertedPoints = false;
    float[,] linesHorizontalK;
    float[,] linesVerticalK;

    public static PointGrid Instance;

    void Start()
    {
        // Initialize the points arrays
        points = new GameObject[numPointsX, numPointsY];
        pointsVector = new Vector3[numPointsX, numPointsY];
        linesHorizontalK = new float [numPointsX, numPointsY];
        linesVerticalK = new float [numPointsX, numPointsY];

        pointSpacingX = (secondX - firstX) / (numPointsX - 1);
        pointSpacingY = (secondY - firstY) / (numPointsY - 1);

        numLinesX = numPointsX - 1;
        numLinesY = numPointsY - 1;

        Debug.DrawLine(new Vector3(4f, 0.01f, 0f), new Vector3(4f + (playableSecondX - playableFirstX) / (numPointsX - 1), 0.01f, 0f), Color.yellow, 500f);
        Debug.DrawLine(new Vector3(4f, 0.01f, 0f), new Vector3(4f, 0.01f, (playableSecondY - playableFirstY) / (numPointsY - 1)), Color.yellow, 500f);
        Debug.DrawLine(new Vector3(4f + (playableSecondX - playableFirstX) / (numPointsX - 1), 0.01f, (playableSecondY - playableFirstY) / (numPointsY - 1)), new Vector3(4f, 0.01f, (playableSecondY - playableFirstY) / (numPointsY - 1)), Color.yellow, 500f);
        Debug.DrawLine(new Vector3(4f + (playableSecondX - playableFirstX) / (numPointsX - 1), 0.01f, (playableSecondY - playableFirstY) / (numPointsY - 1)), new Vector3(4f + (playableSecondX - playableFirstX) / (numPointsX - 1), 0.01f, 0f), Color.yellow, 500f);



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

        for (int y = 0; y < (numPointsY); y++) //COLORED DebugLines for ORIGINAL GRID
            {
                for (int x = 0; x < (numPointsX); x++) //draws the lines between each dot to show where the springs are
                    {
                    //if (x < (numPointsX-1)){
                    //Debug.DrawLine(GetPoint(x,y).transform.position + new Vector3(0f,0.01f,0f),GetPoint(x+1,y).transform.position + new Vector3(0f,0.01f,0f),Color.red, 1f);
                    //}
                    //if (y < (numPointsY-1)){
                    //Debug.DrawLine(GetPoint(x,y).transform.position + new Vector3(0f,0.01f,0f),GetPoint(x,y+1).transform.position + new Vector3(0f,0.01f,0f),Color.red, 1f);
                    //}
                    if (x < (numPointsX - 1))
                    {
                        Debug.DrawLine(GetPoint(x, y).transform.position/((firstX-secondX)/(playableFirstX-playableSecondX)) + new Vector3(0f, 0.01f, 0f), GetPoint(x + 1, y).transform.position/ ((firstX - secondX) / (playableFirstX - playableSecondX)) + new Vector3(0f, 0.01f, 0f), Color.red, 500f);
                    }
                    if (y < (numPointsY - 1))
                    {
                        Debug.DrawLine(GetPoint(x, y).transform.position / ((firstY - secondY) / (playableFirstY - playableSecondY)) + new Vector3(0f, 0.01f, 0f), GetPoint(x, y + 1).transform.position / ((firstY - secondY) / (playableFirstY - playableSecondY)) + new Vector3(0f, 0.01f, 0f), Color.red, 500f);
                    }
                    // if ((y < (numPointsY-1)) && (x < (numPointsX-1))){
                    // Debug.DrawLine(GetPoint(x,y).transform.position,GetPoint(x+1,y+1).transform.position,Color.white, 0f);
                    // }
                }
            }

    }

    void Update() 
    {
        if (frameCount == framesToSimulate)
        {
            if (hasConvertedPoints == false)
            {
                for (int y = 0; y < numPointsY; y++)
                {
                    for (int x = 0; x < numPointsX; x++)
                    {
                        pointsVector[x, y] = GetPoint(x, y).transform.position;
                        Destroy(points[x, y]);
                    }
                }
                for (int y = 0; y < (numPointsY); y++) //DebugLines for SPRING SIMULATION
                {
                    for (int x = 0; x < (numPointsX); x++) //draws the lines between each dot to show where the springs are
                    {
                        if (x < (numPointsX - 1))
                        {
                            Debug.DrawLine(GetPointVector(x, y), GetPointVector(x + 1, y), Color.white, 500f);
                        }
                        if (y < (numPointsY - 1))
                        {
                            Debug.DrawLine(GetPointVector(x, y), GetPointVector(x, y + 1), Color.white, 500f);
                        }
                        if ((y < (numPointsY - 1)) && (x < (numPointsX - 1)))
                        {
                            Debug.DrawLine(GetPointVector(x, y), GetPointVector(x + 1, y + 1), new Vector4(0.1f, 0.1f, 0.1f, 1), 500f);
                        }
                    }
                }
                hasConvertedPoints = true;
                Debug.Log("Points have been converted");
            }

            //Scaling variables:

            float hX = HeadSet.position.x - RigPos.transform.position.x; 
            float hY = RigPos.transform.position.y;
            float hZ = HeadSet.position.z - RigPos.transform.position.z;

            float playableSpacingX = (playableSecondX - playableFirstX) / (numPointsX - 1);
            float playableSpacingY = (playableSecondY - playableFirstY) / (numPointsY - 1);
        
            //Calculate index for position of headset
            if (hX > playableFirstX && hX < playableSecondX && hZ > playableFirstY && hZ < playableSecondY){
                float relativeX = (hX - playableFirstX) / playableSpacingX;
                int indexX = (int) Mathf.Round(relativeX - 0.5f);
                //Debug.Log(indexX);
                float relativeY = (hZ - playableFirstY) / playableSpacingY;
                int indexY = (int) Mathf.Round(relativeY - 0.5f);

                bool whichTriangle = false; // splitting the square into two triangles to calculate position translation.
                if ((relativeX - indexX) > (relativeY - indexY))
                {
                    whichTriangle = true;
                }
                else
                {
                    whichTriangle = false;
                }
                //Debug.Log(indexX + " ___ " + indexY + " " + whichTriangle);

                //Matrix equation used: https://stackoverflow.com/questions/18844000/transfer-coordinates-from-one-triangle-to-another-triangle

                //    | xa1 xa2 xa3 |
                // A =| ya1 ya2 ya3 |
                //    |  1   1   1  |

                //    | xb1 xb2 xb3 |
                // B =| yb1 yb2 yb3 |
                //    |  1   1   1  |

                //     M is the transformation matrix
                //     M * A = B
                //     M * A * Inv(A) = B * Inv(A)
                //     M = B * Inv(A)

                // Multiply M by the column matrix of the point to get 

                float xa1 = playableFirstX + (playableSpacingX * indexX);
                float xa2;
                float xa3 = playableFirstX + (playableSpacingX * (indexX + 1));

                float ya1 = playableFirstY + (playableSpacingY * indexY);
                float ya2;
                float ya3 = playableFirstY + (playableSpacingY * (indexY + 1));
                
                float xb1 = GetPointVector(indexX,indexY).x;
                float xb2;
                float xb3 = GetPointVector((indexX + 1),(indexY + 1)).x;

                float yb1 = GetPointVector(indexX,indexY).z;
                float yb2;
                float yb3 = GetPointVector((indexX + 1),(indexY + 1)).z;

                if (whichTriangle == false)
                {
                    xa2 = playableFirstX + (playableSpacingX * indexX);
                    ya2 = playableFirstY + (playableSpacingY * (indexY + 1));
                    xb2 = GetPointVector(indexX,(indexY + 1)).x;
                    yb2 = GetPointVector(indexX,(indexY + 1)).z;

                }
                else
                {
                    xa2 = playableFirstX + (playableSpacingX * (indexX + 1));
                    ya2 = playableFirstY + (playableSpacingY * indexY);
                    xb2 = GetPointVector((indexX + 1),indexY).x;
                    yb2 = GetPointVector((indexX + 1),indexY).z;

                }

                Matrix4x4 matrixA = new Matrix4x4();
                Matrix4x4 matrixB = new Matrix4x4();

                matrixA.SetRow(0, new Vector4(xa1, xa2, xa3, 0));
                matrixA.SetRow(1, new Vector4(ya1, ya2, ya3, 0));
                matrixA.SetRow(2, new Vector4(1, 1, 1, 0));
                matrixA.SetRow(3, new Vector4(0, 0, 0, 1));
                //Debug.Log(matrixA);

                matrixB.SetRow(0, new Vector4(xb1, xb2, xb3, 0));
                matrixB.SetRow(1, new Vector4(yb1, yb2, yb3, 0));
                matrixB.SetRow(2, new Vector4(1, 1, 1, 0));
                matrixB.SetRow(3, new Vector4(0, 0, 0, 1));

                Matrix4x4 matrixInverseA = matrixA.inverse;
                Matrix4x4 matrixM = matrixB * matrixInverseA;

                Vector4 sourcePosVector = new Vector4(hX, hZ, 1, 0);
                Vector4 NewPosVector = matrixM * sourcePosVector;
                //Debug.Log(NewPosVector);

                RigPos.transform.position = new Vector4(NewPosVector.x - hX, hY, NewPosVector.y - hZ);
                //Debug.Log(new Vector4(NewPosVector.x - hX, hY, NewPosVector.y - hZ));
            }

            


        }
        if (frameCount < framesToSimulate)
        {
            frameCount += 1;
        
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
            for (int y = 0; y < (numPointsY); y++) //DebugLines for SPRING SIMULATION
            {
                for (int x = 0; x < (numPointsX); x++) //draws the lines between each dot to show where the springs are
                {
                    if (x < (numPointsX - 1))
                    {
                        Debug.DrawLine(GetPoint(x, y).transform.position, GetPoint(x + 1, y).transform.position, Color.white, 0f);
                    }
                    if (y < (numPointsY - 1))
                    {
                        Debug.DrawLine(GetPoint(x, y).transform.position, GetPoint(x, y + 1).transform.position, Color.white, 0f);
                    }
                    if ((y < (numPointsY - 1)) && (x < (numPointsX - 1)))
                    {
                        Debug.DrawLine(GetPoint(x, y).transform.position, GetPoint(x + 1, y + 1).transform.position, new Vector4(0.1f, 0.1f, 0.1f, 1), 0f);
                    }
                }
            }
        }

    }

    // Function to retrieve a point at a specific index in the grid
    public GameObject GetPoint(int x, int y)
    {
        return points[x, y];
    }

    public Vector3 GetPointVector(int x, int y)
    {
        return pointsVector[x, y];
    }

    private float kFromXY(Vector3 lineCentre)
    {
        // Vector3 centre = new Vector3(4.5f, 0f, 4.5f);
        // Vector3 posXY = lineCentre - centre;
        // float magK = posXY.magnitude;
        // magK = 8 / ((magK + 1) * (magK + 1));
        // return magK;
        var fields = GameObject.FindGameObjectsWithTag("navifields");
        float k = minK;
        foreach (GameObject field in fields)
        {
            float DistX = lineCentre.x - field.transform.position.x;
            float DistZ = lineCentre.z - field.transform.position.z; 
            float Dist = Mathf.Sqrt(DistX*DistX + DistZ*DistZ);
            float InnerR = innerFieldR * field.transform.localScale.x;
            float OuterR = fieldR * field.transform.localScale.x;
            if (Dist >= OuterR){
                
            }
            else if (Dist <= InnerR){
                k = maxK;
                
            }
            else if(Dist < OuterR && Dist > InnerR){ //can be used to gradually decrease k value when leaving field
                float gradualK = (((OuterR - InnerR) - (Dist - InnerR)) / (OuterR - InnerR)) * (maxK - 0.5f) + 0.5f;
                if (gradualK > k){
                    k = gradualK;
                    // Debug.Log(k);
                }
                // k = maxK;
            }
            
        }


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