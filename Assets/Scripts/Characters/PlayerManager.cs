using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dota.Networking;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField] GameObject championPrefab;
    [SerializeField] Transform[] blueStartPositions;
    [SerializeField] Transform[] redStartPositions;

    int blueStartPositionIndex = 0;
    int redStartPositionIndex = 0;
    
    [SerializeField] List<Champion> debugPlayers = new List<Champion>();
    SyncList<Champion> players = new SyncList<Champion>();
    public event System.Action OnLocalChampionReady;

    private void Awake()
    {
        ((DotaNetworkRoomManager) NetworkRoomManager.singleton).OnAllGamePlayersAdded += PlayerManager_OnAllGamePlayersAdded;
    }

    public SyncList<Champion> GetChampions()
    {
        return players;
    }

    [Server]
    public Vector3 GetSpawnPosition(Team team)
    {
        switch (team)
        {
            case Team.Red:

                Transform redStartPos = redStartPositions[redStartPositionIndex];
                redStartPositionIndex = (redStartPositionIndex + 1) % redStartPositions.Length;
                return redStartPos.position;

            case Team.Blue:

                Transform blueStartPos = blueStartPositions[blueStartPositionIndex];
                blueStartPositionIndex = (blueStartPositionIndex + 1) % blueStartPositions.Length;
                return blueStartPos.position;

            default:
                return Vector3.zero;
        }
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
        Team championTeam = dotaGamePlayer.GetTeam();

        Vector3 spawnPosition = GetSpawnPosition(championTeam);

        GameObject championInstance = Instantiate(championPrefab, spawnPosition, Quaternion.identity);

        Champion champion = championInstance.GetComponent<Champion>();

        champion.SetTeam(championTeam);

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

    [Client]
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
        throw new System.Exception("LocalPlayer not Ready");
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
