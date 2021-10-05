using Dota.Core;
using Dota.Utils;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalAbility : Ability
{
    [SerializeField] GameObject indicatorPrefab = null;
    RectIndicator directionIndicatorInstance = null;

    [SerializeField] GameObject damageRectPrefab = null;

    [SerializeField] GameObject spellPrefab = null;
    [SerializeField] float spellEffectOffset = 2.0f;

    [SerializeField] NetworkAnimator networkAnimator = null;
    [SerializeField] AnimationEventHandler animationEventHandler = null;
    [SerializeField] string animationTrigger = "abilityQ";

    [SerializeField] ActionLocker actionLocker = null;

    [SerializeField] LayerMask attackLayer;
    
    [SerializeField] float baseDamage = 50f;
    [SerializeField] float length = 5f;
    [SerializeField] float width = 2f;

    [SerializeField] float delayTime = 1f;
    [SerializeField] float destroyTime = 1f;

    GameObject damageRectInstance;

    #region Server

    [Server]
    IEnumerator CastSpell(AbilityData abilityData)
    {
        Vector3 direction = (abilityData.mousePos - abilityData.casterPos).normalized;

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

        RaycastHit[] raycastHits = Physics.SphereCastAll(transform.position, width / 2, direction, length - width / 2, attackLayer);
        foreach (RaycastHit hit in raycastHits)
        {
            GameObject go = hit.collider.gameObject;
            CombatTarget combatTarget = go.GetComponent<CombatTarget>();
            if (combatTarget && !TeamChecker.IsSameTeam(gameObject, combatTarget.gameObject))
            {
                float damage = baseDamage + statStore.GetStats().attackDamage;
                
                combatTarget.GetHealth().ServerTakeDamage(damage, abilityData.caster.netIdentity);
            }
        }

        yield return new WaitForSeconds(destroyTime);

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
        directionIndicatorInstance.SetWidth(width);
        directionIndicatorInstance.gameObject.SetActive(false);

        animationEventHandler.OnAttackBackswing += AnimationEventHandler_OnAttackBackswing;
        animationEventHandler.OnAttackPoint += AnimationEventHandler_OnAttackPoint;
    }

    public override void OnStopClient()
    {
        if (!hasAuthority) { return; }
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
        directionIndicatorInstance.SetDirection(abilityData.mousePos - abilityData.casterPos);
    }

    [Client]
    public override void HideIndicator()
    {
        directionIndicatorInstance.gameObject.SetActive(false);
    }

    [Client]
    public override void ClientCast(AbilityData abilityData)
    {
        bool canDo = actionLocker.TryGetLock(this);
        if (canDo)
        {
            networkAnimator.SetTrigger(animationTrigger);

            transform.LookAt(abilityData.mousePos, Vector3.up);

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
