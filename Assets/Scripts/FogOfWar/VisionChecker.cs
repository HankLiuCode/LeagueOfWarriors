using Dota.Networking;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Operation
{
    AddAlly,
    RemoveAlly,
    AddEnemy,
    RemoveEnemy
}

public struct ModifyRequest
{
    public VisionEntity visionEntity;
    public Operation operation;
    public ModifyRequest(VisionEntity visionEntity, Operation operation)
    {
        this.visionEntity = visionEntity;
        this.operation = operation;
    }
}

public class VisionChecker : NetworkBehaviour
{
    [SerializeField] Team localPlayerTeam;

    [SerializeField] MonsterSpawner monsterManager = null;
    [SerializeField] float visionCheckHeight = 0.5f;

    [SerializeField] List<VisionEntity> allies = new List<VisionEntity>();
    [SerializeField] List<VisionEntity> enemies = new List<VisionEntity>();
    [SerializeField] LayerMask obstacleLayer = new LayerMask();
    [SerializeField] LayerMask grassLayer = new LayerMask();

    List<ModifyRequest> modifyRequests = new List<ModifyRequest>();

    public event Action<VisionEntity> OnVisionEntityExit;
    public event Action<VisionEntity> OnVisionEntityEnter;

    public event Action<VisionEntity> OnVisionEntityAdded;
    public event Action<VisionEntity> OnVisionEntityRemoved;


    // Don't forget to unsubscribe
    private void Awake()
    {
        Team team = NetworkClient.localPlayer.GetComponent<DotaRoomPlayer>().GetTeam();
        localPlayerTeam = team;

        Champion.OnChampionSpawned += Champion_OnChampionSpawned;
        Champion.ClientOnChampionDead += Champion_ClientOnChampionDead;

        Tower.OnTowerSpawned += Tower_OnTowerSpawned;
        Tower.ClientOnTowerDead += Tower_ClientOnTowerDead;

        Base.OnBaseSpawned += Base_OnBaseSpawned;
        Base.ClientOnBaseDead += Base_ClientOnBaseDead;

        Minion.OnMinionSpawned += Minion_OnMinionSpawned;
        Minion.ClientOnMinionDead += Minion_ClientOnMinionDead;

        Monster.OnMonsterSpawned += Monster_OnMonsterSpawned;
        Monster.ClientOnMonsterDead += Monster_ClientOnMonsterDead;
    }

    #region Client

    private void Monster_ClientOnMonsterDead(Monster monster)
    {
        VisionEntity visionEntity = monster.GetComponent<VisionEntity>();
        RemoveVisionEntity(visionEntity, monster.GetTeam());
    }

    private void Monster_OnMonsterSpawned(Monster monster)
    {
        VisionEntity visionEntity = monster.GetComponent<VisionEntity>();
        AddVisionEntity(visionEntity, monster.GetTeam());
    }

    private void Base_OnBaseSpawned(Base teamBase)
    {
        VisionEntity visionEntity = teamBase.GetComponent<VisionEntity>();
        AddVisionEntity(visionEntity, teamBase.GetTeam());
    }

    private void Base_ClientOnBaseDead(Base teamBase)
    {
        VisionEntity visionEntity = teamBase.GetComponent<VisionEntity>();
        RemoveVisionEntity(visionEntity, teamBase.GetTeam());
    }

    private void Minion_OnMinionSpawned(Minion minion)
    {
        VisionEntity visionEntity = minion.GetComponent<VisionEntity>();
        AddVisionEntity(visionEntity, minion.GetTeam());
    }

    private void Minion_ClientOnMinionDead(Minion minion)
    {
        VisionEntity visionEntity = minion.GetComponent<VisionEntity>();
        RemoveVisionEntity(visionEntity, minion.GetTeam());
    }

    private void Tower_ClientOnTowerDead(Tower tower)
    {
        VisionEntity visionEntity = tower.GetComponent<VisionEntity>();
        RemoveVisionEntity(visionEntity, tower.GetTeam());
    }

    private void Tower_OnTowerSpawned(Tower tower)
    {
        VisionEntity visionEntity = tower.GetComponent<VisionEntity>();
        AddVisionEntity(visionEntity, tower.GetTeam());
    }

    private void Champion_ClientOnChampionDead(Champion champion)
    {
        VisionEntity visionEntity = champion.GetComponent<VisionEntity>();
        RemoveVisionEntity(visionEntity, champion.GetTeam());
    }

    private void Champion_OnChampionSpawned(Champion champion)
    {
        VisionEntity visionEntity = champion.GetComponent<VisionEntity>();
        AddVisionEntity(visionEntity, champion.GetTeam());
    }

    private void AddVisionEntity(VisionEntity visionEntity, Team team)
    {
        if (localPlayerTeam == team)
        {
            modifyRequests.Add(new ModifyRequest(visionEntity, Operation.AddAlly));
        }
        else
        {
            modifyRequests.Add(new ModifyRequest(visionEntity, Operation.AddEnemy));
        }
    }

    private void RemoveVisionEntity(VisionEntity visionEntity, Team team)
    {
        if (localPlayerTeam == team)
        {
            modifyRequests.Add(new ModifyRequest(visionEntity, Operation.RemoveAlly));
        }
        else
        {
            modifyRequests.Add(new ModifyRequest(visionEntity, Operation.RemoveEnemy));
        }
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
        UpdateAllyEnemyList();
        AdjustEnemyVisibility();
    }

    void UpdateAllyEnemyList()
    {
        foreach(ModifyRequest request in modifyRequests)
        {
            switch (request.operation)
            {
                case Operation.AddAlly:
                    allies.Add(request.visionEntity);
                    request.visionEntity.SetVisible(true);
                    OnVisionEntityAdded?.Invoke(request.visionEntity);
                    OnVisionEntityEnter?.Invoke(request.visionEntity);
                    break;

                case Operation.RemoveAlly:
                    allies.Remove(request.visionEntity);
                    OnVisionEntityRemoved?.Invoke(request.visionEntity);
                    break;

                case Operation.AddEnemy:
                    enemies.Add(request.visionEntity);
                    OnVisionEntityAdded?.Invoke(request.visionEntity);
                    break;

                case Operation.RemoveEnemy:
                    enemies.Remove(request.visionEntity);
                    OnVisionEntityRemoved?.Invoke(request.visionEntity);
                    break;
            }
        }
        modifyRequests.Clear();
    }

    private void AdjustEnemyVisibility()
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
                        if ((ally.GetCurrentBush() == enemy.GetCurrentBush()))
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
            else if (!wasVisible && isVisible)
            {
                OnVisionEntityEnter?.Invoke(enemy);
            }

            enemy.SetVisible(isVisible);
        }
    }

    private void OnDestroy()
    {
        Champion.OnChampionSpawned -= Champion_OnChampionSpawned;
        Champion.ClientOnChampionDead -= Champion_ClientOnChampionDead;

        Tower.OnTowerSpawned -= Tower_OnTowerSpawned;
        Tower.ClientOnTowerDead -= Tower_ClientOnTowerDead;

        Base.OnBaseSpawned -= Base_OnBaseSpawned;
        Base.ClientOnBaseDead -= Base_ClientOnBaseDead;

        Minion.OnMinionSpawned -= Minion_OnMinionSpawned;
        Minion.ClientOnMinionDead -= Minion_ClientOnMinionDead;

        Monster.OnMonsterSpawned -= Monster_OnMonsterSpawned;
        Monster.ClientOnMonsterDead -= Monster_ClientOnMonsterDead;
    }

    #endregion
}