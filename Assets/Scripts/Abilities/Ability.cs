using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Dota.Core;

public abstract class Ability : NetworkBehaviour, IAction
{
    public const int ABILITY_ACTION_PRIORITY = 1;

    [SyncVar]
    [SerializeField] 
    float manaCost = 10f;

    [SyncVar]
    [SerializeField] 
    float cooldownTime = 2f;

    [SerializeField] 
    private bool smartCast;

    public bool SmartCast { get { return smartCast; } set { smartCast = value; } }

    public float GetCooldownTime()
    {
        return cooldownTime;
    }

    public float GetManaCost()
    {
        return manaCost;
    }


    #region Client
    public abstract void ShowIndicator();

    public abstract void UpdateIndicator(AbilityData abilityData);

    public abstract void HideIndicator();

    public abstract void Cast(AbilityData abilityData);

    public int GetPriority()
    {
        return ABILITY_ACTION_PRIORITY;
    }

    public void Begin()
    {
        
    }

    public void End()
    {
        
    }
    #endregion
}
