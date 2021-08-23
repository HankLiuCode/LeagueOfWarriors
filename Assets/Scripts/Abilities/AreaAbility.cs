using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dota.Controls;
using Dota.Core;


public class AbilityData
{
    public Vector3 casterPos;
    public Vector3 mouseClickPos;
    public Vector3 castPos;
    public float delayTime;
}

public class AreaAbility : NetworkBehaviour, IAction, IAbility
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

    AbilityData abilityData;

    #region Server

    [Server]
    IEnumerator CastSpell(AbilityData abilityData)
    {
        NetworkAreaIndicator damageRadiusInstance = Instantiate(damageRadiusPrefab, abilityData.castPos, Quaternion.identity).GetComponent<NetworkAreaIndicator>();
        NetworkServer.Spawn(damageRadiusInstance.gameObject);
        damageRadiusInstance.ServerSetPosition(abilityData.castPos);
        damageRadiusInstance.ServerSetRadius(damageRadius);

        yield return new WaitForSeconds(abilityData.delayTime);

        GameObject effectInstance = Instantiate(spellPrefab, abilityData.castPos, Quaternion.identity);
        NetworkServer.Spawn(effectInstance, connectionToClient);

        Collider[] colliders = Physics.OverlapSphere(abilityData.castPos, damageRadius);
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
    public void ServerSpawnAbilityEffect(AbilityData abilityData)
    {
        StartCoroutine(CastSpell(abilityData));
    }

    [Command]
    public void CmdSpawnAbilityEffect(AbilityData abilityData)
    {
        ServerSpawnAbilityEffect(abilityData);
    }
    #endregion

    #region Client
    public override void OnStartAuthority()
    {
        abilityData = new AbilityData();
        areaIndicator = Instantiate(indicatorPrefab).GetComponent<AreaIndicator>();
        spellRangeInstance = Instantiate(spellRangePrefab).GetComponent<AreaIndicator>();

        areaIndicator.SetRadius(damageRadius);
        spellRangeInstance.SetRadius(maxRange);
        HideIndicator();
    }

    [Client]
    public void ShowIndicator()
    {
        areaIndicator.gameObject.SetActive(true);
        spellRangeInstance.gameObject.SetActive(true);
    }

    [Client]
    public void UpdateIndicator(AbilityData abilityData)
    {
        spellRangeInstance.SetPosition(abilityData.casterPos);
        Vector3 direction = (abilityData.mouseClickPos - abilityData.casterPos).normalized;
        float range = (abilityData.mouseClickPos - abilityData.casterPos).magnitude;
        Vector3 castPosition = transform.position + direction * Mathf.Min(maxRange, range);
        
        abilityData.castPos = castPosition;

        areaIndicator.SetPosition(castPosition);
    }

    [Client]
    public void HideIndicator()
    {
        areaIndicator.gameObject.SetActive(false);
        spellRangeInstance.gameObject.SetActive(false);
    }

    [Client]
    public void Cast(AbilityData abilityData)
    {
        bool canDo = actionLocker.TryGetLock(this);
        if (canDo)
        {
            HideIndicator();

            networkAnimator.SetTrigger("abilityA");

            transform.LookAt(abilityData.castPos, Vector3.up);

            abilityData.delayTime = delayTime;

            CmdSpawnAbilityEffect(abilityData);
        }
    }

    [ClientCallback]
    private void Update()
    {
        //if (!hasAuthority) { return; }

        //if (health.IsDead()) { return; }

        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    StartCoroutine(ShowSpellUI(abilityData));
        //}
    }

    //[Client]
    //IEnumerator ShowSpellUI(AbilityData abilityData)
    //{
    //    ShowIndicator();

    //    while (true)
    //    {
    //        spellRangeInstance.SetPosition(transform.position);

    //        if (Physics.Raycast(DotaPlayerController.GetMouseRay(), out RaycastHit hit, Mathf.Infinity, groundMask))
    //        {
    //            Vector3 direction = (hit.point - transform.position).normalized;
    //            float range = (hit.point - transform.position).magnitude;

    //            Vector3 castPosition = transform.position + direction * Mathf.Min(maxRange, range);

    //            areaIndicator.SetPosition(castPosition);

    //            if (Input.GetMouseButtonDown(0))
    //            {
    //                bool canDo = actionLocker.TryGetLock(this);
    //                if (canDo)
    //                {
    //                    HideIndicator();

    //                    networkAnimator.SetTrigger("abilityA");

    //                    transform.LookAt(castPosition, Vector3.up);

    //                    abilityData.mouseClickPos = castPosition;
    //                    abilityData.delayTime = delayTime;

    //                    CmdSpawnAbilityEffect(abilityData);
    //                    break;
    //                }
    //            }
    //        }

    //        if (Input.GetMouseButtonDown(1))
    //        {
    //            HideIndicator();
    //            break;
    //        }
    //        Debug.Log("Area Ability");
    //        yield return null;
    //    }
    //}

    public void Begin()
    {
        
    }

    public void End()
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
