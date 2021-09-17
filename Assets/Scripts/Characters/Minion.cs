using Dota.Attributes;
using Dota.Utils;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Minion : NetworkBehaviour, ITeamMember, IIconOwner, IMinimapEntity
{
    [SerializeField] 
    Team team;

    [SerializeField] Animator animator = null;
    [SerializeField] ServerMover serverMover = null;
    [SerializeField] ServerFighter serverFighter = null;
    [SerializeField] Health health = null;

    [SerializeField] Sprite icon = null;
    [SerializeField] GameObject minimapIconPrefab = null;

    [SerializeField] float noticeRadius = 5f;
    [SerializeField] float attackRadius = 3f;
    [SerializeField] LayerMask enemyLayerMask;

    Transform currentTarget;
    Tower[] towers = null;
    Base targetBase = null;
    [SerializeField] List<Health> enemyList = new List<Health>();
    List<Health> toRemove = new List<Health>();


    private void Start()
    {
        health.OnHealthDeadEnd += Health_OnServerHealthDeadEnd;
    }

    private void Health_OnServerHealthDeadEnd()
    {
        NetworkServer.Destroy(gameObject);
    }

    #region Server

    public void SetTeam(Team team)
    {
        this.team = team;
        gameObject.tag = team.ToString();
    }

    public void SetTowers(Tower[] towers, Base targetBase)
    {
        this.towers = towers;
        this.targetBase = targetBase;
    }

    public void SetRoad(int road)
    {
        serverMover.SetAreaMask(road);
    }

    [ServerCallback]
    void Update()
    {
        currentTarget = GetTarget();
        serverFighter.StartAttack(currentTarget.gameObject);
    }


    #region Client

    [ClientRpc]
    private void RpcNotifyTeamChanged(Team team)
    {

    }

    #endregion

    Transform GetTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, noticeRadius, enemyLayerMask);

        foreach(Collider c in colliders)
        {
            if(!TeamChecker.IsSameTeam(c.gameObject, gameObject))
            {
                Health health = c.GetComponent<Health>();
                if (health != null && !enemyList.Contains(health))
                {
                    enemyList.Add(health);
                }
            }
        }

        foreach (Health health in enemyList)
        {
            if (health.IsDead() || VectorConvert.XZDistance(health.transform.position, transform.position) > noticeRadius)
            {
                toRemove.Add(health);
            }
        }

        foreach (Health health in toRemove)
        {
            enemyList.Remove(health);
        }

        toRemove.Clear();


        if (enemyList.Count >= 1)
        {
            return enemyList[0].transform;
        }


        for (int i = towers.Length - 1; i >= 0; i--)
        {
            if (!towers[i].GetComponent<Health>().IsDead())
            {
                return towers[i].transform;
            }
        }
        return targetBase.transform;
    }
    #endregion

    public Team GetTeam()
    {
        return team;
    }

    public Sprite GetIcon()
    {
        return icon;
    }

    public MinimapIcon GetMinimapIconInstance()
    {
        MinimapMinionIcon minimapIconInstance = Instantiate(minimapIconPrefab).GetComponent<MinimapMinionIcon>();
        minimapIconInstance.SetVisible(false);
        minimapIconInstance.SetTeam(team);

        return minimapIconInstance;
    }

    public string GetLayerName()
    {
        return "Minion";
    }
}
