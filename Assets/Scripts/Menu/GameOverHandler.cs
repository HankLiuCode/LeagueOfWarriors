using Dota.Networking;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameState
{
    InGame,
    GameOver
}

public class GameOverHandler : NetworkBehaviour
{
    [SerializeField] Team localPlayerTeam;
    [SerializeField] GameOverCanvas gameOverCanvas;

    [SyncVar]
    private GameState gameState;

    private static GameOverHandler Instance;

    public static event System.Action<Base> OnServerGameOver;
    public static event System.Action<Base> OnClientGameOver;



    private void Awake()
    {
        Instance = this;
        localPlayerTeam = NetworkClient.localPlayer.GetComponent<DotaRoomPlayer>().GetTeam();
    }

    public static GameOverHandler Singleton()
    {
        return Instance;
    }

    public GameState GetGameState()
    {
        return gameState;
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

    private void Base_ServerOnBaseDead(Base teamBase)
    {
        gameState = GameState.GameOver;
        OnServerGameOver?.Invoke(teamBase);
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
        OnClientGameOver?.Invoke(teamBase);
    }
    #endregion
}
