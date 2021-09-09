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

    [SerializeField] List<VisionEntity> allies = new List<VisionEntity>();
    [SerializeField] List<VisionEntity> enemies = new List<VisionEntity>();
    [SerializeField] LayerMask obstacleLayer = new LayerMask();

    public event Action OnAllPlayersAdded;
    public event Action<VisionEntity> OnVisionEntityAdded;
    public event Action<VisionEntity> OnVisionEntityRemoved;

    private void Awake()
    {
        playerManager.OnLocalChampionReady += PlayerManager_OnLocalChampionReady;
        minionManager.OnMinionAdded += MinionManager_OnMinionAdded;
        minionManager.OnMinionRemoved += MinionManager_OnMinionRemoved;
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

    private void MinionManager_OnMinionAdded(Minion minion)
    {
        VisionEntity visionEntity = minion.GetComponent<VisionEntity>();
        if(localPlayerTeam == minion.GetTeam())
        {
            allies.Add(visionEntity);
        }
        else
        {
            enemies.Add(visionEntity);
        }

        OnVisionEntityAdded?.Invoke(minion.GetComponent<VisionEntity>());
    }

    private void MinionManager_OnMinionRemoved(Minion minion)
    {
        VisionEntity visionEntity = minion.GetComponent<VisionEntity>();
        if (localPlayerTeam == minion.GetTeam())
        {
            allies.Remove(visionEntity);
        }
        else
        {
            enemies.Remove(visionEntity);
        }

        OnVisionEntityRemoved?.Invoke(minion.GetComponent<VisionEntity>());
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
