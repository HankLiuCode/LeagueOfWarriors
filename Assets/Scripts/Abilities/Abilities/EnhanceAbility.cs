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

    [SerializeField] string animationTrigger = "abilityE";


    #region Client
    public override void Cast(AbilityData abilityData)
    {
        bool canDo = actionLocker.TryGetLock(this);
        if (canDo)
        {
            networkAnimator.SetTrigger(animationTrigger);

            transform.LookAt(abilityData.mousePos, Vector3.up);
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
