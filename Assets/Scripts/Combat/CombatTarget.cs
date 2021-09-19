using Dota.Attributes;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatTarget : NetworkBehaviour
{
    [SerializeField] float allowAttackRadius = 1f;
    [SerializeField] Health health = null;
    [SerializeField] Transform aimPoint = null;
    
    public Transform GetAimPoint()
    {
        return aimPoint;
    }

    public Health GetHealth()
    {
        return health;
    }

    public float GetAllowAttackRadius()
    {
        return allowAttackRadius;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, allowAttackRadius);
    }
}
