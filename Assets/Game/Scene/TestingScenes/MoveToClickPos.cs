using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum MouseButton
{
    Right = 0,
    Left = 1,
    Middle = 2
}

public class MoveToClickPos : MonoBehaviour
{
    public MouseButton mouseButton;
    [SerializeField] NavMeshAgent agent = null;
    
    void Update()
    {
        if(Input.GetMouseButtonDown((int)mouseButton))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                agent.SetDestination(hit.point);
            }
        }
    }
}
