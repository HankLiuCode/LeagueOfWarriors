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

    Ability currentAbility = null;
    AbilityData abilityData = null;

    public override void OnStartAuthority()
    {
        abilityData = new AbilityData();
    }


    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority) { return; }

        if (health.IsDead()) { return; }

        if (Physics.Raycast(DotaPlayerController.GetMouseRay(), out RaycastHit groundHit, Mathf.Infinity, groundMask))
        {
            Vector3 castPosition = new Vector3(groundHit.point.x, 0, groundHit.point.z);
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

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ChangeAbility(abilityQ);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            ChangeAbility(abilityW);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ChangeAbility(abilityE);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ChangeAbility(abilityR);
        }

        if(currentAbility != null)
        {
            currentAbility.UpdateIndicator(abilityData);

            if (Input.GetMouseButtonDown(0))
            {
                if (!currentAbility.SmartCast)
                {
                    currentAbility.HideIndicator();
                    currentAbility.Cast(abilityData);
                    currentAbility = null;
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                currentAbility.HideIndicator();
                currentAbility = null;
            }
        }
    }

    private void ChangeAbility(Ability ability)
    {
        if (currentAbility != null)
            currentAbility.HideIndicator();

        currentAbility = ability;

        if (currentAbility.SmartCast)
            currentAbility.Cast(abilityData);
        else
            currentAbility.ShowIndicator();
    }
}