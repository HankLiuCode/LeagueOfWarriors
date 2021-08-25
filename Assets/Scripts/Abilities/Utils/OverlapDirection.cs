using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlapDirection : MonoBehaviour
{
    [SerializeField] RectIndicator rectIndicator = null;

    [SerializeField] Vector3 testEuler;

    void Update()
    {
        OverlapWithDirection(rectIndicator.GetDirection(), rectIndicator.GetLength(), rectIndicator.GetWidth());
    }

    public void OverlapWithDirection(Vector3 direction, float length, float width)
    {
        float centerY = 0.5f;
        Vector3 center = rectIndicator.transform.position + direction * (length / 2) + Vector3.up * 0.5f;
        Vector3 halfExtent = new Vector3(width / 2, centerY, length / 2);

        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

        Collider[] colliders = Physics.OverlapBox(center, halfExtent, rotation);
        foreach(Collider c in colliders)
        {
            Debug.Log(c.name);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(rectIndicator.transform.position, Vector3.one / 5);

        Gizmos.DrawLine(rectIndicator.transform.position, rectIndicator.transform.position + rectIndicator.GetDirection() * rectIndicator.GetLength());



        //Gizmos.color = Color.green;
        //Vector3 direction = rectIndicator.GetDirection();
        //float length = rectIndicator.GetLength();
        //float width = rectIndicator.GetWidth();
        //Vector3 center = rectIndicator.transform.position + direction * (length / 2) + Vector3.up * 0.5f;

        //Matrix4x4 cubeTransform = Matrix4x4.TRS(Vector3.zero, Quaternion.LookRotation(direction, Vector3.up) , Vector3.one);
        //Matrix4x4 oldGizmosMatrix = Gizmos.matrix;

        //Gizmos.matrix *= cubeTransform;

        //Gizmos.DrawCube(center, new Vector3(width, 1, length));

        //Gizmos.matrix = oldGizmosMatrix;

        // ---------------------------------------------------------------



        //Gizmos.color = Color.green;

        //Quaternion rotation = Quaternion.Euler(0, rot, 0);

        //Matrix4x4 cubeTransform = transform.localToWorldMatrix;

        //Matrix4x4 oldGizmosMatrix = Gizmos.matrix;

        //Gizmos.matrix *= cubeTransform;

        //Gizmos.DrawCube(Vector3.zero, new Vector3(1, 1, 1));

        //Gizmos.matrix = oldGizmosMatrix;
    }
}
