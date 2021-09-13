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

    public event Action<VisionEntity> OnVisionEntityExit;
    public event Action<VisionEntity> OnVisionEntityEnter;

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

        foreach (Champion player in players)
        {
            AddVisionEntity(player.GetComponent<VisionEntity>(), player.GetTeam());
        }
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

    //private void UpdateVisibleEnemy()
    //{
    //    foreach (VisionEntity enemy in enemies)
    //    {
    //        bool wasVisible = enemy.GetVisible();
    //        bool isVisible = false;

    //        foreach (VisionEntity ally in allies)
    //        {
    //            Vector3 direction = enemy.transform.position - ally.transform.position;
    //            float distance = direction.magnitude;

    //            if (distance < checkRadius && !Physics.Raycast(ally.transform.position, direction, distance, obstacleLayer))
    //            {
    //                isVisible = true;
    //                enemy.SetVisible(true);

    //                if (!wasVisible)
    //                {
    //                    Debug.Log("Enemy Enter Vision");
    //                    OnVisionEntityEnter?.Invoke(enemy);
    //                }
    //            }
    //            else
    //            {
    //                enemy.SetVisible(false);

    //                if (wasVisible)
    //                {
    //                    Debug.Log("Enemy Exit Vision");
    //                    OnVisionEntityExit?.Invoke(enemy);
    //                }
    //            }
    //        }
    //    }
    //}

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
                Debug.Log("Enemy Exit View");
                OnVisionEntityExit?.Invoke(enemy);
            }
            else if(!wasVisible && isVisible)
            {
                Debug.Log("Enemy Enter View");
                OnVisionEntityEnter?.Invoke(enemy);
            }

            enemy.SetVisible(isVisible);
        }
    }
}
