using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public abstract class Ability : NetworkBehaviour
{

    [SyncVar]
    [SerializeField] 
    float manaCost = 10f;

    [SyncVar]
    [SerializeField] 
    float cooldownTime = 2f;

    [SerializeField] 
    private bool smartCast;

    public bool SmartCast { get { return smartCast; } set { smartCast = value; } }

    public float GetManaCost()
    {
        return manaCost;
    }



    #region Client

    public abstract void ShowIndicator();

    public abstract void UpdateIndicator(AbilityData abilityData);

    public abstract void HideIndicator();

    public abstract void Cast(AbilityData abilityData);

    #endregion
}
