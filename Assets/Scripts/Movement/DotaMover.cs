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
        [SerializeField] NavMeshAgent agent = null;
        [SerializeField] Animator animator = null;
        [SerializeField] Health health = null;

        // animator parameters
        [SyncVar]
        float forwardSpeed;


        #region Server

        [Server]
        public void ServerMoveTo(Vector3 position)
        {
            agent.isStopped = false;
            agent.SetDestination(position);
        }

        [Server]
        public void ServerMoveStop()
        {
            agent.isStopped = true;
        }

        [Command]
        public void CmdMoveTo(Vector3 position)
        {
            ServerMoveTo(position);
        }

        [Command]
        public void CmdMoveStop()
        {
            ServerMoveStop();
        }

        [Server]
        private void ServerUpdateAnimator()
        {
            Vector3 velocity = agent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            forwardSpeed = localVelocity.z;
        }

        #endregion

        [Client]
        private void ClientUpdateAnimator()
        {
            animator.SetFloat("forwardSpeed", forwardSpeed);
        }

        private void Update()
        {
            if (isServer)
            {
                agent.enabled = !health.IsDead();
                
                ServerUpdateAnimator();
            }

            ClientUpdateAnimator();
        }
    }

}