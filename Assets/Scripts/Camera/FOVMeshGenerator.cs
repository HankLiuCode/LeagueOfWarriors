using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVMeshGenerator : MonoBehaviour
{
    [SerializeField] GameObject temporaryMeshPrefab = null;
    [SerializeField] float radius = 20f;

    GameObject fovMesh = null;
    private void Awake()
    {
        fovMesh = Instantiate(temporaryMeshPrefab, transform, false);
        SetFOVRadius(radius);
    }

    public void SetFOVRadius(float radius)
    {
        fovMesh.transform.localScale = new Vector3(radius, 1, radius);
    }
}
