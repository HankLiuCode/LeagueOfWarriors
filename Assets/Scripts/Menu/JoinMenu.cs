using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class JoinMenu : MonoBehaviour
{
    [SerializeField] TMP_InputField address = null;

    public void JoinLobby()
    {
        DotaNetworkManager.singleton.networkAddress = address.text;
        DotaNetworkManager.singleton.StartClient();
    }

    public void CancelJoin()
    {
        DotaNetworkManager.singleton.StopClient();
    }
}
