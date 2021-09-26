using Dota.Networking;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField] Team localPlayerTeam;
    [SerializeField] GameOverCanvas gameOverCanvas;

    private void Awake()
    {
        localPlayerTeam = NetworkClient.localPlayer.GetComponent<DotaRoomPlayer>().GetTeam();
    }

    #region Server
    public override void OnStartServer()
    {
        Base.ServerOnBaseDead += Base_ServerOnBaseDead;
    }

    public override void OnStopServer()
    {
        Base.ServerOnBaseDead -= Base_ServerOnBaseDead;
    }

    private void Base_ServerOnBaseDead(Base obj)
    {
        
    }
    #endregion

    #region Client

    public override void OnStartClient()
    {
        Base.ClientOnBaseDead += Base_ClientOnBaseDead;
    }

    public override void OnStopClient()
    {
        Base.ClientOnBaseDead -= Base_ClientOnBaseDead;
    }

    private void Base_ClientOnBaseDead(Base teamBase)
    {
        if (teamBase.GetTeam() == localPlayerTeam)
        {
            gameOverCanvas.ShowDefeat();
        }
        else
        {
            gameOverCanvas.ShowVictory();
        }
    }
    #endregion
}
