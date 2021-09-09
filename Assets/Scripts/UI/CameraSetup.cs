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
    [SerializeField] PlayerManager playerManager = null;

    private void Start()
    {
        playerManager.OnLocalChampionReady += PlayerManager_OnLocalChampionReady;
    }

    private void PlayerManager_OnLocalChampionReady()
    {
        Champion champion = playerManager.GetLocalChampion();
        cameraControllerInstance.Initialize(champion.transform);
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
