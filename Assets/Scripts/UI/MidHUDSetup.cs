using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dota.UI;
using Mirror;
using Dota.Networking;
using Dota.Core;

public class MidHUDSetup : MonoBehaviour
{
    [SerializeField] HealthDisplay healthDisplay = null;
    [SerializeField] ManaDisplay manaDisplay = null;
    [SerializeField] GameObject abilitiesContainer = null;

    private void Start()
    {
        ((DotaNetworkRoomManager) NetworkRoomManager.singleton).OnAllGamePlayersAdded += UISetup_OnAllPlayersAdded;
    }

    private void UISetup_OnAllPlayersAdded()
    {
        List<DotaGamePlayer> players = ((DotaNetworkRoomManager)NetworkRoomManager.singleton).ClientGetDotaGamePlayers();
        foreach (DotaGamePlayer player in players)
        {
            if (player.isLocalPlayer)
            {
                Health health = player.GetComponent<Health>();
                Mana mana = player.GetComponent<Mana>();
                healthDisplay.SetHealth(health);
                manaDisplay.SetMana(mana);

                AbilityUI[] abilityUIs = abilitiesContainer.GetComponentsInChildren<AbilityUI>();
                foreach(AbilityUI abilityUI in abilityUIs)
                {
                    abilityUI.SetUp(player.GetComponent<AbilityCaster>());
                }
                return;
            }
        }
    }
}
