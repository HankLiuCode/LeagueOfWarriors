using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Dota.Core;

public class SkillShot : NetworkBehaviour 
{
    [SerializeField] float speed = 8;
    [SerializeField] Vector3 direction;
    [SerializeField] float damage = 50;
    Vector3 startPos;
    Vector3 destroyPoint;
    GameObject owner;

    // OnTriggerEnter IsSometimes called twice or more, this prevents it
    bool hasHit;


    #region Server

    [Server]
    public void ServerDealDamageTo(Health health, float damage)
    {
        health.ServerTakeDamage(damage);
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
    public void ServerSetOwner(GameObject owner)
    {
        this.owner = owner;
    }

    [ServerCallback]
    private void Update()
    {
        transform.forward = destroyPoint - transform.position;
        transform.position += direction * speed * Time.deltaTime;

        if(Vector3.Distance(startPos, destroyPoint) < Vector3.Distance(startPos, transform.position))
        {
            NetworkServer.Destroy(gameObject);
        }
    }
    
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == owner) { return; }

        Health health = other.GetComponent<Health>();
        if (health && !hasHit)
        {
            hasHit = true;
            health.ServerTakeDamage(damage);
            NetworkServer.Destroy(gameObject);
        }
    }
    #endregion
}
