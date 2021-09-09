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
    public event System.Action OnLocalChampionReady;

    private void Awake()
    {
        ((DotaNetworkRoomManager)NetworkRoomManager.singleton).OnAllGamePlayersAdded += PlayerManager_OnAllGamePlayersAdded;
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

    public SyncList<Champion> GetChampions()
    {
        return players;
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
    #endregion

}
