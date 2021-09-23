using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Dota.Networking;

public class DotaNetworkManager : NetworkManager
{
    [SerializeField] List<DotaNewRoomPlayer> serverPlayerList = new List<DotaNewRoomPlayer>();
    
    // Client Side Event
    public static event System.Action<NetworkConnection> OnClientConnected;
    
    // Server Side Event
    public static event System.Action OnGameSceneFinishLoading;

    public override void OnStartServer()
    {
        NetworkServer.RegisterHandler<PlayerConnectMessage>(OnPlayerConnected);
        DotaNewRoomPlayer.OnPlayerReady += DotaNewRoomPlayer_OnPlayerReady;
    }

    private void DotaNewRoomPlayer_OnPlayerReady(DotaNewRoomPlayer roomPlayer)
    {
        bool allPlayersReady = true;
        foreach(DotaNewRoomPlayer player in serverPlayerList)
        {
            if (!player.GetIsReady())
            {
                allPlayersReady = false;
                break;
            }
        }
        if (allPlayersReady)
        {
            foreach (DotaNewRoomPlayer player in serverPlayerList)
            {
                player.ServerSetReady(false);
            }
            ServerChangeScene("Game");
        }
    }

    public void ReturnToRoom()
    {
        ServerChangeScene("Room");
    }

    public List<DotaNewRoomPlayer> GetServerPlayerList()
    {
        return serverPlayerList;
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if(sceneName == "Game")
        {
            OnGameSceneFinishLoading?.Invoke();
        }
        else
        {
            Debug.Log(sceneName + " Finished loading.");
        }
    }

    public override void OnStopServer()
    {
        serverPlayerList.Clear();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        foreach (DotaNewRoomPlayer player in serverPlayerList)
        {
            if (player.connectionToClient == conn)
            {
                NetworkServer.Destroy(player.gameObject);
            }
            else
            {
                player.connectionToClient.Send(new PlayerLeaveMessage());
            }
        }
        serverPlayerList.RemoveAll(player => player.connectionToClient == conn);
    }

    public void OnPlayerConnected(NetworkConnection conn, PlayerConnectMessage playerConnectMsg)
    { 
        GameObject playerInstance = Instantiate(playerPrefab);

        DotaNewRoomPlayer dotaGamePlayer = playerInstance.GetComponent<DotaNewRoomPlayer>();

        dotaGamePlayer.ServerSetPlayerName(playerConnectMsg.playerName);

        serverPlayerList.Add(dotaGamePlayer);

        playerInstance.name = $"{playerPrefab.name} [connId={conn.connectionId}]";

        NetworkServer.AddPlayerForConnection(conn, playerInstance);
    }

    public override void OnServerAddPlayer(NetworkConnection conn) { }


    #region Client

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        
        Debug.Log("OnClientConnect");

        OnClientConnected?.Invoke(conn);
    }

    #endregion
}
