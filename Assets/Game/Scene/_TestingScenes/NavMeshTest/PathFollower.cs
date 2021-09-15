using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Dota.Utils;

[RequireComponent(typeof(ObstacleAvoider))]
public class PathFollower : MonoBehaviour
{
    [SerializeField] private Vector3[] wayPoints;
    [SerializeField] int nextIndex = -1;
    [SerializeField] bool reachedDest = true;
    [SerializeField] bool enableObstacleAvoid;
    [SerializeField] ObstacleAvoider obstacleAvoider = null;
    public const float ARRIVE_EPSILON = 0.1f;

    public bool ReachedDestination { get { return reachedDest; } }

    public void SetPath(Vector3[] wayPoints)
    {
        if(wayPoints.Length <= 1) { return; }

        this.wayPoints = wayPoints;

        reachedDest = false;

        nextIndex = 0;

        //StartCoroutine(SetWayPointAfterTurn(wayPoints));
    }

    IEnumerator SetWayPointAfterTurn(Vector3[] wayPoints)
    {
        Vector3 targetForward = (wayPoints[1] - transform.position).normalized;

        while(true)
        {
            if(Vector3.Distance(transform.forward, targetForward) > ARRIVE_EPSILON)
            {
                transform.forward = Vector3.MoveTowards(transform.forward, targetForward, 0.2f);
                yield return null;
            }
            else
            {
                this.wayPoints = wayPoints;

                reachedDest = false;

                nextIndex = 0;

                break;
            }
        }

        yield return null;
    }

    public void Move(NavMeshAgent navMeshAgent, float speed)
    {
        if (reachedDest) 
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
