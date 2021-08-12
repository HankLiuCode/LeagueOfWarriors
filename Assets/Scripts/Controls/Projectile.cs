using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dota.Core;

namespace Dota.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] Health target = null;
        [SerializeField] float speed = 1;

        float damage = 0;


        public void SetTarget(Health target, float damage)
        {
            this.target = target;
            this.damage = damage;
        }

        private void Update()
        {
            if (target == null) { return; }

            transform.LookAt(GetAimLocation());
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, GetAimLocation()) < 0.1)
            {
                target.TakeDamage(damage);
                Destroy(gameObject);
            }
        }


        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null)
            {
                return target.transform.position;
            }
            return target.transform.position + Vector3.up * targetCapsule.height / 2;
        }
    }

}