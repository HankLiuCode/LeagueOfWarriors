using Dota.Core;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalAbility : Ability, IAction
{
    [SerializeField] GameObject indicatorPrefab = null;
    RectIndicator directionIndicatorInstance = null;

    [SerializeField] GameObject damageRectPrefab = null;
    [SerializeField] GameObject damageRectInstance = null;

    [SerializeField] GameObject spellPrefab = null;
    [SerializeField] float spellEffectOffset = 2.0f;

    [SerializeField] NetworkAnimator networkAnimator = null;
    [SerializeField] AnimationEventHandler animationEventHandler = null;
    [SerializeField] string animationTrigger = "abilityQ";

    [SerializeField] ActionLocker actionLocker = null;

    [SerializeField] float damage = 50f;
    [SerializeField] float length = 5f;
    [SerializeField] float width = 2f;

    [SerializeField] float delayTime = 1f;
    [SerializeField] float destroyTime = 1f;
    [SerializeField] int priority = 1;

    #region Server

    [Server]
    IEnumerator CastSpell(AbilityData abilityData)
    {
        Vector3 direction = (abilityData.mouseClickPos - abilityData.casterPos).normalized;

        NetworkRectIndicator damageRectInstance = Instantiate(damageRectPrefab, abilityData.casterPos, Quaternion.identity).GetComponent<NetworkRectIndicator>();
        
        damageRectInstance.ServerSetPosition(abilityData.casterPos);
        damageRectInstance.ServerSetDirection(direction);
        damageRectInstance.ServerSetLength(length);
        damageRectInstance.ServerSetWidth(width);

        NetworkServer.Spawn(damageRectInstance.gameObject);

        yield return new WaitForSeconds(abilityData.delayTime);

        GameObject effectInstance = Instantiate(spellPrefab, abilityData.casterPos + direction * spellEffectOffset, Quaternion.identity);

        effectInstance.transform.rotation = Quaternion.LookRotation(direction);

        NetworkServer.Spawn(effectInstance, connectionToClient);

        yield return new WaitForSeconds(destroyTime);

        // Deal Damage to health in rect
        //Collider[] colliders = Physics.OverlapSphere(abilityData.castPos, damageRadius);
        //foreach (Collider c in colliders)
        //{
        //    GameObject go = c.gameObject;
        //    Health health = go.GetComponent<Health>();
        //    if (health)
        //    {
        //        health.ServerTakeDamage(damage);
        //    }
        //}

        NetworkServer.Destroy(effectInstance);
        NetworkServer.Destroy(damageRectInstance.gameObject);
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
        directionIndicatorInstance = Instantiate(indicatorPrefab).GetComponent<RectIndicator>();
        directionIndicatorInstance.SetLength(length);
        directionIndicatorInstance.gameObject.SetActive(false);

        animationEventHandler.OnAttackBackswing += AnimationEventHandler_OnAttackBackswing;
        animationEventHandler.OnAttackPoint += AnimationEventHandler_OnAttackPoint;
    }

    public override void OnStopClient()
    {
        if (!hasAuthority) { return; }
        Debug.Log("Directional Ability OnStopAuthority");
        Destroy(directionIndicatorInstance.gameObject);
        animationEventHandler.OnAttackBackswing -= AnimationEventHandler_OnAttackBackswing;
        animationEventHandler.OnAttackPoint -= AnimationEventHandler_OnAttackPoint;
    }

    [Client]
    public override void ShowIndicator()
    {
        directionIndicatorInstance.gameObject.SetActive(true);
    }
    [Client]
    public override void UpdateIndicator(AbilityData abilityData)
    {
        directionIndicatorInstance.SetPosition(abilityData.casterPos);
        directionIndicatorInstance.SetDirection(abilityData.mouseClickPos - abilityData.casterPos);
    }

    [Client]
    public override void HideIndicator()
    {
        directionIndicatorInstance.gameObject.SetActive(false);
    }

    [Client]
    public override void Cast(AbilityData abilityData)
    {
        bool canDo = actionLocker.TryGetLock(this);
        if (canDo)
        {
            networkAnimator.SetTrigger(animationTrigger);

            transform.LookAt(abilityData.mouseClickPos, Vector3.up);

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
