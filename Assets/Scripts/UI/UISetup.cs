using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Dota.UI;
using Dota.Core;
using Dota.Networking;
using Dota.Controls;

public class UISetup : MonoBehaviour
{
    [SerializeField] GameObject midHUDCanvasPrefab = null;
    [SerializeField] GameObject otherPlayerCanvasPrefab = null;
    [SerializeField] CameraController cameraControllerInstance = null;


    HealthDisplay healthDisplayInstance = null;
    ManaDisplay manaDisplayInstance = null;
    AbilityUI[] abilityDisplayInstances = null;

    OtherPlayerStatsDisplay otherPlayerCanvasInstance = null;
    

    private void Start()
    {
        ((DotaNetworkRoomManager) NetworkRoomManager.singleton).OnAllPlayersAdded += UISetup_OnAllPlayersAdded;
    }

    private void UISetup_OnAllPlayersAdded()
    {
        List<DotaGamePlayer> players = ((DotaNetworkRoomManager) NetworkRoomManager.singleton).ClientGetDotaGamePlayers();
        foreach(DotaGamePlayer player in players)
        {
            if (player.isLocalPlayer)
            {
                SetUpUI(player);
            }
        }
        SetUpOtherUI(players);
    }

    public void SetUpUI(DotaGamePlayer dotaGamePlayer)
    {
        DotaPlayerController dpc = dotaGamePlayer.GetDotaPlayerController();
        SetUpSelfUI(dpc);
    }

    public void SetUpOtherUI(List<DotaGamePlayer> players)
    {
        otherPlayerCanvasInstance = Instantiate(otherPlayerCanvasPrefab).GetComponent<OtherPlayerStatsDisplay>();

        List<DotaPlayerController> playerControllers = new List<DotaPlayerController>();
        foreach (DotaGamePlayer dp in players)
        {
            playerControllers.Add(dp.GetDotaPlayerController());
        }
        otherPlayerCanvasInstance.BindPlayersToDisplays(playerControllers);
    }

    public void SetUpSelfUI(DotaPlayerController localPlayerController)
    {
        GameObject displayInstance = Instantiate(midHUDCanvasPrefab);
        healthDisplayInstance = displayInstance.GetComponent<HealthDisplay>();
        manaDisplayInstance = displayInstance.GetComponent<ManaDisplay>();
        abilityDisplayInstances = displayInstance.GetComponentsInChildren<AbilityUI>();
        
        cameraControllerInstance.Initialize(localPlayerController.transform);
        healthDisplayInstance.SetHealth(localPlayerController.GetComponent<Health>());
        manaDisplayInstance.SetMana(localPlayerController.GetComponent<Mana>());

        foreach (AbilityUI aInstance in abilityDisplayInstances)
        {
            aInstance.SetUp(localPlayerController);
        }
    }
}
