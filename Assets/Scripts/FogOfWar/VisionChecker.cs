using Dota.Networking;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionChecker : MonoBehaviour
{
    [SerializeField] Team localPlayerTeam;
    [SerializeField] float checkRadius = 10f;

    [SerializeField] MinionManager minionManager = null;
    [SerializeField] PlayerManager playerManager = null;
    [SerializeField] BuildingManager towerManager = null;

    [SerializeField] List<VisionEntity> allies = new List<VisionEntity>();
    [SerializeField] List<VisionEntity> enemies = new List<VisionEntity>();
    [SerializeField] LayerMask obstacleLayer = new LayerMask();

    public event Action<VisionEntity> OnVisionEntityExit;
    public event Action<VisionEntity> OnVisionEntityEnter;

    public event Action<VisionEntity> OnVisionEntityAdded;
    public event Action<VisionEntity> OnVisionEntityRemoved;

    private void Awake()
    {
        playerManager.OnLocalChampionReady += PlayerManager_OnLocalChampionReady;

        playerManager.OnChampionAdded += PlayerManager_OnChampionAdded;
        playerManager.OnChampionRemoved += PlayerManager_OnChampionRemoved;

        minionManager.OnMinionAdded += MinionManager_OnMinionAdded;
        minionManager.OnMinionRemoved += MinionManager_OnMinionRemoved;

        towerManager.OnTowerAdded += TowerManager_OnTowerAdded;
        towerManager.OnTowerRemoved += TowerManager_OnTowerRemoved;
    }

    private void TowerManager_OnTowerRemoved(Tower tower)
    {
        VisionEntity visionEntity = tower.GetComponent<VisionEntity>();
        RemoveVisionEntity(visionEntity, tower.GetTeam());
    }

    private void TowerManager_OnTowerAdded(Tower tower)
    {
        VisionEntity visionEntity = tower.GetComponent<VisionEntity>();
        AddVisionEntity(visionEntity, tower.GetTeam());
    }

    private void PlayerManager_OnLocalChampionReady()
    {
        Champion localPlayer = playerManager.GetLocalChampion();
        localPlayerTeam = localPlayer.GetTeam();
        SyncList<Champion> players = playerManager.GetChampions();

        foreach (Champion player in players)
        {
            AddVisionEntity(player.GetComponent<VisionEntity>(), player.GetTeam());
        }
    }

    private void PlayerManager_OnChampionRemoved(Champion champion)
    {
        VisionEntity visionEntity = champion.GetComponent<VisionEntity>();
        RemoveVisionEntity(visionEntity, champion.GetTeam());
    }

    private void PlayerManager_OnChampionAdded(Champion champion)
    {
        VisionEntity visionEntity = champion.GetComponent<VisionEntity>();
        AddVisionEntity(visionEntity, champion.GetTeam());
    }

    private void MinionManager_OnMinionAdded(Minion minion)
    {
        VisionEntity visionEntity = minion.GetComponent<VisionEntity>();
        AddVisionEntity(visionEntity, minion.GetTeam());
    }

    private void MinionManager_OnMinionRemoved(Minion minion)
    {
        VisionEntity visionEntity = minion.GetComponent<VisionEntity>();
        RemoveVisionEntity(visionEntity, minion.GetTeam());
    }

    private void RemoveVisionEntity(VisionEntity visionEntity, Team team)
    {
        if (localPlayerTeam == team)
        {
            allies.Remove(visionEntity);
        }
        else
        {
            enemies.Remove(visionEntity);
        }

        OnVisionEntityRemoved?.Invoke(visionEntity);
    }

    private void AddVisionEntity(VisionEntity visionEntity, Team team)
    {
        if (localPlayerTeam == team)
        {
            allies.Add(visionEntity);
        }
        else
        {
            enemies.Add(visionEntity);
        }

        OnVisionEntityAdded?.Invoke(visionEntity);
    }

    public List<VisionEntity> GetVisible()
    {
        List<VisionEntity> visible = new List<VisionEntity>(allies);
        foreach (VisionEntity enemy in enemies)
        {
            if (enemy.GetVisible())
            {
                visible.Add(enemy);
            }
        }
        return visible;
    }

    public List<VisionEntity> GetAll()
    {
        List<VisionEntity> all = new List<VisionEntity>(allies);
        all.AddRange(enemies);
        return all;
    }

    private void Update()
    {
        UpdateVisibleEnemy();
    }

    private void UpdateVisibleEnemy()
    {
        foreach (VisionEntity enemy in enemies)
        {
            bool wasVisible = enemy.GetVisible();
            bool isVisible = false;

            foreach (VisionEntity ally in allies)
            {
                float distance = Vector3.Distance(ally.transform.position, enemy.transform.position);
                if (distance > checkRadius)
                {
                    isVisible = false;
                    continue;
                }

                Vector3 direction = enemy.transform.position - ally.transform.position;
                bool hasObstacle = Physics.Raycast(ally.transform.position, direction, distance, obstacleLayer);

                if (hasObstacle)
                {
                    isVisible = false;
                }
                else
                {
                    isVisible = true;
                    break;
                }
            }

            if (wasVisible && !isVisible)
            {
                OnVisionEntityExit?.Invoke(enemy);
            }
            else if(!wasVisible && isVisible)
            {
                OnVisionEntityEnter?.Invoke(enemy);
            }

            enemy.SetVisible(isVisible);
        }
    }
}
