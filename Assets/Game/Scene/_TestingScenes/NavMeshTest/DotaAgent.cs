using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PathFollower))]
public class DotaAgent : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent = null;
    [SerializeField] float speed = 5f;
    [SerializeField] PathFollower pathFollower = null;

    public bool IsStopped { get; set; }
    public float Speed { get { return speed; } set { speed = value; } }
    // Cache
    NavMeshPath navMeshPath;

    private void Start()
    {
        navMeshPath = new NavMeshPath();
    }

    public void SetDestination(Vector3 targetPoint)
    {
        IsStopped = false;
        bool isOnNav = agent.isOnNavMesh;
        agent.CalculatePath(targetPoint, navMeshPath);
        pathFollower.SetPath(navMeshPath.corners);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                SetDestination(hit.point);
            }
        }
        
        if (!IsStopped)
        {
            Debug.Log("Following Path");
            pathFollower.Move(agent, Speed);
        }
    }
}
