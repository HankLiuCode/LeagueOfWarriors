using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dota.Networking;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField] GameObject championPrefab;
    [SerializeField] List<Champion> debugPlayers = new List<Champion>();
    SyncList<Champion> players = new SyncList<Champion>();

    public event System.Action OnServerAllChampionSpawned;
    public event System.Action OnLocalChampionReady;


    private void Awake()
    {
        ((DotaNetworkRoomManager)NetworkRoomManager.singleton).OnAllGamePlayersAdded += PlayerManager_OnAllGamePlayersAdded;
    }
    private void Start()
    {
        players.Callback += Players_Callback;
    }

    #region Server

    [Server]
    public void SpawnChampion(DotaGamePlayer dotaGamePlayer)
    {
        StartCoroutine(SpawnChampionWhenConnectionReady(dotaGamePlayer));
    }

    [Server]
    IEnumerator SpawnChampionWhenConnectionReady(DotaGamePlayer dotaGamePlayer)
    {
        GameObject championInstance = Instantiate(championPrefab);

        Champion champion = championInstance.GetComponent<Champion>();

        champion.SetTeam(dotaGamePlayer.GetTeam());

        yield return new WaitUntil(() => dotaGamePlayer.connectionToClient != null);

        NetworkServer.Spawn(championInstance, dotaGamePlayer.connectionToClient);

        //Debug.Log(dotaGamePlayer.connectionToClient == null ? "ConnectionToClient is Null" : "Not Null");

        players.Add(championInstance.GetComponent<Champion>());

        List<DotaGamePlayer> dotaGamePlayers = ((DotaNetworkRoomManager)NetworkRoomManager.singleton).ServerGetDotaPlayers();

        if (players.Count == dotaGamePlayers.Count)
        {
            // At this point
            // 1. The Champion is all added to List On The Server Side
            // 2. The Champion is all spawned on the server side
            // BUT 
            // 1. The champion list is on client not synced with the server yet
            // 2. The champion might not be spawned yet on the client 

            RpcNotifyServerSpawnedAllChampion();
        }
    }

    [Server]
    private void PlayerManager_OnAllGamePlayersAdded()
    {
        List<DotaGamePlayer> dotaGamePlayers = ((DotaNetworkRoomManager) NetworkRoomManager.singleton).ServerGetDotaPlayers();

        foreach(DotaGamePlayer gamePlayer in dotaGamePlayers)
        {
            SpawnChampion(gamePlayer);
        }
    }
    #endregion
    

    #region Client
    [ClientRpc]
    private void RpcNotifyServerSpawnedAllChampion()
    {
        Debug.Log("RpcNotifyServerConnectionReady");
        StartCoroutine(InvokeWhenChampionListSynced());
    }

    IEnumerator InvokeWhenChampionListSynced()
    {
        yield return new WaitForSeconds(1.0f);

        // this has to be called since the client doesn't receive the callback from server
        SyncDebugList();

        OnLocalChampionReady?.Invoke();
    }

    [Client]
    public Champion GetLocalChampion()
    {
        foreach (Champion player in players)
        {
            if(player.hasAuthority && isClient)
            {
                return player;
            }
        }
        return null;
    }

    public SyncList<Champion> GetPlayers()
    {
        return players;
    }
    #endregion

    private void Players_Callback(SyncList<Champion>.Operation op, int itemIndex, Champion oldItem, Champion newItem)
    {
        switch (op)
        {
            case SyncList<Champion>.Operation.OP_ADD:
                // index is where it got added in the list
                // newItem is the new item
                break;

            case SyncList<Champion>.Operation.OP_CLEAR:
                // list got cleared
                break;

            case SyncList<Champion>.Operation.OP_INSERT:
                // index is where it got added in the list
                // newItem is the new item
                break;

            case SyncList<Champion>.Operation.OP_REMOVEAT:
                // index is where it got removed in the list
                // oldItem is the item that was removed
                break;

            case SyncList<Champion>.Operation.OP_SET:
                // index is the index of the item that was updated
                // oldItem is the previous value for the item at the index
                // newItem is the new value for the item at the index
                break;
        }
        SyncDebugList();
    }

    [Client]
    private void SyncDebugList()
    {
        debugPlayers.Clear();
        foreach (Champion champion in players)
        {
            debugPlayers.Add(champion);
        }
    }
}
