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

    public override void OnStartServer()
    {
        Debug.Log("PlayerManager OnStartServer");
        DotaNetworkManager.ServerOnAllClientSceneLoaded += DotaNetworkManager_ServerOnAllClientSceneLoaded;
    }

    private void DotaNetworkManager_ServerOnAllClientSceneLoaded(string scene)
    {
        List<DotaRoomPlayer> roomPlayers = ((DotaNetworkManager)NetworkManager.singleton).GetServerPlayers();
        SpawnChampionsForPlayers(roomPlayers);
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
    public void SpawnChampionForPlayer(DotaRoomPlayer player)
    {
        Team championTeam = player.GetTeam();

        Vector3 spawnPosition = GetSpawnPosition(championTeam);

        GameObject championInstance = Instantiate(championPrefab, spawnPosition, Quaternion.identity);

        Champion champion = championInstance.GetComponent<Champion>();

        champion.ServerSetTeam(championTeam);

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

    [Server]
    IEnumerator ReviveChampionAfter(Champion champion, float seconds)
    {
        yield return new WaitForSeconds(seconds);

        Vector3 spawnPos = GetSpawnPosition(champion.GetTeam());

        RpcTeleportChampionToSpawnPos(champion, spawnPos);

        champion.ServerRevive();
    }

    #region Client

    [ClientRpc]
    private void RpcTeleportChampionToSpawnPos(Champion champion, Vector3 spawnPos)
    {
        champion.transform.position = spawnPos;
    }
    #endregion

}
