using Dota.Attributes;
using Dota.Movement;
using Dota.Utils;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerFighter : NetworkBehaviour
{
    public const float MOVE_EPSILON = 0.1f;

    Health target;

    [SerializeField] float attackCooldownTimer;
    [SerializeField] float attackRange = 2f;

    // this cannot be lower than 1f or animation will be strange
    [SerializeField]
    float timeBetweenAttacks = 1f;

    [SerializeField] NetworkAnimator netAnimator = null;
    [SerializeField] AnimationEventHandler animationEventHandler = null;
    [SerializeField] ServerMover mover = null;
    [SerializeField] StatStore statStore = null;
    

    [SerializeField] bool hasFinishedBackswing = true;

    
    [Server]
    public void ServerDealDamageTo(Health health, float damage)
    {
        health.ServerTakeDamage(damage);
    }

    public override void OnStartServer()
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

        if (TeamChecker.IsSameTeam(gameObject, combatTarget)) { return false; }

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
        if (target != null)
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
        ServerDealDamageTo(target, statStore.GetStats().attackDamage);
    }

    private bool GetIsInRange()
    {
        return Vector3.Distance(transform.position, target.transform.position) < attackRange;
    }

    [ServerCallback]
    private void Update()
    {
        attackCooldownTimer -= Time.deltaTime;

        if (target == null) { return; }

        if (target.IsDead()) { return; }

        if (!GetIsInRange())
        {
            if (hasFinishedBackswing)
            {
                Vector3 targetDir = (target.transform.position - transform.position).normalized;
                mover.MoveTo(target.transform.position - targetDir * (attackRange - MOVE_EPSILON));
            }
        }
        else
        {
            if (attackCooldownTimer <= 0)
            {
                attackCooldownTimer = timeBetweenAttacks;
                transform.LookAt(target.transform);
                netAnimator.ResetTrigger("stopAttack");
                netAnimator.SetTrigger("attack");
                hasFinishedBackswing = false;
            }
        }
    }
}