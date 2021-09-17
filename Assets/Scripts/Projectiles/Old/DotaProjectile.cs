using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dota.Attributes;
using Mirror;

namespace Dota.Combat
{
    // TODO: Make Parent class from skillshot and projectile
    public class DotaProjectile : NetworkBehaviour
    {
        [SerializeField] Health target = null;
        [SerializeField] float speed = 1;
        float damage = 0;
        GameObject owner;


        #region Server

        [Server]
        public void ServerDealDamageTo(Health health, float damage)
        {
            health.ServerTakeDamage(damage);
        }

        [Server]
        public void ServerSetOwner(GameObject owner)
        {
            this.owner = owner;
        }

        [Server]
        public void SetTarget(Health target, float damage)
        {
            this.target = target;
            this.damage = damage;
        }

        [Server]
        IEnumerator HitAfter(Health health, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            NetworkServer.Destroy(gameObject);
            health.ServerTakeDamage(damage);
        }


        [Server]
        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null)
            {
                return target.transform.position;
            }
            return target.transform.position + Vector3.up * targetCapsule.height / 2;
        }

        [ServerCallback]
        private void Update()
        {
            if (target == null) { return; }

            transform.LookAt(GetAimLocation());

            transform.position = Vector3.MoveTowards(transform.position, GetAimLocation(), speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, GetAimLocation()) < 0.1)
            {
                StartCoroutine(HitAfter(target, 0.1f));
            }
        }
        #endregion
    }
}