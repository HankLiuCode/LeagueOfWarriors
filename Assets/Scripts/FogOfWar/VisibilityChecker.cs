using Dota.Networking;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityChecker : MonoBehaviour
{
    [SerializeField] Team localPlayerTeam;
    [SerializeField] float checkRadius = 10f;
    [SerializeField] MinionManager minionManager = null;

    [SerializeField] List<VisionEntity> allies = new List<VisionEntity>();
    [SerializeField] List<VisionEntity> enemies = new List<VisionEntity>();
    
    [SerializeField] LayerMask obstacleLayer = new LayerMask();
    
    private void Start()
    {
        ((DotaNetworkRoomManager)NetworkRoomManager.singleton).OnAllGamePlayersAdded += VisibilityChecker_OnAllPlayersAdded;
        minionManager.OnMinionAdded += MinionManager_OnMinionAdded;
        minionManager.OnMinionRemoved += MinionManager_OnMinionRemoved;

        DotaGamePlayer.OnDotaGamePlayerStop += DotaGamePlayer_OnDotaGamePlayerStop;
    }

    private void MinionManager_OnMinionAdded(NetworkIdentity obj)
    {
        allies.Add(obj.GetComponent<VisionEntity>());
    }

    private void MinionManager_OnMinionRemoved(NetworkIdentity obj)
    {
        allies.Remove(obj.GetComponent<VisionEntity>());
    }

    private void DotaGamePlayer_OnDotaGamePlayerStop(DotaGamePlayer dotaGamePlayer)
    {
        // Remove DotaPlayer From allies ?
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

        List<DotaGamePlayer> blueTeamPlayers = ((DotaNetworkRoomManager)NetworkRoomManager.singleton).ClientGetBlueTeamGamePlayers();
        List<DotaGamePlayer> redTeamPlayers = ((DotaNetworkRoomManager)NetworkRoomManager.singleton).ClientGetRedTeamGamePlayers();

        switch (localPlayerTeam)
        {
            case Team.Red:
                foreach (DotaGamePlayer redPlayer in redTeamPlayers)
                {
                    allies.Add(redPlayer.GetComponent<VisionEntity>());
                }
                foreach (DotaGamePlayer bluePlayer in blueTeamPlayers)
                {
                    enemies.Add(bluePlayer.GetComponent<VisionEntity>());
                }
                break;

            case Team.Blue:
                foreach (DotaGamePlayer bluePlayer in blueTeamPlayers)
                {
                    allies.Add(bluePlayer.GetComponent<VisionEntity>());
                }
                foreach (DotaGamePlayer redPlayer in redTeamPlayers)
                {
                    enemies.Add(redPlayer.GetComponent<VisionEntity>());
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
