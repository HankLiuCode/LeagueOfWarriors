using Dota.Core;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

namespace Dota.Movement
{
    public class DotaMover : NetworkBehaviour, IAction
    {
        public const int MOVE_ACTION_PRIORITY = 0;

        [SerializeField] Animator animator = null;
        [SerializeField] Health health = null;
        [SerializeField] NavMeshAgent agent = null;
        [SerializeField] ActionLocker actionScheduler = null;
        [SerializeField] Stats stats = null;

        #region Client
        public override void OnStartAuthority()
        {
            agent.speed = stats.GetMovementSpeed();
        }

        [Client]
        public void MoveTo(Vector3 position)
        {
            bool canMove = actionScheduler.TryGetLock(this);
            if (canMove)
            {
                agent.speed = stats.GetMovementSpeed();
                agent.isStopped = false;
                agent.SetDestination(position);
            }
        }

        [Client]
        public void End()
        {
            agent.isStopped = true;
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
            agent.enabled = !health.IsDead();

            if (isClient)
            {
                if (!hasAuthority) { return; }

                Vector3 velocity = agent.velocity;

                Vector3 localVelocity = transform.InverseTransformDirection(velocity);

                float forwardSpeed = localVelocity.normalized.z;

                animator.SetBool("running", forwardSpeed > 0.1);
            }
        }
        #endregion
    }
}