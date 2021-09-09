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
    [SerializeField] PlayerManager playerManager = null;

    [SerializeField] List<VisionEntity> allies = new List<VisionEntity>();
    [SerializeField] List<VisionEntity> enemies = new List<VisionEntity>();
    
    [SerializeField] LayerMask obstacleLayer = new LayerMask();


    public event Action OnAllPlayersAdded;
    public event Action<VisionEntity> OnVisionEntityAdded;
    public event Action<VisionEntity> OnVisionEntityRemoved;

    private void Awake()
    {
        playerManager.OnLocalChampionReady += PlayerManager_OnLocalChampionReady;

        minionManager.OnRedMinionAdded += MinionManager_OnRedMinionAdded;
        minionManager.OnRedMinionRemoved += MinionManager_OnRedMinionRemoved;
        minionManager.OnBlueMinionAdded += MinionManager_OnBlueMinionAdded;
        minionManager.OnBlueMinionRemoved += MinionManager_OnBlueMinionRemoved;
    }

    private void PlayerManager_OnLocalChampionReady()
    {
        Champion localPlayer = playerManager.GetLocalChampion();
        localPlayerTeam = localPlayer.GetTeam();

        SyncList<Champion> players = playerManager.GetChampions();

        foreach(Champion player in players)
        {
            if(player.GetTeam() == localPlayerTeam)
            {
                allies.Add(player.GetComponent<VisionEntity>());
            }
            else
            {
                enemies.Add(player.GetComponent<VisionEntity>());
            }
        }
        OnAllPlayersAdded?.Invoke();
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

    public List<VisionEntity> GetEnemies()
    {
        return enemies;
    }

    public List<VisionEntity> GetAllies()
    {
        return allies;
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
