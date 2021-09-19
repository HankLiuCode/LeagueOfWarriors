using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HostMenu : MonoBehaviour
{
    [SerializeField] private GameObject joinLobbyPanel;
    [SerializeField] private TextMeshProUGUI textMesh;
    public void HostLobby()
    {
        DotaNetworkRoomManager.singleton.StartHost();
    }

    public void ShowJoinLobbyPanel()
    {
        joinLobbyPanel.gameObject.SetActive(true);
    }

    public void HideJoinLobbyPanel()
    {
        joinLobbyPanel.gameObject.SetActive(false);
    }

    public void JoinLobby()
    {
        DotaNetworkRoomManager.singleton.networkAddress = textMesh.text;
        DotaNetworkRoomManager.singleton.StartClient();
    }
}
