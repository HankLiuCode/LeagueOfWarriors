using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dota.Controls;
using Dota.Core;


public class AreaAbility : Ability
{
    [SerializeField] GameObject indicatorPrefab = null;
    CircleIndicator areaIndicatorInstance = null;

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

    #region Server

    [Server]
    IEnumerator CastSpell(AbilityData abilityData)
    {
        NetworkCircleIndicator damageRadiusInstance = Instantiate(damageRadiusPrefab, abilityData.castPos, Quaternion.identity).GetComponent<NetworkCircleIndicator>();
        NetworkServer.Spawn(damageRadiusInstance.gameObject);
        damageRadiusInstance.ServerSetPosition(abilityData.castPos);
        damageRadiusInstance.ServerSetRadius(damageRadius);

        yield return new WaitForSeconds(abilityData.delayTime);
        
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
        areaIndicatorInstance = Instantiate(indicatorPrefab).GetComponent<CircleIndicator>();
        spellRangeInstance = Instantiate(spellRangePrefab).GetComponent<CircleIndicator>();

        areaIndicatorInstance.SetRadius(damageRadius);
        spellRangeInstance.SetRadius(maxRange);
        HideIndicator();
        
        animationEventHandler.OnAttackBackswing += AnimationEventHandler_OnAttackBackswing;
        animationEventHandler.OnAttackPoint += AnimationEventHandler_OnAttackPoint;
    }

    public override void OnStopClient()
    {
        if (!hasAuthority) { return; }

        //Destroy(areaIndicatorInstance.gameObject);
        //Destroy(spellRangeInstance.gameObject);

        animationEventHandler.OnAttackBackswing -= AnimationEventHandler_OnAttackBackswing;
        animationEventHandler.OnAttackPoint -= AnimationEventHandler_OnAttackPoint;
    }

    [Client]
    public override void ShowIndicator()
    {
        areaIndicatorInstance.gameObject.SetActive(true);
        spellRangeInstance.gameObject.SetActive(true);
    }

    [Client]
    public override void UpdateIndicator(AbilityData abilityData)
    {
        spellRangeInstance.SetPosition(abilityData.casterPos);
        
        areaIndicatorInstance.SetPosition(abilityData.mousePos);
    }

    [Client]
    public override void HideIndicator()
    {
        areaIndicatorInstance.gameObject.SetActive(false);
        spellRangeInstance.gameObject.SetActive(false);
    }

    [Client]
    public override void Cast(AbilityData abilityData)
    {
        bool canDo = actionLocker.TryGetLock(this);
        if (canDo)
        {
            Vector3 direction = (abilityData.mousePos - abilityData.casterPos).normalized;

            float range = (abilityData.mousePos - abilityData.casterPos).magnitude;

            Vector3 castPosition = transform.position + direction * Mathf.Min(maxRange, range);

            abilityData.castPos = castPosition;

            networkAnimator.SetTrigger(animationTrigger);

            transform.LookAt(abilityData.castPos, Vector3.up);

            abilityData.delayTime = delayTime;

            CmdSpawnAbilityEffect(abilityData);
        }
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
