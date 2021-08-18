using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class PathFollower : MonoBehaviour
{
    // change to private when done
    public NavMeshAgent agent = null;
    public bool isStopped { get { return agent.isStopped; } set { agent.isStopped = value;  } }
    public Vector3 velocity { get { return agent.velocity; } set { agent.velocity = value; } }



    public void SetSpeed(float speed)
    {
        agent.speed = speed;
    }

    public void SetStopRange(float stopRange)
    {
        agent.stoppingDistance = stopRange;
    }

    public void SetEnabled(bool isEnabled)
    {
        agent.enabled = isEnabled;
    }

    public bool GetEnabled()
    {
        return agent.enabled;
    }

    public void SetDestination(Vector3 position)
    {
        agent.SetDestination(position);
    }
}
