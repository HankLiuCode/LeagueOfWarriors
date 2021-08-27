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
    [SerializeField] Mana mana = null;
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

        if (Physics.Raycast(CameraController.GetMouseRay(), out RaycastHit groundHit, Mathf.Infinity, groundMask))
        {
            Vector3 castPosition = new Vector3(groundHit.point.x, 0, groundHit.point.z);
            abilityData.casterPos = transform.position;
            abilityData.mouseClickPos = castPosition;
        }

        if (Physics.Raycast(CameraController.GetMouseRay(), out RaycastHit playerHit, Mathf.Infinity))
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
            ChangeOrCastAbility(abilityQ);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            ChangeOrCastAbility(abilityW);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ChangeOrCastAbility(abilityE);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ChangeOrCastAbility(abilityR);
        }

        if(currentAbility != null)
        {
            currentAbility.UpdateIndicator(abilityData);

            if (Input.GetMouseButtonDown(0))
            {
                if (!currentAbility.SmartCast)
                {
                    currentAbility.HideIndicator();
                    mana.ClientUseMana(currentAbility.GetManaCost());
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

    private void ChangeOrCastAbility(Ability ability)
    {
        if (currentAbility != null)
            currentAbility.HideIndicator();

        if (!mana.IsManaEnough(ability.GetManaCost())) return;

        currentAbility = ability;

        if (currentAbility.SmartCast)
        {
            mana.ClientUseMana(currentAbility.GetManaCost());
            currentAbility.Cast(abilityData);
        }
        else
        {
            currentAbility.ShowIndicator();
        }
    }
}