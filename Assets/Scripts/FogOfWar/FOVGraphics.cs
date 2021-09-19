using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVGraphics : MonoBehaviour
{
    [SerializeField] Collider currentBush = null;

    [SerializeField] float viewRadius = 20f;
    [SerializeField] int degreePerCast = 10;
    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] LayerMask grassLayer;
    [SerializeField] float offset = 0.5f;
    [SerializeField] MeshFilter viewMeshFilter = null;

    Mesh viewMesh;
    int vertexCount;
    Vector3[] vertices;
    int[] triangles;

    RaycastHit[] hitBuffer = new RaycastHit[3];

    float updateInterval = 0.01f;
    bool isUpdating = false;

    float updateTimer;

    public void GenerateMesh(float viewRadius, int degreePerCast)
    {
        this.viewRadius = viewRadius;
        this.degreePerCast = degreePerCast;

        viewMesh = new Mesh();
        viewMesh.name = "ViewMesh";
        viewMeshFilter.mesh = viewMesh;

        List<Vector3> viewPoints = GetViewPoints();

        vertexCount = viewPoints.Count + 1;
        vertices = new Vector3[vertexCount];
        triangles = new int[(vertexCount - 1) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        triangles[triangles.Length - 3] = 0;
        triangles[triangles.Length - 2] = vertexCount - 1;
        triangles[triangles.Length - 1] = 1;


        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    public void StartUpdateMesh(float updateInterval)
    {
        this.updateInterval = updateInterval;
        this.isUpdating = true;
    }

    private void Update()
    {
        UpdateMesh();

        //if (!isUpdating){ return; }

        //if(updateTimer <= 0)
        //{
        //    UpdateMesh();
        //    updateTimer = updateInterval;
        //}

        //updateTimer -= Time.deltaTime;
    }

    private void UpdateMesh()
    {
        List<Vector3> viewPoints = GetViewPoints();

        vertices[0] = Vector3.zero;

        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    private List<Vector3> GetViewPoints()
    {
        List<Vector3> viewPoints = new List<Vector3>();

        for (int i = 0; i < 360; i += degreePerCast)
        {
            Vector3 direction = DirectionFromAngle(i);

            LayerMask checkLayer = obstacleLayer | grassLayer;

            int hitCount = Physics.RaycastNonAlloc(transform.position + Vector3.up * offset, direction, hitBuffer, viewRadius, checkLayer);

            Vector3 vertexPoint = transform.position + direction * viewRadius;
            float minDist = viewRadius;
            for (int j=0; j < hitCount; j++)
            {
                if( hitBuffer[j].collider == currentBush ) { continue; }

                float dist = Vector3.Distance(hitBuffer[j].point, transform.position);

                if(dist < minDist)
                {
                    vertexPoint = hitBuffer[j].point;
                    minDist = dist;
                }
            }
            viewPoints.Add(vertexPoint);
        }
        return viewPoints;
    }

    public Vector3 DirectionFromAngle(float angleInDegrees)
    {
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if ((1 << other.gameObject.layer & grassLayer) > 0)
        {
            currentBush = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == currentBush)
        {
            currentBush = null;
        }
    }
}
