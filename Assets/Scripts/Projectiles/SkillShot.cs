using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Dota.Core;

namespace Dota.Abilities
{
    // TODO: Make Parent class from skillshot and projectile
    public class SkillShot : NetworkBehaviour
    {
        [SerializeField] Vector3 direction;
        [SerializeField] float speed = 8;
        [SerializeField] float damage = 50;

        [SyncVar]
        [SerializeField]
        NetworkIdentity owner;

        [SyncVar]
        [SerializeField]
        Vector3 startPos;

        [SyncVar]
        [SerializeField]
        Vector3 destroyPoint;

        // OnTriggerEnter Is sometimes called twice or more, this prevents it
        bool hasHit;

        // Client-Side Collision Detection / Server Spawns

        // Update
        //     skillShot position update on client side

        // OnCollision
        //      CmdNotifyHasHit

        // CmdNotifyServerHit()




        #region Both

        private void Update()
        {
            if (isServer)
            {
                ServerUpdate();
            }

            if (isClient)
            {
                ClientUpdate();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isServer)
            {
                ServerOnTriggerEnter(other);
            }

            if (isClient)
            {
                ClientOnTriggerEnter(other);
            }
        }
        #endregion

        #region Server

        [Server]
        public void ServerDealDamageTo(Health health, float damage)
        {
            health.ServerTakeDamage(damage);
        }

        [Server]
        public void ServerSetSpeed(float speed)
        {
            this.speed = speed;
        }

        [Server]
        public void ServerSetOwner(NetworkIdentity owner)
        {
            this.owner = owner;
        }

        [Server]
        public void ServerSetDamage(float damage)
        {
            this.damage = damage;
        }

        [Server]
        public void ServerSetDirection(Vector3 startPos, Vector3 direction, float travelDist)
        {
            this.direction = new Vector3(direction.normalized.x, 0, direction.normalized.z);

            this.startPos = startPos;
            destroyPoint = startPos + this.direction.normalized * travelDist;
        }

        [Server]
        private void ServerOnTriggerEnter(Collider other)
        {
            NetworkIdentity otherIdentity = other.gameObject.GetComponent<NetworkIdentity>();
            Health health = other.GetComponent<Health>();
            if (health && !hasHit && otherIdentity != owner)
            {
                if (health.IsDead()) { return; }

                hasHit = true;
                health.ServerTakeDamage(damage);
                gameObject.SetActive(false);
            }
        }


        [Server]
        private void ServerUpdate()
        {
            transform.forward = destroyPoint - transform.position;

            transform.position += direction * speed * Time.deltaTime;

            if (Vector3.Distance(startPos, destroyPoint) < Vector3.Distance(startPos, transform.position))
            {
                gameObject.SetActive(false);
            }
        }
        #endregion

        #region Client


        [Client]
        private void ClientUpdate()
        {
            if (Vector3.Distance(startPos, destroyPoint) < Vector3.Distance(startPos, transform.position))
            {
                gameObject.SetActive(false);
            }
        }

        private void ClientOnTriggerEnter(Collider other)
        {
            NetworkIdentity otherIdentity = other.gameObject.GetComponent<NetworkIdentity>();
            Health health = other.GetComponent<Health>();
            if (health && !hasHit && otherIdentity != owner)
            {
                //if (health.IsDead()) { return; }
                hasHit = true;
                gameObject.SetActive(false);
            }
        }
        #endregion
    }
}