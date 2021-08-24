using Dota.Core;
using Dota.Movement;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dota.Combat
{
    public class DotaFighter : NetworkBehaviour
    {
        Health target;
        float timeSinceLastAttack = Mathf.Infinity;

        [SerializeField] float attackRange = 2f;
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] float attackDamage = 20f;

        [SerializeField] NetworkAnimator netAnimator = null;
        [SerializeField] AnimationEventHandler animationEventHandler = null;
        [SerializeField] DotaMover mover = null;

        [SerializeField] Transform rightHand = null;
        [SerializeField] Transform leftHand = null;

        // if basic attack is melee then its null
        [SerializeField] DotaProjectile projectilePrefab = null;

        bool hasFinishedBackswing = true;

        #region Server

        [Server]
        public void ServerDealDamageTo(Health health, float damage)
        {
            health.ServerTakeDamage(damage);
        }

        [Command]
        public void CmdDealDamageTo(Health health, float damage)
        {
            ServerDealDamageTo(health, damage);
        }

        [Server]
        public void ServerSpawnProjectile(Health target, GameObject owner, float attackDamage)
        {
            DotaProjectile projectile = Instantiate(projectilePrefab, rightHand.transform.position, Quaternion.identity);
            NetworkServer.Spawn(projectile.gameObject, owner.GetComponent<NetworkIdentity>().connectionToClient);
            projectile.SetTarget(target, attackDamage);
        }

        [Command]
        public void CmdSpawnProjectile(Health target, GameObject owner, float attackDamage)
        {
            ServerSpawnProjectile(target, owner, attackDamage);
        }

        #endregion

        #region Client

        public override void OnStartAuthority()
        {
            animationEventHandler.OnAttackBackswing += AttackBackSwing;
            animationEventHandler.OnAttackPoint += AttackPoint;
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == gameObject) return false;
            Health health = combatTarget.GetComponent<Health>();
            if (health == null) return false;
            return true;
        }

        public void StartAttack(GameObject combatTarget)
        {
            Health health = combatTarget.GetComponent<Health>();
            if (health == null) { return; }
            target = health;
        }

        public void StopAttack()
        {
            target = null;
            TriggerStopAttackAnimation();
        }

        private void TriggerStopAttackAnimation()
        {
            netAnimator.ResetTrigger("attack");
            netAnimator.SetTrigger("stopAttack");
        }

        void MeleeAttack()
        {
            CmdDealDamageTo(target, attackDamage);
        }

        void RangedAttack()
        {
            CmdSpawnProjectile(target, gameObject, attackDamage);
        }
        
        // Animation Event
        void AttackPoint()
        {
            if (target == null) { return; }

            if (projectilePrefab == null)
            {
                MeleeAttack();
            }
            else
            {
                RangedAttack();
            }
        }

        // Animation Event
        void AttackBackSwing()
        {
            hasFinishedBackswing = true;
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < attackRange;
        }

        private void Update()
        {
            if (!hasAuthority) { return; }

            timeSinceLastAttack += Time.deltaTime;

            if (target == null) { return; }

            if (target.IsDead()) { return; }



            if (!GetIsInRange())
            {
                if (hasFinishedBackswing)
                {
                    mover.MoveTo(target.transform.position);
                }
            }
            else
            {
                if (timeSinceLastAttack > timeBetweenAttacks)
                {
                    netAnimator.ResetTrigger("stopAttack");
                    netAnimator.SetTrigger("attack");
                    timeSinceLastAttack = 0;
                    hasFinishedBackswing = false;
                }
            }
        }
        #endregion
    }
}
