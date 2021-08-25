using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public abstract class Ability : NetworkBehaviour
{

    [SerializeField] float manaCost = 10f;
    [SerializeField] float cooldownTime = 2f;
    [SerializeField] private bool smartCast;

    public bool SmartCast { get { return smartCast; } set { smartCast = value; } }


    #region Client

    public abstract void ShowIndicator();

    public abstract void UpdateIndicator(AbilityData abilityData);

    public abstract void HideIndicator();

    public abstract void Cast(AbilityData abilityData);

    #endregion
}
