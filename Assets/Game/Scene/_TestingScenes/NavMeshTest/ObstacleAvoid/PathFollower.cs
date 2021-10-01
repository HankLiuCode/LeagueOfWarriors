using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Dota.Utils;

public class PathFollower : MonoBehaviour
{
    [SerializeField] private Vector3[] wayPoints;
    [SerializeField] int nextIndex = -1;
    [SerializeField] bool reachedDest = true;
    [SerializeField] bool enableObstacleAvoid;
    [SerializeField] ObstacleAvoider obstacleAvoider = null;
    public const float ARRIVE_EPSILON = 0.1f;

    private Quaternion targetRotation;
    float turnSpeed = 0.1f;

    public bool ReachedDestination { get { return reachedDest; } }


    public void SetPath(Vector3[] wayPoints)
    {
        if(wayPoints.Length <= 1) { return; }

        this.wayPoints = wayPoints;

        reachedDest = false;

        nextIndex = 0;
    }

    public void Move(NavMeshAgent navMeshAgent, float speed)
    {
        if (reachedDest || !navMeshAgent.enabled) 
        { 
            return; 
        }

        float distance = VectorConvert.XZDistance(wayPoints[nextIndex], transform.position);

        if (distance < ARRIVE_EPSILON)
        {
            nextIndex++;

            if (nextIndex >= wayPoints.Length)
            {
                reachedDest = true;
                nextIndex = wayPoints.Length - 1;
            }
        }

        //targetRotation = Quaternion.LookRotation(wayPoints[nextIndex] - transform.position, Vector3.up);
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed);

        //navMeshAgent.Move(transform.forward * speed * Time.deltaTime);

        Vector3 direction = VectorConvert.XZDirection(transform.position, wayPoints[nextIndex]);
        transform.forward = direction;
        navMeshAgent.Move(direction * speed * Time.deltaTime);

        //Vector3 seekTarget = wayPoints[nextIndex];

        //if (enableObstacleAvoid)
        //{
        //    obstacleAvoider.Seek(navMeshAgent, speed, seekTarget);
        //}
        //else
        //{
        //    Vector3 direction = VectorConvert.XZDirection(transform.position, wayPoints[nextIndex]);
        //    transform.forward = direction;
        //    navMeshAgent.Move(direction * speed * Time.deltaTime);
        //}
    }

    public Vector3 NextWayPoint()
    {
        return wayPoints[nextIndex];
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        for (int i = 0; i < wayPoints.Length - 1; i++)
        {
            Gizmos.DrawCube(wayPoints[i], Vector3.one);
            Gizmos.DrawCube(wayPoints[i + 1], Vector3.one);
            Gizmos.DrawLine(wayPoints[i], wayPoints[i + 1]);
        }

        if(nextIndex < wayPoints.Length && nextIndex >= 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(wayPoints[nextIndex], Vector3.one);
        }
    }
}
