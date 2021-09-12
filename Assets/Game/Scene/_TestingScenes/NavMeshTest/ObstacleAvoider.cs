using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoider : MonoBehaviour
{
    [SerializeField] float radius = 1f;
    [SerializeField] float obstacleRadius = 2f;
    [SerializeField] float probeLength = 2f;
    [SerializeField] List<Transform> obstacles;

    public List<Transform> GetAvoidTargets()
    {
        Vector3 probeVec = transform.forward * probeLength;

        List<Transform> avoidTargets = new List<Transform>();

        foreach (Transform obstacle in obstacles)
        {
            Vector3 obstacleVec = obstacle.position - transform.position;

            float probeCheckDist = probeVec.magnitude;

            if(probeCheckDist > probeLength + obstacleRadius)
            {
                continue;
            }

            Vector3 obstacleDir = obstacleVec.normalized;

            Vector3 probeDir = probeVec.normalized;

            float dot = Vector3.Dot(probeDir, obstacleDir);

            if (dot < 0) 
            { 
                continue; 
            }

            float obstacleDist = obstacleVec.magnitude;
            float obstacleProjectionOnProbeDist = obstacleVec.magnitude * dot;
            float obstacleToProjectionDist = (obstacleDist * obstacleDist - obstacleProjectionOnProbeDist * obstacleProjectionOnProbeDist);
            
            if (dot < radius + obstacleRadius)
            {
                avoidTargets.Add(obstacle);
            }
        }

        return avoidTargets;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach(Transform avoidTarget in GetAvoidTargets())
        {
            Gizmos.DrawWireCube(avoidTarget.position, Vector3.one);
        }

        Gizmos.color = Color.green;
        foreach (Transform obstacle in obstacles)
        {
            Gizmos.DrawWireSphere(obstacle.position, obstacleRadius);
        }

        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * probeLength);
    }
}
