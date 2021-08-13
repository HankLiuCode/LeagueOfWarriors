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

        [SerializeField] float attackDuration = 1f;
        [SerializeField] bool isAttacking = false;

        [SerializeField] Animator animator = null;
        [SerializeField] NetworkAnimator netAnimator = null;

        [SerializeField] Health health = null;

        [SerializeField] Transform rightHand = null;
        [SerializeField] Transform leftHand = null;

        // if basic attack is melee then its null
        [SerializeField] Projectile projectilePrefab = null;

        float atkRotAdjust = 70f;

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
            Projectile projectile = Instantiate(projectilePrefab, rightHand.transform.position, Quaternion.identity);
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
        private void TriggerStopAttackAnimation()
        {
            netAnimator.ResetTrigger("attack");
            netAnimator.SetTrigger("stopAttack");
        }

        IEnumerator AttackAnimation()
        {
            netAnimator.ResetTrigger("stopAttack");
            netAnimator.SetTrigger("attack");

            isAttacking = true;
            float timeSinceAttackStart = 0;

            while (timeSinceAttackStart < attackDuration)
            {
                timeSinceAttackStart += Time.deltaTime;
                yield return null;
            }

            isAttacking = false;
            yield return null;
        }

        private void AttackBehaviour()
        {
            Vector3 targetDir = target.transform.position - transform.position;
            transform.LookAt(transform.position + Quaternion.AngleAxis(atkRotAdjust, Vector3.up) * targetDir);

            if (timeSinceLastAttack > timeBetweenAttacks && !isAttacking)
            {
                StartCoroutine(AttackAnimation());
                timeSinceLastAttack = 0;
            }
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

        void MeleeAttack()
        {
            CmdDealDamageTo(target, attackDamage);
        }

        void RangedAttack()
        {
            CmdSpawnProjectile(target, gameObject, attackDamage);
        }
        
        // Animation Event
        void Hit()
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
                if (isAttacking) { return; }
                GetComponent<DotaMover>().MoveTo(target.transform.position);
            }
            else
            {
                GetComponent<DotaMover>().MoveStop();
                AttackBehaviour();
            }
        }

        #endregion
    }
}
