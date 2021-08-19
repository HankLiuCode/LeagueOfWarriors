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
        [SerializeField] float speed = 8;
        [SerializeField] Vector3 direction;
        [SerializeField] float damage = 50;
        Vector3 startPos;
        GameObject owner;

        [SyncVar]
        Vector3 destroyPoint;

        // OnTriggerEnter Is sometimes called twice or more, this prevents it
        bool hasHit;

        #region Both

        private void Update()
        {
            if (isServer)
            {
                ServerUpdate();
            }

            if (hasAuthority && isClient)
            {
                ClientUpdate();
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
        public void ServerSetDirection(Vector3 startPos, Vector3 direction, float travelDist)
        {
            this.direction = new Vector3(direction.normalized.x, 0, direction.normalized.z);
            this.startPos = startPos;
            destroyPoint = startPos + this.direction.normalized * travelDist;
        }

        [Server]
        public void ServerSetSpeed(float speed)
        {
            this.speed = speed;
        }

        [Server]
        public void ServerSetOwner(GameObject owner)
        {
            this.owner = owner;
        }

        [Server]
        public void ServerSetDamage(float damage)
        {
            this.damage = damage;
        }

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == owner) { return; }

            Health health = other.GetComponent<Health>();
            if (health && !hasHit)
            {
                if (health.IsDead()) { return; }

                hasHit = true;
                health.ServerTakeDamage(damage);
            }
        }

        [Server]
        private void ServerUpdate()
        {
            transform.forward = destroyPoint - transform.position;

            transform.position += direction * speed * Time.deltaTime;

            if (Vector3.Distance(startPos, destroyPoint) < Vector3.Distance(startPos, transform.position))
            {
                Debug.Log("ServerDestroy");
            }
        }

        #endregion

        #region Client

        [Client]
        private void ClientUpdate()
        {
            transform.forward = destroyPoint - transform.position;

            transform.position += direction * speed * Time.deltaTime;

            if (Vector3.Distance(startPos, destroyPoint) < Vector3.Distance(startPos, transform.position))
            {
                Destroy(gameObject);
            }
        }
        #endregion
    }
}