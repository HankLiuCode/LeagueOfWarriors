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
    [SerializeField] Obstacle obstacle = null;
    public float Speed { get { return speed; } set { speed = value; } }
    public bool CanMove { get; set; }
    public bool IsMoving { get { return (!pathFollower.ReachedDestination) && CanMove; } }

    // Cache
    NavMeshPath navMeshPath;

    private void Start()
    {
        navMeshPath = new NavMeshPath();
    }

    public void SetMask(int mask)
    {
        agent.areaMask = mask;
    }
    
    public void SetDestination(Vector3 targetPoint)
    {
        bool isOnNav = agent.isOnNavMesh;

        NavMesh.SamplePosition(targetPoint, out NavMeshHit pointOnNavMesh, 500f, NavMesh.AllAreas);

        agent.CalculatePath(pointOnNavMesh.position, navMeshPath);

        pathFollower.SetPath(navMeshPath.corners);
    }

    private void Update()
    {
        obstacle.IsEnabled = !IsMoving;

        if (CanMove)
        {
            pathFollower.Move(agent, Speed);
        }
    }
}
