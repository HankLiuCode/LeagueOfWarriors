using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Dota.Networking;

public class DotaNetworkManager : NetworkManager
{
    [SerializeField] List<DotaRoomPlayer> serverPlayerList = new List<DotaRoomPlayer>();
    
    // Client Side Event
    public static event System.Action<NetworkConnection> OnClientConnected;
    
    // Server Side Event
    public static event System.Action OnGameSceneFinishLoading;

    public override void OnStartServer()
    {
        NetworkServer.RegisterHandler<PlayerConnectMessage>(OnPlayerConnected);
        NetworkServer.RegisterHandler<PlayerUpdateMessage> (OnPlayerUpdated);

        DotaRoomPlayer.OnPlayerConnectionModified += DotaRoomPlayer_OnPlayerConnectionModified;
    }

    // Server
    public void OnPlayerUpdated(NetworkConnection conn, PlayerUpdateMessage playerUpdateMsg)
    {

        foreach (DotaRoomPlayer player in serverPlayerList)
        {
            if (player.connectionToClient == conn)
            {
                if (playerUpdateMsg.switchChampion)
                {
                    player.ServerSetChampionId(playerUpdateMsg.championId);
                }

                if (playerUpdateMsg.switchTeam)
                {
                    Team originTeam = player.GetTeam();
                    if(originTeam == Team.Red)
                    {
                        player.ServerSetTeam(Team.Blue);
                    }
                    else if(originTeam == Team.Blue)
                    {
                        player.ServerSetTeam(Team.Red);
                    }
                }

                if (playerUpdateMsg.toggleReady)
                {
                    player.ServerSetConnectionState(PlayerConnectionState.RoomReady);
                }
            }
        }
    }

    // Server
    public void OnPlayerConnected(NetworkConnection conn, PlayerConnectMessage playerConnectMsg)
    {
        GameObject playerInstance = Instantiate(playerPrefab);

        DotaRoomPlayer dotaGamePlayer = playerInstance.GetComponent<DotaRoomPlayer>();

        dotaGamePlayer.ServerSetPlayerName(playerConnectMsg.playerName);

        serverPlayerList.Add(dotaGamePlayer);

        playerInstance.name = $"{playerPrefab.name} [connId={conn.connectionId}]";

        NetworkServer.AddPlayerForConnection(conn, playerInstance);
    }

    // Server
    private void DotaRoomPlayer_OnPlayerConnectionModified(DotaRoomPlayer roomPlayer, PlayerConnectionState playerConnState)
    {
        bool allPlayersReady = true;
        foreach (DotaRoomPlayer player in serverPlayerList)
        {
            if (!(playerConnState == PlayerConnectionState.RoomReady))
            {
                allPlayersReady = false;
                break;
            }
        }
        if (allPlayersReady)
        {
            foreach (DotaRoomPlayer player in serverPlayerList)
            {
                player.ServerSetConnectionState(PlayerConnectionState.RoomToGame);
            }
            ServerChangeScene("Game");
        }
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        conn.Send(new ClientSceneLoadedMessage());
    }

    public List<DotaRoomPlayer> GetServerPlayerList()
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
        foreach (DotaRoomPlayer player in serverPlayerList)
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
