using Dota.Networking;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityChecker : MonoBehaviour
{
    [SerializeField] Team localPlayerTeam;
    [SerializeField] float checkRadius = 10f;
    [SerializeField] List<VisionEntity> allies = new List<VisionEntity>();
    [SerializeField] List<VisionEntity> enemies = new List<VisionEntity>();
    
    [SerializeField] LayerMask obstacleLayer = new LayerMask();
    [SerializeField] LayerMask visionCheckLayer = new LayerMask();
    
    private void Start()
    {
        ((DotaNetworkRoomManager)NetworkRoomManager.singleton).OnAllGamePlayersAdded += VisibilityChecker_OnAllPlayersAdded;
        DotaGamePlayer.OnDotaGamePlayerStop += DotaGamePlayer_OnDotaGamePlayerStop;
    }

    private void DotaGamePlayer_OnDotaGamePlayerStop()
    {
        
    }

    public List<VisionEntity> GetEnemies()
    {
        return enemies;
    }

    public List<VisionEntity> GetAllies()
    {
        return allies;
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
                    allies.Add(sameTeamGamePlayer.GetComponent<VisionEntity>());
                }

                otherTeamGamePlayers = ((DotaNetworkRoomManager) NetworkRoomManager.singleton).ClientGetBlueTeamGamePlayers();
                foreach (DotaGamePlayer otherTeamGamePlayer in otherTeamGamePlayers)
                {
                    enemies.Add(otherTeamGamePlayer.GetComponent<VisionEntity>());
                }
                break;

            case Team.Blue:
                sameTeamGamePlayers = ((DotaNetworkRoomManager)NetworkRoomManager.singleton).ClientGetBlueTeamGamePlayers();
                foreach (DotaGamePlayer gamePlayer in sameTeamGamePlayers)
                {
                    allies.Add(gamePlayer.GetComponent<VisionEntity>());
                }

                otherTeamGamePlayers = ((DotaNetworkRoomManager)NetworkRoomManager.singleton).ClientGetRedTeamGamePlayers();
                foreach (DotaGamePlayer otherTeamGamePlayer in otherTeamGamePlayers)
                {
                    enemies.Add(otherTeamGamePlayer.GetComponent<VisionEntity>());
                }
                break;
        }
    }

    private void Update()
    {
        UpdateVisibleEnemy();
    }

    private void UpdateVisibleEnemy()
    {
        foreach(VisionEntity enemy in enemies)
        {
            bool isEnemyVisible = false;
            foreach (VisionEntity ally in allies)
            {
                float distance = Vector3.Distance(ally.transform.position, enemy.transform.position);
                if (distance > checkRadius)
                {
                    isEnemyVisible = false;
                    continue;
                }

                Vector3 direction = enemy.transform.position - ally.transform.position;
                bool hasObstacle = Physics.Raycast(ally.transform.position, direction, distance, obstacleLayer);

                if (hasObstacle)
                {
                    isEnemyVisible = false;
                }
                else
                {
                    isEnemyVisible = true;
                    break;
                }
            }
            enemy.SetVisible(isEnemyVisible);
        }
    }
}
