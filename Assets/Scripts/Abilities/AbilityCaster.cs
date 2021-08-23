using Dota.Controls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Dota.Core;
public class AbilityData
{
    public Vector3 casterPos;
    public Vector3 mouseClickPos;
    public Vector3 castPos;
    public Transform target;
    public float delayTime;
}

public class AbilityCaster : NetworkBehaviour
{
    [SerializeField] Health health = null;
    [SerializeField] Ability abilityQ;
    [SerializeField] Ability abilityW;
    [SerializeField] Ability abilityE;
    [SerializeField] Ability abilityR;
    [SerializeField] LayerMask groundMask = new LayerMask();
    [SerializeField] LayerMask playerMask = new LayerMask();

    Ability currentAbility = null;
    AbilityData abilityData = new AbilityData();


    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority) { return; }

        if (health.IsDead()) { return; }



        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentAbility?.HideIndicator();
            currentAbility = abilityQ;
            currentAbility.ShowIndicator();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            currentAbility?.HideIndicator();
            currentAbility = abilityW;
            currentAbility.ShowIndicator();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            currentAbility?.HideIndicator();
            currentAbility = abilityE;
            currentAbility.ShowIndicator();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            currentAbility?.HideIndicator();
            currentAbility = abilityR;
            currentAbility.ShowIndicator();
        }

        if (Physics.Raycast(DotaPlayerController.GetMouseRay(), out RaycastHit groundHit, Mathf.Infinity, groundMask))
        {
            Vector3 castPosition = groundHit.point;
            abilityData.casterPos = transform.position;
            abilityData.mouseClickPos = castPosition;
        }

        if (Physics.Raycast(DotaPlayerController.GetMouseRay(), out RaycastHit playerHit, Mathf.Infinity))
        {
            Health target = playerHit.collider.gameObject.GetComponent<Health>();
            if (target && !target.IsDead())
            {
                abilityData.target = target.transform;
            }
            else
            {
                abilityData.target = null;
            }
        }

        if (currentAbility == null) { return; }

        currentAbility.UpdateIndicator(abilityData);

        if (Input.GetMouseButtonDown(0))
        {
            currentAbility.HideIndicator();
            currentAbility.Cast(abilityData);
            currentAbility = null;
        }

        if (Input.GetMouseButtonDown(1))
        {
            currentAbility.HideIndicator();
            currentAbility = null;
        }
    }
}
