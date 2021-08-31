using Dota.Core;
using Mirror;
using UnityEngine;

namespace Dota.Movement
{
    public class DotaMover : NetworkBehaviour, IAction
    {
        public const int MOVE_ACTION_PRIORITY = 0;

        [SerializeField] Animator animator = null;
        [SerializeField] Health health = null;
        [SerializeField] PathFollower pathFollower = null;
        [SerializeField] ActionLocker actionScheduler = null;
        [SerializeField] float maxSpeed = 5;


        #region Client
        public override void OnStartAuthority()
        {
            pathFollower.SetSpeed(maxSpeed);
        }

        [Client]
        public void SetSpeed(float speed)
        {
            pathFollower.SetSpeed(speed);
        }

        [Client]
        public void SetStopRange(float stopRange)
        {
            pathFollower.SetStopRange(stopRange);
        }

        [Client]
        public void MoveTo(Vector3 position)
        {
            bool canMove = actionScheduler.TryGetLock(this);
            if (canMove)
            {
                pathFollower.isStopped = false;
                pathFollower.SetDestination(position);
            }
        }

        [Client]
        public void End()
        {
            pathFollower.isStopped = true;
        }

        [Client]
        public void Begin()
        {

        }

        public int GetPriority()
        {
            return MOVE_ACTION_PRIORITY;
        }

        [ClientCallback]
        private void Update()
        {
            if (!hasAuthority) { return; }

            pathFollower.SetEnabled(!health.IsDead());

            Vector3 velocity = pathFollower.velocity;

            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            
            float forwardSpeed = localVelocity.normalized.z;

            animator.SetBool("running", forwardSpeed > 0.1);
        }
        #endregion
    }
}