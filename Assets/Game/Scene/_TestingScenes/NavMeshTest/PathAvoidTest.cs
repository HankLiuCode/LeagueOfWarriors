using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathAvoidTest : MonoBehaviour
{
    [SerializeField] DotaAgent agent = null;

    void Start()
    {
        
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                agent.SetDestination(hit.point);
                agent.CanMove = true;
            }
        }
    }
}
