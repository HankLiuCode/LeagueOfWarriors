using Dota.Controls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Dota.Core;
public class AbilityData
{
    public Vector3 casterPos;
    public Vector3 mousePos;
    public Vector3 castPos;
    public Transform target;
    public float delayTime;
}

public class AbilityCaster : NetworkBehaviour
{
    [SerializeField] Health health = null;
    [SerializeField] Mana mana = null;
    [SerializeField] CooldownStore cooldownStore = null;
    [SerializeField] ActionLocker actionLocker = null;
    [SerializeField] Ability[] abilities = new Ability[4];
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
            abilityData.mousePos = castPosition;
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
            ChangeOrCastAbility(abilities[0]);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            ChangeOrCastAbility(abilities[1]);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ChangeOrCastAbility(abilities[2]);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ChangeOrCastAbility(abilities[3]);
        }

        if(currentAbility != null)
        {
            currentAbility.UpdateIndicator(abilityData);

            if (Input.GetMouseButtonDown(0))
            {
                if (!currentAbility.SmartCast)
                {
                    currentAbility.HideIndicator();
                    CastAbility(currentAbility);
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
        {
            currentAbility.HideIndicator();
        }

        if (!mana.IsManaEnough(ability.GetManaCost())) return;

        if(cooldownStore.GetTimeRemaining(ability) > 0) { return; }

        currentAbility = ability;

        if (currentAbility.SmartCast)
        {
            CastAbility(currentAbility);
            currentAbility = null;
        }
        else
        {
            currentAbility.ShowIndicator();
        }
    }

    private void CastAbility(Ability ability)
    {
        bool canCast = actionLocker.TryGetLock(ability);
        if (canCast)
        {
            mana.ClientUseMana(ability.GetManaCost());
            ability.Cast(abilityData);
            cooldownStore.ClientStartCooldown(ability, ability.GetCooldownTime());
        }
    }

    public Ability GetAbility(int index)
    {
        return abilities[index];
    }
}