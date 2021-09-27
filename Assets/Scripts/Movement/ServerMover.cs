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
            agent.CanMove = true;
            agent.SetDestination(position);
        }
    }

    public void SetAreaMask(int mask)
    {
        agent.SetMask(mask);
    }

    [Client]
    public void End()
    {
        agent.CanMove = false;
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
            animator.SetBool("running", agent.IsMoving);
        }
    }

    #endregion
}