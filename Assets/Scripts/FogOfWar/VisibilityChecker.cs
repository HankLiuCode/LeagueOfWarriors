using Dota.Networking;
using Mirror;
using System;
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


    public event Action OnAllPlayersAdded;
    public event Action<VisionEntity> OnVisionEntityAdded;
    public event Action<VisionEntity> OnVisionEntityRemoved;

    private void Awake()
    {
        ((DotaNetworkRoomManager)NetworkRoomManager.singleton).OnAllGamePlayersAdded += VisibilityChecker_OnAllPlayersAdded;

        minionManager.OnRedMinionAdded += MinionManager_OnRedMinionAdded;
        minionManager.OnRedMinionRemoved += MinionManager_OnRedMinionRemoved;
        minionManager.OnBlueMinionAdded += MinionManager_OnBlueMinionAdded;
        minionManager.OnBlueMinionRemoved += MinionManager_OnBlueMinionRemoved;

        DotaGamePlayer.OnDotaGamePlayerStop += DotaGamePlayer_OnDotaGamePlayerStop;
    }

    private void MinionManager_OnBlueMinionRemoved(NetworkIdentity obj)
    {
        switch (localPlayerTeam)
        {
            case Team.Red:
                enemies.Remove(obj.GetComponent<VisionEntity>());
                break;

            case Team.Blue:
                allies.Remove(obj.GetComponent<VisionEntity>());
                break;
        }
        OnVisionEntityRemoved?.Invoke(obj.GetComponent<VisionEntity>());
    }

    private void MinionManager_OnBlueMinionAdded(NetworkIdentity obj)
    {
        switch (localPlayerTeam)
        {
            case Team.Red:
                enemies.Add(obj.GetComponent<VisionEntity>());
                break;

            case Team.Blue:
                allies.Add(obj.GetComponent<VisionEntity>());
                break;
        }
        OnVisionEntityAdded?.Invoke(obj.GetComponent<VisionEntity>());
    }

    private void MinionManager_OnRedMinionAdded(NetworkIdentity obj)
    {
        switch (localPlayerTeam)
        {
            case Team.Red:
                allies.Add(obj.GetComponent<VisionEntity>());
                break;

            case Team.Blue:
                enemies.Add(obj.GetComponent<VisionEntity>());
                break;
        }
        OnVisionEntityAdded?.Invoke(obj.GetComponent<VisionEntity>());
    }

    private void MinionManager_OnRedMinionRemoved(NetworkIdentity obj)
    {
        switch (localPlayerTeam)
        {
            case Team.Red:
                allies.Remove(obj.GetComponent<VisionEntity>());
                break;

            case Team.Blue:
                enemies.Remove(obj.GetComponent<VisionEntity>());
                break;
        }
        OnVisionEntityRemoved?.Invoke(obj.GetComponent<VisionEntity>());
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

        OnAllPlayersAdded?.Invoke();
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
