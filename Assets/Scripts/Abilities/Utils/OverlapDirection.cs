using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlapDirection : MonoBehaviour
{
    [SerializeField] SimpleOverlapBox overlapBox = null;
    float angle = 40f;
    public bool doTransform = false;



    private void Update()
    {
        if (doTransform)
        {
            overlapBox.rotation = Quaternion.Euler(0, angle, 0) * overlapBox.rotation * Quaternion.Inverse(Quaternion.Euler(0, angle, 0));
            Vector3 direction = overlapBox.center - transform.position;
            direction = Quaternion.Euler(0, angle, 0) * direction;
            overlapBox.center = transform.position + direction;
            doTransform = false;
        }
    }



    private void OnDrawGizmos()
    {
        
    }
}
