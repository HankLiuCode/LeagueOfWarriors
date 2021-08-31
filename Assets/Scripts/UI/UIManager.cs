using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Dota.Networking;

public class UIManager : NetworkBehaviour
{
    [SerializeField] List<DotaGamePlayer> allPlayers = new List<DotaGamePlayer>();

    void Start()
    {
        ((DotaNetworkManager)NetworkManager.singleton).OnPlayerChanged += UIManager_OnPlayerChanged;
    }

    private void UIManager_OnPlayerChanged()
    {
        allPlayers = new List<DotaGamePlayer>(((DotaNetworkManager)NetworkManager.singleton).DotaPlayers);

    }
}
