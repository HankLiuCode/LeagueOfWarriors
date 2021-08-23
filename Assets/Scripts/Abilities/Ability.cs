using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public abstract class Ability : NetworkBehaviour
{


    #region Client

    public abstract void ShowIndicator();

    public abstract void UpdateIndicator(AbilityData abilityData);

    public abstract void HideIndicator();

    public abstract void Cast(AbilityData abilityData);

    #endregion
}
