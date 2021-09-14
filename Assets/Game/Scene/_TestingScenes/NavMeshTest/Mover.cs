using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mover : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent = null;
    [SerializeField] float speed = 5f;
    [SerializeField] PathFollower pathFollower = null;
    [SerializeField] ObstacleAvoider obstacleAvoider = null;
    [SerializeField] Vector3 targetPoint;
    [SerializeField] Transform target = null;

    // Cache
    NavMeshPath navMeshPath;

    private void Start()
    {
        navMeshPath = new NavMeshPath();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                targetPoint = hit.point;
                agent.CalculatePath(targetPoint, navMeshPath);
                pathFollower.SetPath(navMeshPath.corners);
            }
        }
        pathFollower.Move(agent, 5f);
    }
}
