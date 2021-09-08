using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Dota.UI;
using Dota.Core;
using Dota.Networking;
using Dota.Controls;

public class CameraSetup : MonoBehaviour
{
    [SerializeField] CameraController cameraControllerInstance = null;

    private void Start()
    {
        ((DotaNetworkRoomManager) NetworkRoomManager.singleton).OnAllGamePlayersAdded += UISetup_OnAllPlayersAdded;
    }

    private void UISetup_OnAllPlayersAdded()
    {
        List<DotaGamePlayer> players = ((DotaNetworkRoomManager) NetworkRoomManager.singleton).ClientGetDotaGamePlayers();
        foreach(DotaGamePlayer player in players)
        {
            if (player.isLocalPlayer)
            {
                cameraControllerInstance.Initialize(player.transform);
                return;
            }
        }
    }

    //public void SetUpOtherUI(List<DotaGamePlayer> players)
    //{
    //    otherPlayerCanvasInstance = Instantiate(otherPlayerCanvasPrefab).GetComponent<OtherPlayerStatsDisplay>();

    //    List<DotaPlayerController> playerControllers = new List<DotaPlayerController>();
    //    foreach (DotaGamePlayer dp in players)
    //    {
    //        playerControllers.Add(dp.GetComponent<DotaPlayerController>());
    //    }
    //    otherPlayerCanvasInstance.BindPlayersToDisplays(playerControllers);
    //}
}
