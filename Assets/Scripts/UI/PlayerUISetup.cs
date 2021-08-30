using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Dota.UI;
using Dota.Core;
using Dota.Networking;
using Dota.Controls;

public class PlayerUISetup : MonoBehaviour
{
    [SerializeField] GameObject selfDisplayPrefab = null;
    [SerializeField] GameObject otherDisplayPrefab = null;
    [SerializeField] CameraController cameraControllerPrefab = null;

    HealthDisplay healthDisplayInstance = null;
    ManaDisplay manaDisplayInstance = null;
    OtherPlayerStatsDisplay otherStatsDisplayInstance = null;
    CameraController cameraControllerInstance = null;

    private void Start()
    {
        ((DotaNetworkManager)NetworkManager.singleton).OnPlayerChanged += PlayerUISetup_OnPlayerChanged;

        otherStatsDisplayInstance = Instantiate(otherDisplayPrefab).GetComponent<OtherPlayerStatsDisplay>();

        RebindOtherPlayersUI();
    }

    private void PlayerUISetup_OnPlayerChanged()
    {
        RebindOtherPlayersUI();
    }

    public void RebindOtherPlayersUI()
    {
        List<DotaPlayer> players = ((DotaNetworkManager)NetworkManager.singleton).DotaPlayers;
        List<DotaPlayerController> playerControllers = new List<DotaPlayerController>();
        foreach (DotaPlayer dp in players)
        {
            playerControllers.Add(dp.GetDotaPlayerController());
        }
        otherStatsDisplayInstance.BindPlayersToDisplays(playerControllers);
    }

    public void DestroyOtherUI()
    {
        Destroy(otherStatsDisplayInstance);
    }

    public void SetUpSelfUI(DotaPlayerController localPlayerController)
    {
        GameObject displayInstance = Instantiate(selfDisplayPrefab);
        healthDisplayInstance = displayInstance.GetComponent<HealthDisplay>();
        manaDisplayInstance = displayInstance.GetComponent<ManaDisplay>();
        cameraControllerInstance = Instantiate(cameraControllerPrefab);

        cameraControllerInstance.Initialize(localPlayerController.transform);
        healthDisplayInstance.SetHealth(localPlayerController.GetComponent<Health>());
        manaDisplayInstance.SetMana(localPlayerController.GetComponent<Mana>());
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
