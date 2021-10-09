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
    [SerializeField] FloatingBarManager floatingBarCanvas;

    [SyncVar]
    private GameState gameState;

    public float gameOverAnimationLength = 5f;

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
        OnClientGameOver?.Invoke(teamBase);

        floatingBarCanvas.gameObject.SetActive(false);
        ShowGameOverVideo(teamBase);
        StartCoroutine(ShowButtonAfter(gameOverAnimationLength));
    }

    public void ShowGameOverVideo(Base destroyedBase)
    {
        if (destroyedBase.GetTeam() == localPlayerTeam)
        {
            gameOverCanvas.ShowDefeat();
        }
        else
        {
            gameOverCanvas.ShowVictory();
        }
    }

    IEnumerator ShowButtonAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        gameOverCanvas.ShowContinueButton();
    }


    #endregion
}
