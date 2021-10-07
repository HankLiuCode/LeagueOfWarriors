using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    public const float ACCEPTABLE_LATENCY = 0.2f;
    public const float DESTROY_EPSILON = 0.1f;

    [SyncVar]
    [SerializeField] 
    CombatTarget target;

    NetworkIdentity owner;

    [SerializeField] Vector3 spawnPosition;
    [SerializeField] float speed = 5f;
    float damage = 10f;

    #region Server

    public void SetOwner(NetworkIdentity owner)
    {
        this.owner = owner;
    }

    public void SetTarget(CombatTarget target, float damage, Vector3 spawnPosition)
    {
        this.target = target;
        this.spawnPosition = spawnPosition;
    }

    #endregion


    private void Update()
    {
        Vector3 targetVec = target.GetAimPoint().position - transform.position;
        Vector3 direction = targetVec.normalized;
        if (isServer)
        {
            float delta = speed * Time.deltaTime;
            transform.position += direction * delta;
            if (targetVec.magnitude <= DESTROY_EPSILON)
            {
                target.GetHealth().ServerTakeDamage(damage, netIdentity);
                NetworkServer.Destroy(gameObject);
            }
        }
    }
}
