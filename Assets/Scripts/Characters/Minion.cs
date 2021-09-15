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
    [SyncVar]
    Team team;

    [SerializeField] Animator animator = null;
    [SerializeField] ServerMover serverMover = null;
    [SerializeField] ServerFighter serverFighter = null;
    [SerializeField] Health health = null;
    [SerializeField] Sprite icon = null;
    [SerializeField] Sprite minimapIcon = null;
    [SerializeField] GameObject minimapIconPrefab = null;

    [SerializeField] float noticeRadius = 5f;
    [SerializeField] float attackRadius = 3f;
    [SerializeField] LayerMask enemyLayerMask;

    Transform currentTarget;
    Transform[] towers = null;
    Transform targetBase = null;
    [SerializeField] List<Health> enemyList = new List<Health>();
    List<Health> toRemove = new List<Health>();


    private void Start()
    {
        health.OnHealthDeadEnd += Health_OnHealthDeadEnd;
    }

    private void Health_OnHealthDeadEnd()
    {
        Destroy(gameObject);
    }

    #region Server

    public void SetTeam(Team team)
    {
        this.team = team;
        gameObject.tag = team.ToString();
    }

    public void SetTowers(Transform[] towers, Transform targetBase)
    {
        transform.forward = towers[2].position - transform.position;
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
            if (health.IsDead())
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
                return towers[i];
            }
        }
        return targetBase;
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

    public Sprite GetMinimapIcon()
    {
        return minimapIcon;
    }

    public MinimapIcon GetMinimapIconInstance()
    {
        MinimapIcon minimapIconInstance = Instantiate(minimapIconPrefab).GetComponent<MinimapIcon>();
        minimapIconInstance.SetVisible(false);
        minimapIconInstance.SetTeam(team);

        return minimapIconInstance;
    }

    public string GetLayerName()
    {
        return "Minion";
    }
}
