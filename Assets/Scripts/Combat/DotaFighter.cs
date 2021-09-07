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
        [SerializeField] float attackCooldownTimer;

        [SerializeField] float attackRange = 2f;
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] float attackDamage = 20f;

        [SerializeField] NetworkAnimator netAnimator = null;
        [SerializeField] AnimationEventHandler animationEventHandler = null;
        [SerializeField] DotaMover mover = null;

        [SerializeField] bool hasFinishedBackswing = true;

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

        #endregion

        #region Client

        public override void OnStartAuthority()
        {
            animationEventHandler.OnAttackBackswing += AnimationEventHandler_OnAttackBackswing;
            animationEventHandler.OnAttackPoint += AnimationEventHandler_OnAttackPoint;
        }

        private void AnimationEventHandler_OnAttackPoint()
        {
            if (target == null) { return; }
            MeleeAttack();
        }

        private void AnimationEventHandler_OnAttackBackswing()
        {
            hasFinishedBackswing = true;
        }

        public bool IsAttackable(GameObject combatTarget)
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
            hasFinishedBackswing = true;
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

        private bool GetIsInRange()
        {
            Debug.Log(transform.position);
            Debug.Log(target.transform.position);
            return Vector3.Distance(transform.position, target.transform.position) < attackRange;
        }

        [ClientCallback]
        private void Update()
        {
            if (!hasAuthority) { return; }

            attackCooldownTimer -= Time.deltaTime;

            if (target == null) { return; }

            if (target.IsDead()) { return; }

            if (!GetIsInRange())
            {
                if (hasFinishedBackswing)
                {
                    Vector3 targetDir = (target.transform.position - transform.position).normalized;
                    mover.MoveTo(target.transform.position - targetDir);
                }
            }
            else
            {
                if (attackCooldownTimer <= 0 && hasFinishedBackswing)
                {
                    attackCooldownTimer = timeBetweenAttacks;
                    transform.LookAt(target.transform);
                    netAnimator.ResetTrigger("stopAttack");
                    netAnimator.SetTrigger("attack");
                    hasFinishedBackswing = false;
                }
            }
        }
        #endregion
    }
}