using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dota.Controls;
using Dota.Core;

public class AreaAbility : NetworkBehaviour, IAction
{
    [SerializeField] GameObject indicatorPrefab = null;
    AreaIndicator areaIndicator = null;

    [SerializeField] GameObject spellRangePrefab = null;
    AreaIndicator spellRangeInstance = null;

    [SerializeField] GameObject damageRadiusPrefab = null;
    [SerializeField] GameObject spellPrefab = null;
    
    [SerializeField] NetworkAnimator networkAnimator = null;

    [SerializeField] ActionLocker actionLocker = null;

    [SerializeField] LayerMask groundMask = new LayerMask();
    [SerializeField] Health health = null;

    [SerializeField] float maxRange = 2f;
    [SerializeField] float damage = 50f;

    [SerializeField] float damageRadius = 1f;
    [SerializeField] float delayTime = 1f;

    [SerializeField] int priority = 1;


    bool hasFinishedBackswing = true;


    #region Server

    [Server]
    IEnumerator CastSpell(Vector3 position, float delayTime)
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

    [Server]
    public void ServerSpawnAbilityEffect(Vector3 position, float delayTime)
    {
        StartCoroutine(CastSpell(position, delayTime));
    }

    [Command]
    public void CmdSpawnAbilityEffect(Vector3 position, float delayTime)
    {
        ServerSpawnAbilityEffect(position, delayTime);
    }

    #endregion


    #region Client

    public override void OnStartAuthority()
    {
        areaIndicator = Instantiate(indicatorPrefab).GetComponent<AreaIndicator>();
        spellRangeInstance = Instantiate(spellRangePrefab).GetComponent<AreaIndicator>();
        areaIndicator.SetRadius(damageRadius);
        spellRangeInstance.SetRadius(maxRange);
        areaIndicator.gameObject.SetActive(false);
        spellRangeInstance.gameObject.SetActive(false);
    }

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
        areaIndicator.gameObject.SetActive(true);
        spellRangeInstance.gameObject.SetActive(true);

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
                    bool canDo = actionLocker.TryGetLock(this);
                    if (canDo)
                    {
                        areaIndicator.gameObject.SetActive(false);
                        spellRangeInstance.gameObject.SetActive(false);

                        networkAnimator.SetTrigger("abilityA");

                        transform.LookAt(castPosition, Vector3.up);

                        CmdSpawnAbilityEffect(castPosition, delayTime);
                        break;
                    }
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
    public void Stop()
    {

    }

    public int GetPriority()
    {
        return priority;
    }

    // Animation Event
    private void AttackPoint()
    {

    }

    // Animation Event
    private void AttackBackSwing()
    {
        actionLocker.ReleaseLock(this);
    }

    #endregion
}
