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

        [SerializeField] float attackDuration = 1f;
        [SerializeField] bool isAttacking = false;

        [SerializeField] float attackRange = 2f;
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] float attackDamage = 20f;
        [SerializeField] Animator animator = null;
        [SerializeField] Health health = null;

        [SerializeField] Transform rightHand = null;
        [SerializeField] Transform leftHand = null;

        // if basic attack is melee then its null
        [SerializeField] Projectile projectilePrefab = null;

        float atkRotAdjust = 70f;

        #region Server

        [ClientRpc]
        private void RpcTriggerAttackAnimation()
        {
            if (!isAttacking)
            {
                StartCoroutine(AttackAnimation());
            }
        }

        IEnumerator AttackAnimation()
        {
            animator.ResetTrigger("stopAttack");
            animator.SetTrigger("attack");
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

        [ClientRpc]
        private void RpcTriggerStopAttackAnimation()
        {
            animator.ResetTrigger("attack");
            animator.SetTrigger("stopAttack");
        }

        [Server]
        public void ServerTryAttack(GameObject combatTarget)
        {
            Health health = combatTarget.GetComponent<Health>();
            if (health == null) { return; }
            target = health;
        }

        [Server]
        public void ServerStopAttack()
        {
            target = null;
            RpcTriggerStopAttackAnimation();
        }

        [Command]
        public void CmdTryAttack(GameObject combatTarget)
        {
            ServerTryAttack(combatTarget);
        }

        [Command]
        public void CmdStopAttack()
        {
            ServerStopAttack();
        }

        [Server]
        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < attackRange;
        }

        [Server]
        private void ServerAttackBehaviour()
        {
            Vector3 targetDir = target.transform.position - transform.position;
            transform.LookAt(transform.position + Quaternion.AngleAxis(atkRotAdjust, Vector3.up) * targetDir);

            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                RpcTriggerAttackAnimation();
                timeSinceLastAttack = 0;
            }
        }

        [Server]
        void MeleeAttack()
        {
            target.ServerTakeDamage(attackDamage);
        }

        [Server]
        void RangedAttack()
        {
            Projectile projectile = Instantiate(projectilePrefab, rightHand.transform.position, Quaternion.identity);
            NetworkServer.Spawn(projectile.gameObject);
            projectile.SetTarget(target, attackDamage);
        }

        // Animation Event
        [Server]
        void Hit()
        {
            if(target == null) { return; }

            if(projectilePrefab == null)
            {
                MeleeAttack();
            }
            else
            {
                RangedAttack();
            }
        }

        #endregion

        #region Client
        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == gameObject) return false;
            Health health = combatTarget.GetComponent<Health>();
            if (health == null) return false;
            return true;
        }

        private void Update()
        {
            if (isServer)
            {
                timeSinceLastAttack += Time.deltaTime;

                if (target == null) { return; }

                if (target.IsDead()) { return; }

                if (!GetIsInRange())
                {
                    // !!!
                    if (isAttacking) { return; }
                    GetComponent<DotaMover>().ServerMoveTo(target.transform.position);
                }
                else
                {
                    GetComponent<DotaMover>().ServerMoveStop();
                    ServerAttackBehaviour();
                }
            }
        }

        #endregion
    }
}
