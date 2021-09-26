using Dota.Networking;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField] Team localPlayerTeam;
    [SerializeField] Canvas loadingScreen;

    void Awake()
    {
        DotaNetworkManager.ClientOnAllClientSceneLoaded += DotaNetworkManager_ClientOnAllClientSceneLoaded;
        Base.ClientOnBaseDead += Base_ClientOnBaseDead;
    }

    public override void OnStartClient()
    {
        localPlayerTeam = NetworkClient.localPlayer.GetComponent<DotaRoomPlayer>().GetTeam();
    }

    private void Base_ClientOnBaseDead(Base teamBase)
    {
        if(teamBase.GetTeam() == localPlayerTeam)
        {
            Debug.Log("You Lose!");
        }
        else
        {
            Debug.Log("You Win!");
        }
    }

    private void DotaNetworkManager_ClientOnAllClientSceneLoaded(string obj)
    {
        SetLoadingScreenVisible(false);
    }

    public void SetLoadingScreenVisible(bool isActive)
    {
        loadingScreen.gameObject.SetActive(isActive);
    }
}
