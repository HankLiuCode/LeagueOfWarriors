using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dota.Controls;
using Dota.Core;

public class AbilityCaster : NetworkBehaviour
{
    [SerializeField] GameObject preparePrefab = null;
    [SerializeField] GameObject spellPrefab = null;
    [SerializeField] LayerMask groundMask = new LayerMask();
    [SerializeField] Health health = null;

    [SerializeField] GameObject spellUIPrefab = null;
    
    GameObject spellUIInstance = null;

    [SerializeField] float range = 2f;
    [SerializeField] float damage = 50f;

    [SerializeField] float damageRadius = 1f;
    [SerializeField] float delayTime = 0.5f;


    #region Server
    [Server]
    public void ServerSpawnAbilityEffect(Vector3 position)
    {
        StartCoroutine(CastSpell(position));
    }

    [Server]
    IEnumerator CastSpell(Vector3 position)
    {
        GameObject prepareInstance = Instantiate(preparePrefab, position, Quaternion.identity);
        NetworkServer.Spawn(prepareInstance, connectionToClient);

        yield return new WaitForSeconds(delayTime);

        NetworkServer.Destroy(prepareInstance);

        GameObject effectInstance = Instantiate(spellPrefab, position, Quaternion.identity);
        NetworkServer.Spawn(effectInstance, connectionToClient);

        Collider[] colliders = Physics.OverlapSphere(position, damageRadius);
        foreach (Collider c in colliders)
        {
            GameObject go = c.gameObject;
            Health health = go.GetComponent<Health>();
            if (health)
            {
                health.ServerTakeDamage(damage);
            }
        }

        yield return new WaitForSeconds(1.0f);
        NetworkServer.Destroy(effectInstance);
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

        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(ShowSpellUI());
        }
    }

    [Client]
    IEnumerator ShowSpellUI()
    {
        if(spellUIInstance == null)
        {
            spellUIInstance = Instantiate(spellUIPrefab);
        }
        else
        {
            spellUIInstance.SetActive(true);
        }


        while (true)
        {
            if(Physics.Raycast(DotaPlayerController.GetMouseRay(), out RaycastHit hit, Mathf.Infinity, groundMask))
            {
                spellUIInstance.transform.position = hit.point;
                spellUIInstance.transform.localScale = new Vector3(damageRadius, 1, damageRadius);
            }

            if (Input.GetMouseButtonDown(0))
            {
                spellUIInstance.SetActive(false);
                CmdSpawnAbilityEffect(hit.point);
                break;
            }

            if (Input.GetMouseButtonDown(1))
            {
                spellUIInstance.SetActive(false);
                break;
            }
            yield return null;
        }

    }
    #endregion
}
