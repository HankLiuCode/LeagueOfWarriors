using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Dota.Utils;
using Mirror;

public enum ActorState
{
    None,
    Idle,
    Move,
    Attack
}

public class Actor : NetworkBehaviour
{
    public const float MOVE_EPSILON = 0.3f;

    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;

    [SerializeField] ActorState currentState;
    [SerializeField] Vector3 moveToPoint;
    [SerializeField] CombatTarget attackTarget;

    [SerializeField] float speed = 3f;


    private void Update()
    {
        switch (currentState)
        {
            case ActorState.None:
                break;

            case ActorState.Idle:
                break;

            case ActorState.Move:
                Vector3 direction = VectorConvert.XZVector(transform.position, moveToPoint);
                if (direction.magnitude < MOVE_EPSILON)
                {
                    currentState = ActorState.Idle;
                }
                break;

            case ActorState.Attack:
                Vector3 movePos = VectorConvert.XZVector(attackTarget.transform.position);
                Vector3 attackDir = VectorConvert.XZDirection(transform.position, movePos);
                if (attackDir.magnitude < MOVE_EPSILON)
                {
                    currentState = ActorState.Idle;
                }
                break;
        }

        Vector3 velocity = agent.velocity;
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);
        float forwardSpeed = localVelocity.normalized.z;
        animator.SetBool("running", forwardSpeed > 0.1);
    }

    public void Attack(CombatTarget target)
    {
        attackTarget = target;
        currentState = ActorState.Attack;
        agent.SetDestination(moveToPoint);
    }

    public void MoveTo(Vector3 moveToPoint)
    {
        this.moveToPoint = VectorConvert.XZVector(moveToPoint);
        currentState = ActorState.Move;
        agent.SetDestination(moveToPoint);
    }
}
