using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ObstacleAvoider : MonoBehaviour
{
    public const float DEFAULT_MOVE_STRAIGHT_TIME = 0.2f;

    [SerializeField] float radius = 1f;
    [SerializeField] float obstacleRadius = 2f;
    [SerializeField] float probeLength = 2f;
    [SerializeField] List<Transform> obstacles;

    [SerializeField] Transform target;

    [SerializeField] float clearObstacleCacheInterval = 2f;

    [SerializeField] float moveStraightTime;
    Dictionary<Transform, int> obstacleCache = new Dictionary<Transform, int>();
    float moveStraightTimer;

    private void Start()
    {
        StartCoroutine(ClearObstacleCacheRoutine());
    }

    IEnumerator ClearObstacleCacheRoutine()
    {
        while (true)
        {
            bool isStuck = false;
            foreach(var oCache in obstacleCache)
            {
                if(oCache.Value >= 3)
                {
                    isStuck = true;
                }
            }
            obstacleCache.Clear();

            if (isStuck)
            {
                moveStraightTime = moveStraightTime + 0.1f;
            }
            else
            {
                moveStraightTime = DEFAULT_MOVE_STRAIGHT_TIME;
            }

            yield return new WaitForSeconds(clearObstacleCacheInterval);
        }
    }

    public void Move(NavMeshAgent navMeshAgent, float speed)
    {
        ObstacleInfo obstacleInfo = GetClosestObstacleInfo();

        if (obstacleInfo.hasObstacle)
        {
            Vector3 forward = transform.forward + obstacleInfo.avoidDirection * (obstacleRadius + radius);
            navMeshAgent.speed = speed;
            transform.forward = forward;
            navMeshAgent.Move(transform.forward * speed * Time.deltaTime);
            moveStraightTimer = moveStraightTime;

            if (obstacleCache.ContainsKey(obstacleInfo.obstacle))
            {
                obstacleCache[obstacleInfo.obstacle] += 1;
            }
            else
            {
                obstacleCache.Add(obstacleInfo.obstacle, 1);
            }
        }
        
        if(moveStraightTimer <= 0)
        {
            Vector3 direction = target.position - transform.position;
            Vector3 forward = new Vector3(direction.x, 0, direction.z);
            transform.forward = forward;
            navMeshAgent.speed = speed;
            navMeshAgent.Move(transform.forward * speed * Time.deltaTime);
        }
        else
        {
            moveStraightTimer -= Time.deltaTime;

            navMeshAgent.speed = speed;
            navMeshAgent.Move(transform.forward * speed * Time.deltaTime);
        }
    }


    public ObstacleInfo GetClosestObstacleInfo()
    {
        List<ObstacleInfo> avoidTargets = GetAvoidTargets();

        ObstacleInfo obstacleInfo = new ObstacleInfo();
        obstacleInfo.hasObstacle = false;

        float minDist = float.MaxValue;
        foreach (ObstacleInfo avoidTarget in avoidTargets)
        {
            if (!obstacleInfo.hasObstacle)
            {
                obstacleInfo.hasObstacle = true;
                obstacleInfo.obstacle = avoidTarget.obstacle;
                obstacleInfo.avoidDirection = avoidTarget.avoidDirection;
                minDist = avoidTarget.obstacleVec.magnitude;
            }

            if (avoidTarget.obstacleVec.magnitude < minDist)
            {
                obstacleInfo.hasObstacle = true;
                obstacleInfo.obstacle = avoidTarget.obstacle;
                obstacleInfo.avoidDirection = avoidTarget.avoidDirection;
                minDist = avoidTarget.obstacleVec.magnitude;
            }
        }
        return obstacleInfo;
    }

    public List<ObstacleInfo> GetAvoidTargets()
    {
        Vector3 probeVec = transform.forward * probeLength;

        List<ObstacleInfo> avoidObstacles = new List<ObstacleInfo>();

        List<Transform> avoidTargets = new List<Transform>();

        foreach (Transform obstacle in obstacles)
        {
            Vector3 obstacleVec = obstacle.position - transform.position;

            float obstacleDist = obstacleVec.magnitude;

            if(obstacleDist > probeLength + obstacleRadius)
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

            float obstacleProjectionOnProbeDist = obstacleVec.magnitude * dot;
            float obstacleToProjectionDist = (obstacleDist * obstacleDist - obstacleProjectionOnProbeDist * obstacleProjectionOnProbeDist);

            if (obstacleToProjectionDist < radius + obstacleRadius)
            {
                avoidTargets.Add(obstacle);

                Vector3 avoidDirection = (obstacleProjectionOnProbeDist * transform.forward - obstacleDir);

                ObstacleInfo obstacleInfo = new ObstacleInfo();
                obstacleInfo.hasObstacle = true;
                obstacleInfo.obstacle = obstacle;
                obstacleInfo.obstacleVec = obstacleVec;
                obstacleInfo.avoidDirection = avoidDirection;
                avoidObstacles.Add(obstacleInfo);
            }
        }
        return avoidObstacles;
    }
    
    public struct ObstacleInfo
    {
        public bool hasObstacle;
        public Transform obstacle;
        public Vector3 obstacleVec;
        public Vector3 avoidDirection;
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach(ObstacleInfo avoidTarget in GetAvoidTargets())
        {
            Gizmos.DrawWireCube(avoidTarget.obstacle.position, Vector3.one);
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
