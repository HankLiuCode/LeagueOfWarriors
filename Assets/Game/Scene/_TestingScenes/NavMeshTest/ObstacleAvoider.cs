using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ObstacleAvoider : MonoBehaviour
{
    public const float DEFAULT_MOVE_STRAIGHT_TIME = 0.2f;

    [SerializeField] float radius = 1f;
    [SerializeField] float probeLength = 2f;

    [SerializeField] Vector3 target;
    [SerializeField] float moveStraightTime;
    [SerializeField] float maxTurnSpeed = 0.2f;

    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] float obstacleNoticeRadius = 5f;

    [SerializeField] List<Obstacle> obstacles;

    List<Obstacle> toDelete = new List<Obstacle>();
    
    float moveStraightTimer;
    
    public void SetTarget(Vector3 target)
    {
        this.target = target;
    }

    public bool ObstacleAvoid(NavMeshAgent navMeshAgent, float speed)
    {
        ObstacleInfo obstacleInfo = GetClosestObstacleInfo();

        if (obstacleInfo.hasObstacle)
        {
            float obstacleRadius = obstacleInfo.obstacle.GetRadius();
            Vector3 forward = transform.forward + obstacleInfo.avoidDirection * (obstacleRadius + radius);
            forward = new Vector3(forward.x, 0, forward.z);
            forward.Normalize();

            transform.forward = Vector3.MoveTowards(transform.forward, forward, maxTurnSpeed);

            navMeshAgent.speed = speed;
            navMeshAgent.Move(transform.forward * speed * Time.deltaTime);
            moveStraightTimer = moveStraightTime;

            return true;
        }
        else
        {
            if (moveStraightTimer <= 0)
            {
                Vector3 direction = target - transform.position;
                Vector3 forward = new Vector3(direction.x, 0, direction.z);
                forward = new Vector3(forward.x, 0, forward.z);
                forward.Normalize();

                transform.forward = Vector3.MoveTowards(transform.forward, forward, maxTurnSpeed);

                navMeshAgent.speed = speed;
                navMeshAgent.Move(transform.forward * speed * Time.deltaTime);
            }
            else
            {
                navMeshAgent.speed = speed;
                navMeshAgent.Move(transform.forward * speed * Time.deltaTime);

                moveStraightTimer -= Time.deltaTime;
            }

            return false;
        }
    }

    public float DirectionToAngle(Vector3 direction)
    {
        return Mathf.Atan2(direction.x, direction.z);
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
        UpdateObstacles();

        Vector3 probeVec = transform.forward * probeLength;

        List<ObstacleInfo> avoidObstacles = new List<ObstacleInfo>();

        List<Obstacle> avoidTargets = new List<Obstacle>();

        foreach (Obstacle obstacle in obstacles)
        {
            Vector3 obstacleVec = obstacle.transform.position - transform.position;

            float obstacleDist = obstacleVec.magnitude;

            if (obstacleDist > probeLength + obstacle.GetRadius())
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

            if (obstacleToProjectionDist < radius + obstacle.GetRadius())
            {
                avoidTargets.Add(obstacle);


                Vector3 avoidDirection = (obstacleProjectionOnProbeDist * transform.forward - obstacleDir);
                if (Vector3.Angle(obstacleDir, transform.forward) < 30)
                {
                    avoidDirection = transform.right;
                }

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
        public Obstacle obstacle;
        public Vector3 obstacleVec;
        public Vector3 avoidDirection;
    }

    public Vector3 DirectionFromAngle(float angleInDegrees)
    {
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public void UpdateObstacles()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, obstacleNoticeRadius, obstacleLayer);
        foreach (Collider c in colliders)
        {
            if(c.gameObject == gameObject) { continue; }

            Obstacle obstacle = c.GetComponent<Obstacle>();
            if (!obstacles.Contains(obstacle))
            {
                obstacles.Add(obstacle);
            }
        }

        foreach(Obstacle obstacle in obstacles)
        {
            if(Vector3.Distance(obstacle.transform.position, transform.position) > obstacleNoticeRadius)
            {
                toDelete.Add(obstacle);
            }
        }

        foreach(Obstacle obstacle in toDelete)
        {
            obstacles.Remove(obstacle);
        }

        toDelete.Clear();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (ObstacleInfo avoidTarget in GetAvoidTargets())
        {
            Gizmos.DrawWireCube(avoidTarget.obstacle.transform.position, Vector3.one);
        }

        Gizmos.color = Color.green;
        foreach (Obstacle obstacle in obstacles)
        {
            Gizmos.DrawWireSphere(obstacle.transform.position, obstacle.GetRadius());
        }

        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * probeLength);

        Gizmos.color = Color.red;
        Gizmos.DrawCube(target, Vector3.one);
    }
}