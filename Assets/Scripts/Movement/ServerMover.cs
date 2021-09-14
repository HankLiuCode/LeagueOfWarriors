using Dota.Attributes;
using Dota.Core;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class ServerMover : NetworkBehaviour, IAction
{
    public const int MOVE_ACTION_PRIORITY = 0;

    [SerializeField] Animator animator = null;
    [SerializeField] Health health = null;
    [SerializeField] DotaAgent agent = null;
    [SerializeField] ActionLocker actionLocker = null;
    [SerializeField] StatStore statStore = null;

    #region Server

    public void MoveTo(Vector3 position)
    {
        if (health.IsDead()) { return; }

        bool canMove = actionLocker.TryGetLock(this);
        if (canMove)
        {
            agent.Speed = statStore.GetStats().moveSpeed;
            agent.IsStopped = false;
            agent.SetDestination(position);
        }
    }

    public void SetAreaMask(int mask)
    {
        //agent.areaMask = mask;
    }

    [Client]
    public void End()
    {
        agent.IsStopped = true;
    }

    [Client]
    public void Begin()
    {

    }

    public int GetPriority()
    {
        return MOVE_ACTION_PRIORITY;
    }

    private void Update()
    {
        if (isServer)
        {
            animator.SetBool("running", false);
        }
    }

    #endregion
}


//public class ServerMover : NetworkBehaviour, IAction
//{
//    public const int MOVE_ACTION_PRIORITY = 0;

//    [SerializeField] Animator animator = null;
//    [SerializeField] Health health = null;
//    [SerializeField] NavMeshAgent agent = null;
//    [SerializeField] ActionLocker actionLocker = null;
//    [SerializeField] StatStore statStore = null;

//    #region Server
//    public override void OnStartServer()
//    {
//        agent.speed = statStore.GetStats().moveSpeed;
//    }

//    public void MoveTo(Vector3 position)
//    {
//        if (health.IsDead()) { return; }

//        bool canMove = actionLocker.TryGetLock(this);
//        if (canMove)
//        {
//            agent.speed = statStore.GetStats().moveSpeed;
//            agent.isStopped = false;
//            agent.SetDestination(position);
//        }
//    }

//    public void SetAreaMask(int mask)
//    {
//        agent.areaMask = mask;
//    }

//    [Client]
//    public void End()
//    {
//        agent.isStopped = true;
//    }

//    [Client]
//    public void Begin()
//    {

//    }

//    public int GetPriority()
//    {
//        return MOVE_ACTION_PRIORITY;
//    }

//    private void Update()
//    {
//        agent.enabled = !health.IsDead();

//        if (isServer)
//        {
//            Vector3 velocity = agent.velocity;

//            Vector3 localVelocity = transform.InverseTransformDirection(velocity);

//            float forwardSpeed = localVelocity.normalized.z;

//            animator.SetBool("running", forwardSpeed > 0.1);
//        }
//    }

//    #endregion
//}
