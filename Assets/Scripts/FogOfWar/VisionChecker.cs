using Dota.Networking;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionChecker : NetworkBehaviour
{
    [SerializeField] Team localPlayerTeam;

    [SerializeField] MinionManager minionManager = null;
    [SerializeField] BuildingManager towerManager = null;
    [SerializeField] MonsterManager monsterManager = null;
    [SerializeField] float visionCheckHeight = 0.5f;

    [SerializeField] List<VisionEntity> allies = new List<VisionEntity>();
    [SerializeField] List<VisionEntity> enemies = new List<VisionEntity>();
    [SerializeField] LayerMask obstacleLayer = new LayerMask();
    [SerializeField] LayerMask grassLayer = new LayerMask();

    public event Action<VisionEntity> OnVisionEntityExit;
    public event Action<VisionEntity> OnVisionEntityEnter;

    public event Action<VisionEntity> OnVisionEntityAdded;
    public event Action<VisionEntity> OnVisionEntityRemoved;


    // Don't forget to unsubscribe
    private void Awake()
    {
        Champion.OnChampionSpawned += Champion_OnChampionSpawned;
        Champion.OnChampionDestroyed += Champion_OnChampionDestroyed;

        minionManager.OnMinionAdded += MinionManager_OnMinionAdded;
        minionManager.OnMinionRemoved += MinionManager_OnMinionRemoved;

        Tower.OnTowerSpawned += Tower_OnTowerSpawned;
        Tower.OnTowerDestroyed += Tower_OnTowerDestroyed;

        towerManager.OnBaseAdded += TowerManager_OnBaseAdded;

        monsterManager.OnMonsterAdded += MonsterManager_OnMonsterAdded;
        monsterManager.OnMonsterRemoved += MonsterManager_OnMonsterRemoved;
    }

    public override void OnStartClient()
    {
        localPlayerTeam = NetworkClient.localPlayer.GetComponent<DotaRoomPlayer>().GetTeam();
    }

    private void Tower_OnTowerDestroyed(Tower tower)
    {
        VisionEntity visionEntity = tower.GetComponent<VisionEntity>();
        RemoveVisionEntity(visionEntity, tower.GetTeam());
    }

    private void Tower_OnTowerSpawned(Tower tower)
    {
        VisionEntity visionEntity = tower.GetComponent<VisionEntity>();
        AddVisionEntity(visionEntity, tower.GetTeam());
    }

    private void Champion_OnChampionDestroyed(Champion champion)
    {
        VisionEntity visionEntity = champion.GetComponent<VisionEntity>();
        RemoveVisionEntity(visionEntity, champion.GetTeam());
    }

    private void Champion_OnChampionSpawned(Champion champion)
    {
        VisionEntity visionEntity = champion.GetComponent<VisionEntity>();
        AddVisionEntity(visionEntity, champion.GetTeam());
    }

    private void MonsterManager_OnMonsterRemoved(Monster monster)
    {
        Debug.Log("OnMonsterRemoved");
        VisionEntity visionEntity = monster.GetComponent<VisionEntity>();
        RemoveVisionEntity(visionEntity, monster.GetTeam());
    }

    private void MonsterManager_OnMonsterAdded(Monster monster)
    {
        Debug.Log("OnMonsterAdded");
        VisionEntity visionEntity = monster.GetComponent<VisionEntity>();
        AddVisionEntity(visionEntity, monster.GetTeam());
    }

    private void TowerManager_OnBaseAdded(Base teamBase)
    {
        VisionEntity visionEntity = teamBase.GetComponent<VisionEntity>();
        AddVisionEntity(visionEntity, teamBase.GetTeam());
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

    private void UpdateVisibleAlly()
    {
        foreach (VisionEntity ally in allies)
        {
            ally.SetVisible(true);
        }
    }

    private void OnDestroy()
    {
        
    }

    private void UpdateVisibleEnemy()
    {
        foreach (VisionEntity enemy in enemies)
        {
            if (enemy.IsAlwaysVisible)
            {
                enemy.SetVisible(true);
                continue;
            }

            bool wasVisible = enemy.GetVisible();
            bool isVisible = false;

            foreach (VisionEntity ally in allies)
            {

                float distance = Vector3.Distance(ally.transform.position, enemy.transform.position);
                if (distance > ally.GetViewRadius())
                {
                    isVisible = false;
                    continue;
                }

                Vector3 direction = enemy.transform.position - ally.transform.position;

                if (ally.IsInGrass())
                {
                    if (enemy.IsInGrass())
                    {
                        if((ally.GetCurrentBush() == enemy.GetCurrentBush()))
                        {
                            isVisible = true;
                            break;
                        }
                        else
                        {
                            isVisible = false;
                        }
                    }
                    else
                    {
                        bool hasObstacle = Physics.Raycast(ally.transform.position + Vector3.up * visionCheckHeight, direction, distance, obstacleLayer);
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
                }
                else
                {
                    LayerMask checkLayer = obstacleLayer | grassLayer;

                    bool hasObstacle = Physics.Raycast(ally.transform.position + Vector3.up * visionCheckHeight, direction, distance, checkLayer);
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
