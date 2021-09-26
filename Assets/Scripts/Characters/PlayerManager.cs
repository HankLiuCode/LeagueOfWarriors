using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dota.Networking;

public class PlayerManager : NetworkBehaviour
{
    [Scene] 
    [SerializeField]
    string gameScene;

    [SerializeField] GameObject championPrefab;
    [SerializeField] Transform[] blueStartPositions;
    [SerializeField] Transform[] redStartPositions;

    [SerializeField] List<Champion> serverChampions = new List<Champion>();

    [SerializeField] float deathTime = 3f;

    int blueStartPositionIndex = 0;
    int redStartPositionIndex = 0;

    public override void OnStartServer()
    {
        DotaNetworkManager.ServerOnAllClientSceneLoaded += DotaNetworkManager_ServerOnAllClientSceneLoaded;
        DotaNetworkManager.ServerOnClientDisconnect += DotaNetworkManager_ServerOnClientDisconnect;
        Champion.ServerOnChampionDead += Champion_ServerOnChampionDead;
    }

    public override void OnStopServer()
    {
        DotaNetworkManager.ServerOnAllClientSceneLoaded -= DotaNetworkManager_ServerOnAllClientSceneLoaded;
        DotaNetworkManager.ServerOnClientDisconnect -= DotaNetworkManager_ServerOnClientDisconnect;
        Champion.ServerOnChampionDead -= Champion_ServerOnChampionDead;
    }

    [Server]
    private void DotaNetworkManager_ServerOnClientDisconnect(DotaRoomPlayer player)
    {
        foreach(Champion champion in serverChampions)
        {
            if(champion.GetOwner() == player)
            {
                champion.netIdentity.RemoveClientAuthority();
                Debug.Log("Champion Authority Removed");
            }
        }
    }

    [Server]
    private void Champion_ServerOnChampionDead(Champion champion)
    {
        serverChampions.Remove(champion);
        StartCoroutine(SpawnChampionForPlayerAfterSeconds(champion.GetOwner(), deathTime));
    }

    [Server]
    private void DotaNetworkManager_ServerOnAllClientSceneLoaded(string scene)
    {
        if(scene == gameScene)
        {
            List<DotaRoomPlayer> roomPlayers = ((DotaNetworkManager)NetworkManager.singleton).GetServerPlayers();
            SpawnChampionsForPlayers(roomPlayers);
        }
    }

    [Server]
    public void SpawnChampionsForPlayers(List<DotaRoomPlayer> players)
    {
        foreach(DotaRoomPlayer player in players)
        {
            SpawnChampionForPlayer(player);
        }
    }

    [Server]
    IEnumerator SpawnChampionForPlayerAfterSeconds(DotaRoomPlayer player, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SpawnChampionForPlayer(player);
    }

    [Server]
    public void SpawnChampionForPlayer(DotaRoomPlayer player)
    {
        Team championTeam = player.GetTeam();

        Vector3 spawnPosition = GetSpawnPosition(championTeam);

        GameObject championInstance = Instantiate(championPrefab, spawnPosition, Quaternion.identity);

        Champion champion = championInstance.GetComponent<Champion>();

        champion.ServerSetOwner(player);

        champion.ServerSetTeam(championTeam);

        serverChampions.Add(champion);

        NetworkServer.Spawn(championInstance, player.connectionToClient);
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

    #region Client
    [ClientRpc]
    private void RpcTeleportChampionToSpawnPos(Champion champion, Vector3 spawnPos)
    {
        champion.transform.position = spawnPos;
    }
    #endregion

}
