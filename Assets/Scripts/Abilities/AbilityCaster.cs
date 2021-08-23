using Dota.Controls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Dota.Core;

public class AbilityCaster : NetworkBehaviour
{
    [SerializeField] Health health = null;
    [SerializeField] AreaAbility areaAbility;
    [SerializeField] LayerMask groundMask = new LayerMask();

    AreaAbility currentAbility = null;
    AbilityData abilityData = new AbilityData();


    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority) { return; }

        if (health.IsDead()) { return; }

        if (Input.GetKeyDown(KeyCode.W))
        {
            currentAbility = areaAbility;
            currentAbility.ShowIndicator();
        }

        if (Physics.Raycast(DotaPlayerController.GetMouseRay(), out RaycastHit hit, Mathf.Infinity, groundMask))
        {
            Vector3 castPosition = hit.point;

            abilityData.casterPos = transform.position;
            abilityData.mouseClickPos = castPosition;
        }

        if(currentAbility == null) { return; }

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
        }
    }
}
