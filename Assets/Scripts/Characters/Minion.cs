using Dota.Attributes;
using Dota.Utils;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Minion : NetworkBehaviour, ITeamMember, IIconOwner, IMinimapEntity
{
    [SyncVar]
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
    [SerializeField] float checkInterval = 0.2f;
    [SerializeField] LayerMask attackLayer;


    [SerializeField] CombatTarget currentTarget;

    Collider[] colliderBuffer = new Collider[10];
    List<Tower> towers = new List<Tower>();
    Base targetBase = null;
    
    private void Start()
    {
        health.OnHealthDeadEnd += Health_OnServerHealthDeadEnd;
        if (isServer)
        {
            StartCoroutine(GetTargetRoutine());
        }
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
        this.towers = new List<Tower>(towers);
        this.targetBase = targetBase;
    }

    public void SetRoad(int road)
    {
        serverMover.SetAreaMask(road);
    }

    [ServerCallback]
    void Update()
    {
        if(currentTarget != null)
        {
            if (currentTarget.GetHealth().IsDead())
            {
                currentTarget = GetTarget();
            }
            serverFighter.StartAttack(currentTarget.gameObject);
        }
    }

    IEnumerator GetTargetRoutine()
    {
        while (true)
        {
            UpdateCurrentTarget();
            yield return new WaitForSeconds(checkInterval);
        }
    }

    private void UpdateCurrentTarget()
    {
        if (currentTarget == null || currentTarget.GetHealth().IsDead())
        {
            currentTarget = GetTarget();
        }
        else
        {
            if (IsBuildingTarget(currentTarget) || IsOutOfNoticeRange(currentTarget))
            {
                currentTarget = GetTarget();
            }
        }
    }

    private bool IsOutOfNoticeRange(CombatTarget target)
    {
        if(VectorConvert.XZDistance(target.transform.position, transform.position) > noticeRadius)
        {
            return true;
        }
        return false;
    }

    private bool IsBuildingTarget(CombatTarget target)
    {
        Tower tower = target.GetComponent<Tower>();
        if (tower != null)
        {
            return true;
        }
        return false;
    }

    private CombatTarget GetTarget()
    {
        CombatTarget characterTarget = GetCharacterTarget();
        if(characterTarget == null)
        {
            return GetBuildingTarget();
        }
        return characterTarget;
    }

    private CombatTarget GetCharacterTarget()
    {
        int targetCount = Physics.OverlapSphereNonAlloc(transform.position, noticeRadius, colliderBuffer, attackLayer);

        for (int i = 0; i < targetCount; i++)
        {
            GameObject colliderGameObject = colliderBuffer[i].gameObject;

            if (colliderGameObject == gameObject) { continue; }

            if ((!TeamChecker.IsSameTeam(gameObject, colliderGameObject)))
            {
                CombatTarget combatTarget = colliderGameObject.GetComponent<CombatTarget>();

                return combatTarget;
            }
        }
        return null;
    }

    private CombatTarget GetBuildingTarget()
    {
        for(int i= towers.Count - 1; i >= 0; i--)
        {
            if(towers[i] != null && !towers[i].GetComponent<CombatTarget>().GetHealth().IsDead())
            {
                CombatTarget combatTarget = towers[i].GetComponent<CombatTarget>();
                if(combatTarget != null && !combatTarget.GetHealth().IsDead())
                {
                    return combatTarget;
                }
            }
        }

        return targetBase.GetComponent<CombatTarget>();
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
