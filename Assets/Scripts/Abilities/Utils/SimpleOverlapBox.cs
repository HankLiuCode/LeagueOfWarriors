using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleOverlapBox : MonoBehaviour
{
    public Vector3 center;
    public Vector3 size = Vector3.one;
    public Quaternion rotation;

    bool isColliding = false;

    void Update()
    {

        Collider[] colliders = Physics.OverlapBox(center, size / 2, rotation);
        isColliding = false;
        foreach (Collider c in colliders)
        {
            isColliding = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isColliding ? Color.red : Color.green;

        Gizmos.matrix = Matrix4x4.Rotate(rotation);
        Gizmos.DrawWireCube(center, size);
    }
}
