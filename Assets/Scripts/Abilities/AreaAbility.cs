﻿using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dota.Controls;
using Dota.Core;


public class AreaAbility : Ability, IAction
{
    [SerializeField] GameObject indicatorPrefab = null;
    CircleIndicator areaIndicator = null;

    [SerializeField] GameObject spellRangePrefab = null;
    CircleIndicator spellRangeInstance = null;

    [SerializeField] GameObject damageRadiusPrefab = null;
    [SerializeField] GameObject spellPrefab = null;
    
    [SerializeField] NetworkAnimator networkAnimator = null;
    [SerializeField] AnimationEventHandler animationEventHandler = null;
    [SerializeField] string animationTrigger = "abilityW";

    [SerializeField] ActionLocker actionLocker = null;

    [SerializeField] float maxRange = 2f;
    [SerializeField] float damage = 50f;

    [SerializeField] float damageRadius = 1f;
    [SerializeField] float delayTime = 1f;
    [SerializeField] float destroyTime = 3f;
    [SerializeField] int priority = 1;

    #region Server

    [Server]
    IEnumerator CastSpell(AbilityData abilityData)
    {
        NetworkCircleIndicator damageRadiusInstance = Instantiate(damageRadiusPrefab, abilityData.castPos, Quaternion.identity).GetComponent<NetworkCircleIndicator>();
        NetworkServer.Spawn(damageRadiusInstance.gameObject);
        damageRadiusInstance.ServerSetPosition(abilityData.castPos);
        damageRadiusInstance.ServerSetRadius(damageRadius);

        yield return new WaitForSeconds(abilityData.delayTime);

        Debug.Log(abilityData.castPos);
        GameObject effectInstance = Instantiate(spellPrefab, abilityData.castPos, Quaternion.identity);
        NetworkServer.Spawn(effectInstance, connectionToClient);

        Collider[] colliders = Physics.OverlapSphere(abilityData.castPos, damageRadius);
        foreach (Collider c in colliders)
        {
            GameObject go = c.gameObject;
            Health health = go.GetComponent<Health>();
            if (health)
            {
                health.ServerTakeDamage(damage);
            }
        }
        yield return new WaitForSeconds(destroyTime);
        NetworkServer.Destroy(effectInstance);
        NetworkServer.Destroy(damageRadiusInstance.gameObject);
    }

    [Server]
    public void ServerSpawnAbilityEffect(AbilityData abilityData)
    {
        StartCoroutine(CastSpell(abilityData));
    }

    [Command]
    public void CmdSpawnAbilityEffect(AbilityData abilityData)
    {
        ServerSpawnAbilityEffect(abilityData);
    }
    #endregion

    #region Client
    public override void OnStartAuthority()
    {
        areaIndicator = Instantiate(indicatorPrefab).GetComponent<CircleIndicator>();
        spellRangeInstance = Instantiate(spellRangePrefab).GetComponent<CircleIndicator>();

        areaIndicator.SetRadius(damageRadius);
        spellRangeInstance.SetRadius(maxRange);
        HideIndicator();
        
        animationEventHandler.OnAttackBackswing += AnimationEventHandler_OnAttackBackswing;
        animationEventHandler.OnAttackPoint += AnimationEventHandler_OnAttackPoint;
    }

    [Client]
    public override void ShowIndicator()
    {
        areaIndicator.gameObject.SetActive(true);
        spellRangeInstance.gameObject.SetActive(true);
    }

    [Client]
    public override void UpdateIndicator(AbilityData abilityData)
    {
        spellRangeInstance.SetPosition(abilityData.casterPos);
        Vector3 direction = (abilityData.mouseClickPos - abilityData.casterPos).normalized;
        float range = (abilityData.mouseClickPos - abilityData.casterPos).magnitude;
        Vector3 castPosition = transform.position + direction * Mathf.Min(maxRange, range);
        
        abilityData.castPos = castPosition;

        areaIndicator.SetPosition(castPosition);
    }

    [Client]
    public override void HideIndicator()
    {
        areaIndicator.gameObject.SetActive(false);
        spellRangeInstance.gameObject.SetActive(false);
    }

    [Client]
    public override void Cast(AbilityData abilityData)
    {
        bool canDo = actionLocker.TryGetLock(this);
        if (canDo)
        {
            networkAnimator.SetTrigger(animationTrigger);

            transform.LookAt(abilityData.castPos, Vector3.up);

            abilityData.delayTime = delayTime;

            CmdSpawnAbilityEffect(abilityData);
        }
    }

    public void Begin()
    {
        
    }

    public void End()
    {
        
    }

    public int GetPriority()
    {
        return priority;
    }

    private void AnimationEventHandler_OnAttackBackswing()
    {
        actionLocker.ReleaseLock(this);
    }
    private void AnimationEventHandler_OnAttackPoint()
    {
        
    }

    #endregion
}
