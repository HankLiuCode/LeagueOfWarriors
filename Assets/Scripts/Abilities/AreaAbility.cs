using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dota.Controls;
using Dota.Core;


public class AreaAbility : Ability, IAction
{
    [SerializeField] GameObject indicatorPrefab = null;
    AreaIndicator areaIndicator = null;

    [SerializeField] GameObject spellRangePrefab = null;
    AreaIndicator spellRangeInstance = null;

    [SerializeField] GameObject damageRadiusPrefab = null;
    [SerializeField] GameObject spellPrefab = null;
    
    [SerializeField] NetworkAnimator networkAnimator = null;

    [SerializeField] ActionLocker actionLocker = null;

    [SerializeField] float maxRange = 2f;
    [SerializeField] float damage = 50f;

    [SerializeField] float damageRadius = 1f;
    [SerializeField] float delayTime = 1f;
    [SerializeField] int priority = 1;

    #region Server

    [Server]
    IEnumerator CastSpell(AbilityData abilityData)
    {
        NetworkAreaIndicator damageRadiusInstance = Instantiate(damageRadiusPrefab, abilityData.castPos, Quaternion.identity).GetComponent<NetworkAreaIndicator>();
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
        yield return new WaitForSeconds(0.5f);
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
        areaIndicator = Instantiate(indicatorPrefab).GetComponent<AreaIndicator>();
        spellRangeInstance = Instantiate(spellRangePrefab).GetComponent<AreaIndicator>();

        areaIndicator.SetRadius(damageRadius);
        spellRangeInstance.SetRadius(maxRange);
        HideIndicator();
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
            networkAnimator.SetTrigger("abilityA");

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

    // Animation Event
    private void AttackPoint()
    {

    }

    // Animation Event
    private void AttackBackSwing()
    {
        actionLocker.ReleaseLock(this);
    }

    #endregion
}
