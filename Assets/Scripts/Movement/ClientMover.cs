using Dota.Attributes;
using Dota.Core;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

namespace Dota.Movement
{
    public class ClientMover : NetworkBehaviour, IAction
    {
        public const int MOVE_ACTION_PRIORITY = 0;

        [SerializeField] Animator animator = null;
        [SerializeField] Health health = null;
        [SerializeField] NavMeshAgent agent = null;
        [SerializeField] ActionLocker actionLocker = null;
        [SerializeField] StatStore statStore = null;


        #region Server

        public override void OnStartServer()
        {
            health.ServerOnHealthDead += Health_ServerOnHealthDead;
        }

        private void Health_ServerOnHealthDead(Health obj, NetworkIdentity attacker)
        {
            agent.enabled = false;
        }
        #endregion


        #region Client

        public override void OnStartClient()
        {
            health.ClientOnHealthDead += Health_ClientOnHealthDead;
        }

        private void Health_ClientOnHealthDead(Health obj, NetworkIdentity attacker)
        {
            agent.enabled = false;
        }

        public override void OnStartAuthority()
        {
            agent.speed = statStore.GetStats().moveSpeed;
        }

        [Client]
        public void MoveTo(Vector3 position)
        {
            bool canMove = actionLocker.TryGetLock(this);

            if (canMove)
            {
                NavMesh.SamplePosition(position, out NavMeshHit hit, 10f, NavMesh.AllAreas);

                agent.speed = statStore.GetStats().moveSpeed;
                agent.isStopped = false;
                agent.SetDestination(hit.position);
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