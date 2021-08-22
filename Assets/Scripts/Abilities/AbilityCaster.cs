using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AbilityCaster : NetworkBehaviour
{
    [SerializeField] IAbility[] abilities = new IAbility[4];
}
