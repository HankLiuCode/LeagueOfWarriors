using Dota.Core;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Dota.Movement
{
    public class DotaMover : NetworkBehaviour
    {
        [SerializeField] Animator animator = null;
        [SerializeField] Health health = null;
        [SerializeField] PathFollower pathFollower = null;


        #region Client
        [Client]
        public void MoveTo(Vector3 position)
        {
            pathFollower.isStopped = false;
            pathFollower.SetDestination(position);
        }

        [Client]
        public void MoveStop()
        {
            pathFollower.isStopped = true;
        }

        [ClientCallback]
        private void Update()
        {
            if (!hasAuthority) { return; }

            pathFollower.SetEnabled(!health.IsDead());

            Vector3 velocity = pathFollower.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float forwardSpeed = localVelocity.z;
            animator.SetFloat("forwardSpeed", forwardSpeed);
        }
        #endregion
    }
}