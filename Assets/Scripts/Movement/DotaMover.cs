using Dota.Core;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Dota.Movement
{
    public class DotaMover : NetworkBehaviour, IAction
    {
        [SerializeField] Animator animator = null;
        [SerializeField] Health health = null;
        [SerializeField] PathFollower pathFollower = null;
        [SerializeField] float maxSpeed = 5;
        [SerializeField] ActionLocker actionScheduler = null;

        [SerializeField] int priority = 0;


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
            return priority;
        }

        [ClientCallback]
        private void Update()
        {
            if (!hasAuthority) { return; }

            pathFollower.SetEnabled(!health.IsDead());

            Vector3 velocity = pathFollower.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);

            float forwardSpeed = localVelocity.normalized.z;
            animator.SetFloat("forwardSpeed", forwardSpeed);
        }
        #endregion
    }
}