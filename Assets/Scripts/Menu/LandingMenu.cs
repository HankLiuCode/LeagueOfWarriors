using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LandingMenu : MonoBehaviour
{
    [SerializeField] private GameObject hostLobbyPanel;
    [SerializeField] private GameObject joinLobbyPanel;
    

    public void HostLobby()
    {
        DotaNetworkManager.singleton.StartHost();
    }

    public void SetHostPanel(bool isActive)
    {
        hostLobbyPanel.SetActive(isActive);
    }

    public void SetJoinPanel(bool isActive)
    {
        joinLobbyPanel.SetActive(isActive);
    }
}
