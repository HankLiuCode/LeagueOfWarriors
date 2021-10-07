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

    CombatTarget target;

    [SerializeField] float attackCooldownTimer;
    [SerializeField] float attackRange = 2f;

    // this cannot be lower than 1f or animation will be strange
    [SerializeField]
    float timeBetweenAttacks = 1f;

    [SerializeField] NetworkAnimator netAnimator = null;
    [SerializeField] AnimationEventHandler animationEventHandler = null;
    [SerializeField] Health health = null;
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
        CombatTarget target = combatTarget.GetComponent<CombatTarget>();
        if (target == null) { return; }
        this.target = target;
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
        Health health = target.GetHealth();
        ServerDealDamageTo(health, statStore.GetStats().attackDamage);
    }

    private bool GetIsInRange()
    {
        bool isInRange = Vector3.Distance(transform.position, target.transform.position) < attackRange + target.GetAllowAttackRadius();
        return isInRange;
    }

    [Server]
    private Vector3 GetAttackPosition()
    {
        Vector3 targetPos = target.transform.position;

        Vector3 targetDir = (targetPos - transform.position).normalized;

        Vector3 relativePosFromTarget = targetDir * (attackRange - MOVE_EPSILON);

        Vector3 defaultAttackPos = targetPos - relativePosFromTarget;

        return defaultAttackPos;
    }

    [Server]
    private bool HasObstacle(Vector3 position)
    {
        List<DotaObstacle> obstacles = ObstacleManager.GetInstance().GetObstacles();
        foreach(DotaObstacle o in obstacles)
        {
            if(VectorConvert.XZDistance(o.transform.position, position) < o.GetRadius())
            {
                return true;
            }
        }
        return false;
    }

    [ServerCallback]
    private void Update()
    {
        if (health.IsDead()) { return; }

        attackCooldownTimer -= Time.deltaTime;

        if (target == null) { return; }

        if (target.GetHealth().IsDead()) { return; }

        if (!GetIsInRange())
        {
            if (hasFinishedBackswing)
            {
                mover.MoveTo(GetAttackPosition());
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if(target != null)
        {
            Gizmos.DrawCube(target.transform.position, Vector3.one);
        }
    }
}