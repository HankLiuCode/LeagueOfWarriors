using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class JoinMenu : MonoBehaviour
{
    [SerializeField] TMP_InputField address = null;
    [SerializeField] TMP_InputField playerName = null;

    public void JoinLobby()
    {
        DotaNetworkManager.OnClientConnected += DotaNetworkManager_OnClientConnected;
        DotaNetworkManager.singleton.networkAddress = address.text;
        DotaNetworkManager.singleton.StartClient();
    }

    private void DotaNetworkManager_OnClientConnected(NetworkConnection conn)
    {
        string playerStr = playerName.text;
        conn.Send(new PlayerConnectMessage { playerName = playerStr });
    }
}
