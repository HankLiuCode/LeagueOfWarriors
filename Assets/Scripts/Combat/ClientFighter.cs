using Dota.Attributes;
using Dota.Core;
using Dota.Movement;
using Dota.Utils;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dota.Combat
{
    public class ClientFighter : NetworkBehaviour
    {
        public const float MOVE_EPSILON = 0.1f;

        CombatTarget target;

        [SerializeField] float attackCooldownTimer;
        [SerializeField] float attackRange = 2f;

        // this cannot be lower than 1f or animation will be strange
        [SerializeField] 
        float timeBetweenAttacks = 1f;

        [SerializeField] NetworkAnimator netAnimator = null;
        [SerializeField] AnimationEventHandler animationEventHandler = null;
        [SerializeField] ClientMover mover = null;
        [SerializeField] StatStore statStore = null;
        [SerializeField] ActionLocker actionLocker = null;

        [SerializeField] bool hasFinishedBackswing = true;

        #region Server

        [Server]
        public void ServerDealDamageTo(Health health, float damage)
        {
            health.ServerTakeDamage(damage, netIdentity);
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

        public bool IsAttackable(GameObject combatCandidate)
        {
            if (combatCandidate == gameObject) return false;

            if(TeamChecker.IsSameTeam(gameObject, combatCandidate)) { return false; }

            CombatTarget combatTarget = combatCandidate.GetComponent<CombatTarget>();

            if (combatTarget == null) return false;

            return true;
        }

        public void StartAttack(GameObject combatTarget)
        {
            CombatTarget target = combatTarget.GetComponent<CombatTarget>();

            if (target == null) { return; }

            this.target = target;
        }

        public void StopAttack()
        {
            if(target != null)
            {
                target = null;
                hasFinishedBackswing = true;
                TriggerStopAttackAnimation();
            }
        }

        private void TriggerStopAttackAnimation()
        {
            netAnimator.ResetTrigger("attack");
            netAnimator.SetTrigger("stopAttack");
        }

        void MeleeAttack()
        {
            CmdDealDamageTo(target.GetHealth(), statStore.GetStats().attackDamage);
        }

        private bool GetIsInRange()
        {
            bool isInRange = Vector3.Distance(transform.position, target.transform.position) < attackRange + target.GetAllowAttackRadius();

            return isInRange;
        }
        
        [ClientCallback]
        private void Update()
        {
            if (!hasAuthority) { return; }

            attackCooldownTimer -= Time.deltaTime;

            if (target == null) { return; }

            if (target.GetHealth().IsDead()) { return; }
            
            if (!GetIsInRange())
            {
                if (hasFinishedBackswing)
                {
                    Vector3 targetDir = (target.transform.position - transform.position).normalized;

                    Vector3 moveToPos = target.transform.position - targetDir * (attackRange - MOVE_EPSILON);

                    mover.MoveTo(moveToPos);
                }
            }
            else
            {
                transform.LookAt(target.transform);

                if (attackCooldownTimer <= 0)
                {
                    attackCooldownTimer = timeBetweenAttacks;
                    netAnimator.ResetTrigger("stopAttack");
                    netAnimator.SetTrigger("attack");
                    hasFinishedBackswing = false;
                }
            }
        }
        #endregion
    }
}