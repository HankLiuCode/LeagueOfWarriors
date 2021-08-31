using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShowAgentPath : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent = null;
    NavMeshPath path = null;


    private void OnDrawGizmos()
    {
        if (agent.hasPath)
        {
            path = agent.path;
        }

        Gizmos.color = Color.green;

        if (path != null)
        {
            Vector3[] wayPoints = path.corners;
            for(int i=0; i<wayPoints.Length - 1; i++)
            {
                Gizmos.DrawLine(wayPoints[i], wayPoints[i + 1]);
            }
        }
    }
}
