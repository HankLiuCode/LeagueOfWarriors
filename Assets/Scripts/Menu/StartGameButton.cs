using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGameButton : NetworkBehaviour
{
    [SerializeField] Button startButton;

    private void Awake()
    {
        DotaNetworkManager.ServerOnAllPlayersReady += DotaNetworkManager_ServerOnAllPlayersReady;
    }

    private void DotaNetworkManager_ServerOnAllPlayersReady()
    {
        startButton.gameObject.SetActive(true);
    }

    [Server]
    public void StartGame()
    {
        if (isServer)
        {
            ((DotaNetworkManager)NetworkManager.singleton).ChangeToGameScene();
        }
    }

    public override void OnStartServer()
    {
        startButton.gameObject.SetActive(false);
    }


    private void OnDestroy()
    {
        DotaNetworkManager.ServerOnAllPlayersReady -= DotaNetworkManager_ServerOnAllPlayersReady;
    }
}
