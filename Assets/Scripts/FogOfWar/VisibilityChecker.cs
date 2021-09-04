using Dota.Networking;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityChecker : MonoBehaviour
{
    [SerializeField] Team localPlayerTeam;
    [SerializeField] float checkRadius = 10f;
    [SerializeField] List<GameObject> allies = new List<GameObject>();
    [SerializeField] List<GameObject> enemies = new List<GameObject>();
    [SerializeField] List<GameObject> enemiesInViewRadius = new List<GameObject>();
    [SerializeField] LayerMask visionCheckLayer = new LayerMask();
    
    private void Start()
    {
        ((DotaNetworkRoomManager)NetworkRoomManager.singleton).OnAllPlayersAdded += VisibilityChecker_OnAllPlayersAdded;
    }

    private void VisibilityChecker_OnAllPlayersAdded()
    {
        localPlayerTeam = ((DotaNetworkRoomManager)NetworkRoomManager.singleton).GetLocalGamePlayer().GetTeam();

        List<DotaGamePlayer> sameTeamGamePlayers;
        List<DotaGamePlayer> otherTeamGamePlayers;

        switch (localPlayerTeam)
        {
            case Team.Red:
                sameTeamGamePlayers = ((DotaNetworkRoomManager) NetworkRoomManager.singleton).ClientGetRedTeamGamePlayers();
                foreach (DotaGamePlayer sameTeamGamePlayer in sameTeamGamePlayers)
                {
                    allies.Add(sameTeamGamePlayer.gameObject);
                }

                otherTeamGamePlayers = ((DotaNetworkRoomManager) NetworkRoomManager.singleton).ClientGetBlueTeamGamePlayers();
                foreach (DotaGamePlayer otherTeamGamePlayer in otherTeamGamePlayers)
                {
                    enemies.Add(otherTeamGamePlayer.gameObject);
                }
                break;

            case Team.Blue:
                sameTeamGamePlayers = ((DotaNetworkRoomManager)NetworkRoomManager.singleton).ClientGetBlueTeamGamePlayers();
                foreach (DotaGamePlayer gamePlayer in sameTeamGamePlayers)
                {
                    allies.Add(gamePlayer.gameObject);
                }

                otherTeamGamePlayers = ((DotaNetworkRoomManager)NetworkRoomManager.singleton).ClientGetRedTeamGamePlayers();
                foreach (DotaGamePlayer otherTeamGamePlayer in otherTeamGamePlayers)
                {
                    enemies.Add(otherTeamGamePlayer.gameObject);
                }
                break;
        }
    }

    private void Update()
    {
        //UpdateVisibleEnemyList();
        //AdjustEnemyVisibility();
    }

    public void AdjustEnemyVisibility()
    {
        foreach(GameObject enemy in enemies)
        {
            SkinnedMeshRenderer renderer = enemy.GetComponentInChildren<SkinnedMeshRenderer>();
            renderer.enabled = false;
        }

        foreach(GameObject visibleEnemy in enemiesInViewRadius)
        {
            SkinnedMeshRenderer renderer = visibleEnemy.GetComponentInChildren<SkinnedMeshRenderer>();
            renderer.enabled = true;
        }
    }


    public void UpdateVisibleEnemyList()
    {
        enemies.Clear();
        foreach(GameObject ally in allies)
        {
            Collider[] colliders = Physics.OverlapSphere(ally.transform.position, checkRadius, visionCheckLayer);
            foreach(Collider c in colliders)
            {
                if(c.tag != localPlayerTeam.ToString())
                {
                    enemiesInViewRadius.Add(c.gameObject);
                }
            }
        }
    }
}
