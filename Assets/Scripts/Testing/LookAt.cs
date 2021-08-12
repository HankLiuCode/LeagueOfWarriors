using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    [SerializeField] Transform target;

    // Update is called once per frame
    void Update()
    {
        Vector3 targetDir = target.position - transform.position;
        

        transform.LookAt(transform.position + Quaternion.AngleAxis(90, Vector3.up) * targetDir);
    }
}
