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

    [SerializeField] GameObject skillShotPrefab = null;
    [SerializeField] Vector3 castOffset = Vector3.up * 0.5f;

    [SerializeField] float damage = 50f;
    [SerializeField] float travelDist = 10f;

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
        yield return new WaitForSeconds(0.1f);

        Vector3 castPos = transform.position + castOffset;
        SkillShot skillShotInstance = Instantiate(skillShotPrefab, castPos, Quaternion.identity).GetComponent<SkillShot>();
        NetworkServer.Spawn(skillShotInstance.gameObject);
        skillShotInstance.ServerSetDirection(castPos, direction, travelDist);
        skillShotInstance.ServerSetOwner(gameObject);

        yield return null;
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
        directionIndicator = directionIndicator ?? Instantiate(indicatorPrefab).GetComponent<DirectionIndicator>();
        directionIndicator.gameObject.SetActive(true);
        directionIndicator.SetLength(travelDist);

        while (true)
        {
            if (Physics.Raycast(DotaPlayerController.GetMouseRay(), out RaycastHit hit, Mathf.Infinity, groundMask))
            {
                directionIndicator.SetPosition(transform.position);
                directionIndicator.SetDirection(hit.point - transform.position);

                if (Input.GetMouseButtonDown(0))
                {
                    directionIndicator.gameObject.SetActive(false);
                    CmdSpawnAbilityEffect(directionIndicator.GetDirection());
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
