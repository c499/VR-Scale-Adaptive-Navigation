using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordinateTransfer : MonoBehaviour
{
    // Vertices of the source triangle
    public Vector3[] sourceVertices = new Vector3[3];

    // Vertices of the target triangle
    public Vector3[] targetVertices = new Vector3[3];

    // The point in the source triangle
    public Vector3 pointInSourceTriangle;

    // The transformed point in the target triangle
    private Vector3 pointInTargetTriangle;

    // The transformation matrix
    private Matrix4x4 transformationMatrix;

    void Start()
    {
        // Calculate the transformation matrix
        transformationMatrix = CalculateTransformationMatrix(sourceVertices, targetVertices);

        // Transfer the point to the target triangle
        pointInTargetTriangle = TransferPoint(pointInSourceTriangle, transformationMatrix);

        Debug.Log(pointInTargetTriangle);
    }

    Matrix4x4 CalculateTransformationMatrix(Vector3[] sourceVertices, Vector3[] targetVertices)
    {
        // Calculate the centroid of the source triangle
        Vector3 sourceCentroid = new Vector3();
        for (int i = 0; i < 3; i++)
        {
            sourceCentroid += sourceVertices[i];
        }
        sourceCentroid /= 3;

        // Calculate the centroid of the target triangle
        Vector3 targetCentroid = new Vector3();
        for (int i = 0; i < 3; i++)
        {
            targetCentroid += targetVertices[i];
        }
        targetCentroid /= 3;

        // Calculate the rotation matrix
        Matrix4x4 rotationMatrix = new Matrix4x4();
        for (int i = 0; i < 3; i++)
        {
            Vector3 sourceVector = sourceVertices[i] - sourceCentroid;
            Vector3 targetVector = targetVertices[i] - targetCentroid;
            for (int j = 0; j < 3; j++)
            {
                rotationMatrix[i, j] = Vector3.Dot(sourceVector, targetVector);
            }
        }

        // Calculate the translation vector
        Vector3 translationVector = targetCentroid - sourceCentroid;

        // Assemble the transformation matrix
        Matrix4x4 transformationMatrix = new Matrix4x4();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                transformationMatrix[i, j] = rotationMatrix[i, j];
            }
            transformationMatrix[i, 3] = translationVector[i];
        }
        transformationMatrix[3, 3] = 1;

        return transformationMatrix;
    }

    Vector3 TransferPoint(Vector3 point, Matrix4x4 transformationMatrix)
    {
        Vector4 pointInHomogeneousCoordinates = new Vector4(point.x, point.y, point.z, 1);
        Vector4 transformedPointInHomogeneousCoordinates = transformationMatrix * pointInHomogeneousCoordinates;
        return new Vector3(transformedPointInHomogeneousCoordinates.x, transformedPointInHomogeneousCoordinates.y, transformedPointInHomogeneousCoordinates.z);
    }
}