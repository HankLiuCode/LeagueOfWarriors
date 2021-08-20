using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Dota.Core;

public class SkillShot : NetworkBehaviour
{
    [SyncVar]
    [SerializeField] 
    float speed = 8;
    
    [SyncVar]
    [SerializeField] 
    float damage = 50;

    [SyncVar]
    [SerializeField] 
    Vector3 direction;

    [SyncVar]
    [SerializeField]
    Vector3 startPos;

    [SyncVar]
    [SerializeField]
    Vector3 destroyPoint;

    [SyncVar]
    [SerializeField]
    NetworkIdentity owner;

    // OnTriggerEnter IsSometimes called twice or more, this prevents it
    bool hasHit;


    #region Server

    [Server]
    public void ServerDealDamageTo(Health health, float damage)
    {
        health.ServerTakeDamage(damage);
        NetworkServer.Destroy(gameObject);
    }

    [Command]
    public void CmdDealDamageTo(Health health, float damage)
    {
        ServerDealDamageTo(health, damage);
    }

    [Command]
    private void CmdDestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Server]
    public void ServerSetDirection(Vector3 startPos, Vector3 direction, float travelDist)
    {
        this.direction = new Vector3(direction.normalized.x, 0, direction.normalized.z);
        this.startPos = startPos;
        destroyPoint = startPos + this.direction.normalized * travelDist;
    }

    [Server]
    public void ServerSetOwner(NetworkIdentity owner)
    {
        this.owner = owner;
    }

    [Server]
    public void ServerSetDamage(float damage)
    {
        this.damage = damage;
    }

    [Server]
    public void ServerSetSpeed(float speed)
    {
        this.speed = speed;
    }

    #endregion

    #region Client
    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority) { return; }

        transform.forward = destroyPoint - transform.position;
        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(startPos, destroyPoint) < Vector3.Distance(startPos, transform.position))
        {
            CmdDestroySelf();
        }
    }

    [ClientCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (!hasAuthority) { return; }

        NetworkIdentity otherIdentity = other.gameObject.GetComponent<NetworkIdentity>();
        Health health = other.GetComponent<Health>();
        if (health && !hasHit && otherIdentity != owner)
        {
            if (health.IsDead()) { return; }

            hasHit = true;
            CmdDealDamageTo(health, damage);
        }
    }
    #endregion
}