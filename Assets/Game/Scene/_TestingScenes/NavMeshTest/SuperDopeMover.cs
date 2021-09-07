using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SuperDopeMover : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent = null;
    [SerializeField] float speed = 5f;
    NavMeshPath path = null;
    [SerializeField] int nextIndex = -1;


    private void Start()
    {
        path = new NavMeshPath();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                agent.CalculatePath(hit.point, path);
                nextIndex = 0;
            }
        }

        if (nextIndex >= path.corners.Length)
        {
            nextIndex = -1;
        }

        if (nextIndex == -1) { return; }

        Vector3 twoDPosition = new Vector3(transform.position.x, 0, transform.position.z);

        Vector3 movement = (path.corners[nextIndex] - twoDPosition).normalized * speed * Time.deltaTime;

        agent.Move(movement);

        if(Vector3.Distance(twoDPosition, path.corners[nextIndex]) < 0.1)
        {
            nextIndex++;

            Ray ray = new Ray(twoDPosition + Vector3.up * 0.5f, path.corners[nextIndex] - twoDPosition);

            bool hasHit = Physics.Raycast(ray, out RaycastHit hit, Vector3.Distance(twoDPosition, path.corners[nextIndex]));

            if (hasHit)
            {
                Debug.Log("Has Obstacle In Path");
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (path != null)
        {
            for(int i=0; i < path.corners.Length - 1; i++)
            {
                Gizmos.DrawCube(path.corners[i], Vector3.one);
                Gizmos.DrawCube(path.corners[i + 1], Vector3.one);
                Gizmos.DrawLine(path.corners[i], path.corners[i + 1]);
            }
        }
    }

}
