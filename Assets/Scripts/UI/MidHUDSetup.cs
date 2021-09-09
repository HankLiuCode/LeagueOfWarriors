using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dota.UI;
using Mirror;
using Dota.Networking;
using Dota.Core;

public class MidHUDSetup : MonoBehaviour
{
    [SerializeField] PlayerManager playerManager = null;
    [SerializeField] IconDisplay iconDisplay = null;
    [SerializeField] HealthDisplay healthDisplay = null;
    [SerializeField] ManaDisplay manaDisplay = null;
    [SerializeField] GameObject abilitiesContainer = null;

    private void Start()
    {
        playerManager.OnLocalChampionReady += PlayerManager_OnLocalChampionReady;
    }

    private void PlayerManager_OnLocalChampionReady()
    {
        Champion champion = playerManager.GetLocalChampion();
        Health health = champion.GetComponent<Health>();
        Mana mana = champion.GetComponent<Mana>();

        iconDisplay.SetIcon(champion.GetIcon());
        healthDisplay.SetHealth(health);
        manaDisplay.SetMana(mana);

        AbilityUI[] abilityUIs = abilitiesContainer.GetComponentsInChildren<AbilityUI>();
        foreach (AbilityUI abilityUI in abilityUIs)
        {
            abilityUI.SetUp(champion.GetComponent<AbilityCaster>());
        }
    }
}
