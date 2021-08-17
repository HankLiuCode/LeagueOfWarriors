using Dota.Controls;
using Dota.Core;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillShotAbility : NetworkBehaviour
{
    [SerializeField] GameObject indicatorPrefab = null;
    DirectionIndicator directionIndicator = null;

    [SerializeField] LayerMask groundMask = new LayerMask();
    [SerializeField] Health health = null;

    [SerializeField] float damage = 50f;

    [SerializeField] float delayTime = 1f;


    #region Server
    [Server]
    public void ServerSpawnAbilityEffect(Vector3 direction)
    {
        StartCoroutine(CastSpell(direction));
    }

    [Server]
    IEnumerator CastSpell(Vector3 direction)
    {
        yield return null;

        //NetworkAreaIndicator damageRadiusInstance = Instantiate(damageRadiusPrefab, position, Quaternion.identity).GetComponent<NetworkAreaIndicator>();
        //NetworkServer.Spawn(damageRadiusInstance.gameObject);
        //damageRadiusInstance.ServerSetPosition(position);
        //damageRadiusInstance.ServerSetRadius(damageRadius);

        //yield return new WaitForSeconds(delayTime);

        //GameObject effectInstance = Instantiate(spellPrefab, position, Quaternion.identity);
        //NetworkServer.Spawn(effectInstance, connectionToClient);

        //Collider[] colliders = Physics.OverlapSphere(position, damageRadius);
        //foreach (Collider c in colliders)
        //{
        //    GameObject go = c.gameObject;
        //    Health health = go.GetComponent<Health>();
        //    if (health)
        //    {
        //        health.ServerTakeDamage(damage);
        //    }
        //}
        //yield return new WaitForSeconds(0.5f);
        //NetworkServer.Destroy(effectInstance);
        //NetworkServer.Destroy(damageRadiusInstance.gameObject);
    }

    [Command]
    public void CmdSpawnAbilityEffect(Vector3 position)
    {
        ServerSpawnAbilityEffect(position);
    }
    #endregion


    #region Client
    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority) { return; }

        if (health.IsDead()) { return; }

        if (Input.GetKeyDown(KeyCode.W))
        {
            StartCoroutine(ShowSpellUI());
        }
    }

    [Client]
    IEnumerator ShowSpellUI()
    {
        directionIndicator = directionIndicator ?? Instantiate(indicatorPrefab).GetComponent<DirectionIndicator>();
        directionIndicator.gameObject.SetActive(true);
        directionIndicator.SetLength(5f);

        while (true)
        {
            if (Physics.Raycast(DotaPlayerController.GetMouseRay(), out RaycastHit hit, Mathf.Infinity, groundMask))
            {
                directionIndicator.SetPosition(transform.position);
                directionIndicator.SetDirection(hit.point - transform.position);

                if (Input.GetMouseButtonDown(0))
                {
                    directionIndicator.gameObject.SetActive(false);
                    //CmdSpawnAbilityEffect(castPosition);
                    break;
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                directionIndicator.gameObject.SetActive(false);
                break;
            }
            yield return null;
        }

    }
    #endregion
}
