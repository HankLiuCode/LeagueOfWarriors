using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using System;

public class HostMenu : MonoBehaviour
{
    [SerializeField] TMP_InputField playerName = null;

    public void HostLobby()
    {
        //DotaNetworkManager.OnClientConnected += DotaNetworkManager_OnClientConnected;
        DotaNetworkManager.singleton.StartHost();
    }

    private void DotaNetworkManager_OnClientConnected(NetworkConnection conn)
    {
        string playerStr = playerName.text;
        conn.Send(new ClientConnectMessage { playerName = playerStr });
    }
}
