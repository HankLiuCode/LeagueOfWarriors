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
    [SerializeField] CameraController cameraControllerPrefab = null;

    HealthDisplay healthDisplayInstance = null;
    ManaDisplay manaDisplayInstance = null;
    AbilityUI[] abilityDisplayInstances = null;


    OtherPlayerStatsDisplay otherPlayerCanvasInstance = null;
    CameraController cameraControllerInstance = null;

    public void SetUpUI(DotaGamePlayer dotaGamePlayer)
    {
        DotaPlayerController dpc = dotaGamePlayer.GetDotaPlayerController();
        SetUpSelfUI(dpc);
        SetUpOtherUI();
    }

    public void SetUpOtherUI()
    {
        otherPlayerCanvasInstance = Instantiate(otherPlayerCanvasPrefab).GetComponent<OtherPlayerStatsDisplay>();

        List<DotaGamePlayer> players = ((DotaNetworkRoomManager) NetworkRoomManager.singleton).DotaGamePlayers;

        Debug.Log(players.Count);

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
        
        cameraControllerInstance = Instantiate(cameraControllerPrefab);
        cameraControllerInstance.Initialize(localPlayerController.transform);
        healthDisplayInstance.SetHealth(localPlayerController.GetComponent<Health>());
        manaDisplayInstance.SetMana(localPlayerController.GetComponent<Mana>());

        foreach (AbilityUI aInstance in abilityDisplayInstances)
        {
            aInstance.SetUp(localPlayerController);
        }
    }


    public void DestroyOtherUI()
    {
        Destroy(otherPlayerCanvasInstance);
    }

    public void DestroySelfUI()
    {
        Destroy(healthDisplayInstance.gameObject);
        Destroy(cameraControllerInstance.gameObject);
    }

    public void DestroyAll()
    {
        DestroySelfUI();
        DestroyOtherUI();
        Destroy(gameObject);
    }
}
