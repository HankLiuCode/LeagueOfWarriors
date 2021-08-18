using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dota.Controls;
using Dota.Core;

public class AreaAbility : NetworkBehaviour
{
    [SerializeField] GameObject indicatorPrefab = null;
    AreaIndicator areaIndicator = null;

    [SerializeField] GameObject spellRangePrefab = null;
    AreaIndicator spellRangeInstance = null;

    [SerializeField] GameObject damageRadiusPrefab = null;
    [SerializeField] GameObject spellPrefab = null;

    [SerializeField] Animator animator = null;

    [SerializeField] LayerMask groundMask = new LayerMask();
    [SerializeField] Health health = null;

    [SerializeField] float maxRange = 2f;
    [SerializeField] float damage = 50f;

    [SerializeField] float damageRadius = 1f;
    [SerializeField] float delayTime = 1f;


    #region Server
    [Server]
    public void ServerSpawnAbilityEffect(Vector3 position)
    {
        StartCoroutine(CastSpell(position));
    }

    [Server]
    IEnumerator CastSpell(Vector3 position)
    {
        NetworkAreaIndicator damageRadiusInstance = Instantiate(damageRadiusPrefab, position, Quaternion.identity).GetComponent<NetworkAreaIndicator>();
        NetworkServer.Spawn(damageRadiusInstance.gameObject);
        damageRadiusInstance.ServerSetPosition(position);
        damageRadiusInstance.ServerSetRadius(damageRadius);

        yield return new WaitForSeconds(delayTime);

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
        yield return new WaitForSeconds(0.5f);
        NetworkServer.Destroy(effectInstance);
        NetworkServer.Destroy(damageRadiusInstance.gameObject);
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
        areaIndicator = areaIndicator ?? Instantiate(indicatorPrefab).GetComponent<AreaIndicator>();
        areaIndicator.gameObject.SetActive(true);
        areaIndicator.SetRadius(damageRadius);

        spellRangeInstance = spellRangeInstance ?? Instantiate(spellRangePrefab).GetComponent<AreaIndicator>();
        spellRangeInstance.gameObject.SetActive(true);
        spellRangeInstance.SetRadius(maxRange);

        while (true)
        {
            spellRangeInstance.SetPosition(transform.position);

            if (Physics.Raycast(DotaPlayerController.GetMouseRay(), out RaycastHit hit, Mathf.Infinity, groundMask))
            {
                Vector3 direction = (hit.point - transform.position).normalized;
                float range = (hit.point - transform.position).magnitude;

                Vector3 castPosition = transform.position + direction * Mathf.Min(maxRange, range);

                areaIndicator.SetPosition(castPosition);

                if (Input.GetMouseButtonDown(0))
                {
                    areaIndicator.gameObject.SetActive(false);
                    spellRangeInstance.gameObject.SetActive(false);
                    CmdSpawnAbilityEffect(castPosition);
                    break;
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                areaIndicator.gameObject.SetActive(false);
                spellRangeInstance.gameObject.SetActive(false);
                break;
            }
            yield return null;
        }

    }
    #endregion
}
