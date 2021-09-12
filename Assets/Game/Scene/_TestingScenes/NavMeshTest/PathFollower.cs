using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathFollower : MonoBehaviour
{
    [SerializeField] private Vector3[] wayPoints;
    [SerializeField] int nextIndex = -1;
    [SerializeField] bool reachedDest = true;
    public const float ARRIVE_EPSILON = 0.1f;


    public void SetPath(Vector3[] wayPoints)
    {
        this.wayPoints = wayPoints;
        reachedDest = false;
        nextIndex = 0;
    }

    public void Move(NavMeshAgent navMeshAgent, float speed)
    {
        if (reachedDest) { return; }

        Vector3 direction = Vec2Direction(transform.position, wayPoints[nextIndex]);
        float distance = Vec2Distance(wayPoints[nextIndex], transform.position);

        Debug.Log(distance);

        if (distance < ARRIVE_EPSILON)
        {
            nextIndex++;

            if (nextIndex >= wayPoints.Length)
            {
                reachedDest = true;
                nextIndex = wayPoints.Length - 1;
            }
        }

        navMeshAgent.Move(direction * speed * Time.deltaTime);
    }

    private Vector3 Vec2Direction(Vector3 pos1, Vector3 pos2)
    {
        Vector3 pos12D = new Vector3(pos1.x, 0, pos1.z);
        Vector3 pos22D = new Vector3(pos2.x, 0, pos2.z);
        return (pos22D - pos12D).normalized;
    }

    private float Vec2Distance(Vector3 pos1, Vector3 pos2)
    {
        Vector3 pos12D = new Vector3(pos1.x, 0, pos1.z);
        Vector3 pos22D = new Vector3(pos2.x, 0, pos2.z);
        return Vector3.Distance(pos12D, pos22D);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        for (int i = 0; i < wayPoints.Length - 1; i++)
        {
            Gizmos.DrawCube(wayPoints[i], Vector3.one);
            Gizmos.DrawCube(wayPoints[i + 1], Vector3.one);
            Gizmos.DrawLine(wayPoints[i], wayPoints[i + 1]);
        }
    }
}
