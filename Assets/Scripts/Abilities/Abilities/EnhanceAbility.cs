using Dota.Attributes;
using Dota.Core;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnhanceAbility : Ability
{
    [SerializeField] NetworkAnimator networkAnimator = null;
    [SerializeField] AnimationEventHandler animationEventHandler = null;
    [SerializeField] ActionLocker actionLocker = null;
    [SerializeField] Health health = null;
    [SerializeField] float baseHeal = 10f;
    [SerializeField] string animationTrigger = "abilityE";

    [SerializeField] float baseArmorBuff = 10f;
    [SerializeField] GameObject spellPrefab = null;
    [SerializeField] float destroyTime = 2f;
    [SerializeField] float castHeight = 2f;


    #region Server

    [Server]
    IEnumerator CastSpell(AbilityData abilityData)
    {
        GameObject effectInstance = Instantiate(spellPrefab, abilityData.caster.transform.position + Vector3.up * castHeight, Quaternion.identity);
        NetworkServer.Spawn(effectInstance);
        statStore.AddStats(new Stats() { armor = baseArmorBuff });

        float timer = 0;
        while(timer < destroyTime)
        {
            timer += Time.deltaTime;
            effectInstance.transform.position = abilityData.caster.transform.position + Vector3.up * castHeight;
            yield return null;
        }

        statStore.RemoveStats(new Stats() { armor = baseArmorBuff });
        NetworkServer.Destroy(effectInstance);
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
    public override void ClientCast(AbilityData abilityData)
    {
        bool canDo = actionLocker.TryGetLock(this);
        if (canDo)
        {
            networkAnimator.SetTrigger(animationTrigger);
            health.CmdHeal(baseHeal + statStore.GetStats().magicDamage);
            CmdSpawnAbilityEffect(abilityData);
        }
    }

    public override void OnStartAuthority()
    {
        animationEventHandler.OnAttackBackswing += AnimationEventHandler_OnAttackBackswing;
    }

    private void AnimationEventHandler_OnAttackBackswing()
    {
        actionLocker.ReleaseLock(this);
    }

    public override void ShowIndicator()
    {

    }

    public override void UpdateIndicator(AbilityData abilityData)
    {

    }

    public override void HideIndicator()
    {
        
    }
    #endregion
}
