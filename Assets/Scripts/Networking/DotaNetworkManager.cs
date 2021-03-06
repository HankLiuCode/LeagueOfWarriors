using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Dota.Networking;
using UnityEngine.SceneManagement;
using System;

public class DotaNetworkManager : NetworkManager
{
    [Scene] [SerializeField] string gameScene;
    [SerializeField] int maxPlayers = 2;
    [SerializeField] List<DotaRoomPlayer> serverPlayers = new List<DotaRoomPlayer>();
    [SerializeField] List<DotaRoomPlayer> clientPlayers = new List<DotaRoomPlayer>();

    string serverCurrentScene;
    List<NetworkConnection> sceneLoadedClients = new List<NetworkConnection>();

    public static event System.Action<string> ClientOnAllClientSceneLoaded;
    public static event System.Action ServerOnAllPlayersReady;
    public static event System.Action<string> ServerOnAllClientSceneLoaded;
    public static event System.Action<DotaRoomPlayer> ServerOnClientDisconnect;

    public override void Awake()
    {
        base.Awake();
        DotaRoomPlayer.ClientOnPlayerConnected += DotaRoomPlayer_OnPlayerConnected;
        DotaRoomPlayer.ClientOnPlayerDisconnected += DotaRoomPlayer_OnPlayerDisconnected;
    }

    #region Client

    public override void OnStartClient()
    {
        NetworkClient.RegisterHandler<AllClientFinishLoadSceneMessage>(HandleAllClientSceneLoaded);
    }

    public override void OnStopClient()
    {
        DotaRoomPlayer.ClientOnPlayerConnected -= DotaRoomPlayer_OnPlayerConnected;
        DotaRoomPlayer.ClientOnPlayerDisconnected -= DotaRoomPlayer_OnPlayerDisconnected;
    }

    public List<DotaRoomPlayer> GetClientPlayers()
    {
        return clientPlayers;
    }

    public List<DotaRoomPlayer> GetServerPlayers()
    {
        return serverPlayers;
    }

    private void HandleAllClientSceneLoaded(AllClientFinishLoadSceneMessage msg)
    {
        ClientOnAllClientSceneLoaded?.Invoke(msg.scenePath);
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);
        conn.Send(new ClientSceneLoadedMessage { scenePath = SceneManager.GetActiveScene().path});
    }

    private void DotaRoomPlayer_OnPlayerDisconnected(DotaRoomPlayer player)
    {
        clientPlayers.Remove(player);
    }

    private void DotaRoomPlayer_OnPlayerConnected(DotaRoomPlayer player)
    {
        clientPlayers.Add(player);
    }
    #endregion

    #region Server
    public override void OnStartServer()
    {
        DotaRoomPlayer.ClientOnPlayerConnectionModified += DotaRoomPlayer_OnPlayerConnectionModified;
        NetworkServer.RegisterHandler<ClientSceneLoadedMessage>(OnServerClientSceneLoaded);
        NetworkServer.RegisterHandler<ClientGameToRoomRequestMessage>(ServerOnClientGameToRoomRequest);
    }

    public void ServerOnClientGameToRoomRequest(NetworkConnection networkConnection, ClientGameToRoomRequestMessage msg)
    {
        ChangeToRoomScene();
    }

    public override void OnStopServer()
    {
        DotaRoomPlayer.ClientOnPlayerConnectionModified -= DotaRoomPlayer_OnPlayerConnectionModified;
        DotaRoomPlayer.ClientOnPlayerConnected -= DotaRoomPlayer_OnPlayerConnected;
        DotaRoomPlayer.ClientOnPlayerDisconnected -= DotaRoomPlayer_OnPlayerDisconnected;
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        List<DotaRoomPlayer> toRemove = new List<DotaRoomPlayer>();
        foreach(DotaRoomPlayer player in serverPlayers)
        {
            if(player.connectionToClient == conn)
            {
                toRemove.Add(player);
            }
        }

        foreach(DotaRoomPlayer player in toRemove)
        {
            serverPlayers.Remove(player);
            ServerOnClientDisconnect?.Invoke(player);
            Debug.Log("Player Disconnect: " + conn);
        }

        base.OnServerDisconnect(conn);
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if(serverPlayers.Count >= maxPlayers) { return; }

        if(serverCurrentScene == gameScene) { return; }

        Transform startPos = GetStartPosition();
        GameObject player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);

        DotaRoomPlayer roomPlayer = player.GetComponent<DotaRoomPlayer>();
        serverPlayers.Add(roomPlayer);

        // instantiating a "Player" prefab gives it the name "Player(clone)"
        // => appending the connectionId is WAY more useful for debugging!
        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        serverCurrentScene = sceneName;
    }

    public void OnServerClientSceneLoaded(NetworkConnection networkConnection, ClientSceneLoadedMessage msg)
    {
        if (serverCurrentScene == msg.scenePath)
        {
            sceneLoadedClients.Add(networkConnection);
        }

        if(sceneLoadedClients.Count == serverPlayers.Count)
        {
            ServerOnAllClientSceneLoaded?.Invoke(serverCurrentScene);
            foreach (NetworkConnection connection in sceneLoadedClients)
            {
                connection.Send(new AllClientFinishLoadSceneMessage { scenePath = serverCurrentScene });
            }
            sceneLoadedClients.Clear();
        }
    }

    public int GetMaxPlayers()
    {
        return maxPlayers;
    }

    public void SetMaxPlayers(int maxPlayers)
    {
        this.maxPlayers = maxPlayers;
    }
    
    public void ChangeToRoomScene()
    {
        foreach(DotaRoomPlayer player in serverPlayers)
        {
            player.ServerSetConnectionState(PlayerConnectionState.RoomNotReady);
        }
        ServerChangeScene(onlineScene);
    }

    public void ChangeToGameScene()
    {
        ServerChangeScene(gameScene);
    }

    private void DotaRoomPlayer_OnPlayerConnectionModified(DotaRoomPlayer player)
    {
        if (IsAllowedToStartGame())
        {
            ServerOnAllPlayersReady?.Invoke();
        }
    }

    public bool IsAllowedToStartGame()
    {
        int readyCount = 0;
        int redCount = 0;
        foreach (DotaRoomPlayer serverPlayer in serverPlayers)
        {
            if (serverPlayer.GetConnectionState() == PlayerConnectionState.RoomReady)
            {
                readyCount++;
            }
            if(serverPlayer.GetTeam() == Team.Red)
            {
                redCount++;
            }
        }
        bool hasDifferentTeam = (redCount != serverPlayers.Count) && (redCount != 0);
        bool allReady = readyCount == serverPlayers.Count && serverPlayers.Count != 0;

        if (allReady)
        {
            return true;
        }
        return false;
    }


    #endregion
}
