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

    public event System.Action<Champion> OnChampionAdded;
    public event System.Action<Champion> OnChampionRemoved;
    
    public event System.Action OnLocalChampionReady;

    private void Awake()
    {
        DotaNetworkManager.OnGameSceneFinishLoading += DotaNetworkManager_OnGameSceneFinishLoading;
    }

    private void DotaNetworkManager_OnGameSceneFinishLoading()
    {
        List<DotaNewRoomPlayer> dotaGamePlayers = ((DotaNetworkManager) NetworkManager.singleton).GetServerPlayerList();
        foreach (DotaNewRoomPlayer gamePlayer in dotaGamePlayers)
        {
            SpawnChampion(gamePlayer);
        }
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
    public void SpawnChampion(DotaNewRoomPlayer dotaGamePlayer)
    {
        StartCoroutine(SpawnChampionWhenConnectionReady(dotaGamePlayer));
    }

    [Server]
    IEnumerator SpawnChampionWhenConnectionReady(DotaNewRoomPlayer dotaGamePlayer)
    {
        Team championTeam = dotaGamePlayer.GetTeam();

        Vector3 spawnPosition = GetSpawnPosition(championTeam);

        GameObject championInstance = Instantiate(championPrefab, spawnPosition, Quaternion.identity);

        Champion champion = championInstance.GetComponent<Champion>();

        champion.OnChampionDead += Champion_OnChampionDead;

        champion.ServerSetTeam(championTeam);

        yield return new WaitUntil(() => dotaGamePlayer.connectionToClient != null);

        NetworkServer.Spawn(championInstance, dotaGamePlayer.connectionToClient);

        players.Add(championInstance.GetComponent<Champion>());

        List<DotaNewRoomPlayer> dotaGamePlayers = ((DotaNetworkManager)NetworkManager.singleton).GetServerPlayerList();

        if (players.Count == dotaGamePlayers.Count)
        {
            // At this point
            // 1. The Champion is all added to List On The Server Side
            // 2. The Champion is all spawned on the server side
            // BUT 
            // 1. The champion list is on client not synced with the server yet
            // 2. The champion might not be spawned yet on the client 

            //OnLocalChampionReady?.Invoke();
            RpcNotifyServerSpawnedAllChampion();
        }
    }

    private void Champion_OnChampionDead(Champion champion)
    {
        OnChampionRemoved?.Invoke(champion);
        StartCoroutine(ReviveChampionAfter(champion, 5f));
    }

    [Server]
    IEnumerator ReviveChampionAfter(Champion champion, float seconds)
    {
        yield return new WaitForSeconds(seconds);

        Vector3 spawnPos = GetSpawnPosition(champion.GetTeam());

        RpcTeleportChampionToSpawnPos(champion, spawnPos);

        champion.ServerRevive();
        
        OnChampionAdded?.Invoke(champion);
    }
    #endregion

    #region Client
    [ClientRpc]
    private void RpcNotifyServerSpawnedAllChampion()
    {
        StartCoroutine(InvokeWhenChampionListSynced());
    }

    [ClientRpc]
    private void RpcTeleportChampionToSpawnPos(Champion champion, Vector3 spawnPos)
    {
        champion.transform.position = spawnPos;
    }

    [Client]
    IEnumerator InvokeWhenChampionListSynced()
    {
        yield return new WaitForSeconds(0.5f);

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
