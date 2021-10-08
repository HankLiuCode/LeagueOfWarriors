using Dota.Attributes;
using Dota.Utils;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : NetworkBehaviour, ITeamMember, IMinimapEntity, IIconOwner
{
    [SerializeField] Team team;
    [SerializeField] GameObject minimapIconPrefab = null;
    [SerializeField] Health health = null;
    [SerializeField] Sprite towerIcon = null;

    [Header("Tower")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] GameObject towerDeadEffect;

    [SerializeField] Transform projectileSpawnPos;
    [SerializeField] LayerMask championLayer;
    [SerializeField] LayerMask minionLayer;


    [SerializeField] float attackRadius = 10f;
    [SerializeField] float checkInterval = 0.2f;
    [SerializeField] float fireInterval = 1f;
    [SerializeField] float towerDamage = 20f;
    [SerializeField] Disolver disolver = null;


    Collider[] colliderBuffer = new Collider[10];
    Coroutine bulletFireRoutine;
    Coroutine checkEnemyRoutine;
    [SerializeField] CombatTarget currentTarget;

    public List<Minion> enemyMinions = new List<Minion>();
    public List<Champion> enemyChampions = new List<Champion>();
    public float dealthAnimDuration = 0.5f;

    public static event System.Action<Tower> ServerOnTowerDied;
    public static event System.Action<Tower> ClientOnTowerDead;

    public static event System.Action<Tower> OnTowerSpawned;
    public static event System.Action<Tower> OnTowerDestroyed;

    public string GetLayerName()
    {
        return "Building";
    }

    public MinimapIcon GetMinimapIconInstance()
    {
        MinimapDefaultIcon minimapIconInstance = Instantiate(minimapIconPrefab).GetComponent<MinimapDefaultIcon>();
        minimapIconInstance.SetVisible(false);
        minimapIconInstance.SetTeam(team);

        return minimapIconInstance;
    }

    public Team GetTeam()
    {
        return team;
    }

    public void ServerSetTeam(Team team)
    {
        this.team = team;
    }

    public Sprite GetIcon()
    {
        return towerIcon;
    }

    #region Client

    public override void OnStartClient()
    {
        OnTowerSpawned?.Invoke(this);
        health.ClientOnHealthDead += Health_ClientOnHealthDead;
    }

    public override void OnStopClient()
    {
        OnTowerDestroyed?.Invoke(this);
    }

    private void Health_ClientOnHealthDead(Health health)
    {
        disolver.StartDisolve();
        ClientOnTowerDead?.Invoke(this);
    }

    #endregion

    #region Server

    public override void OnStartServer()
    {
        health.ServerOnHealthDead += Health_ServerOnHealthDead;
        checkEnemyRoutine = StartCoroutine(GetTargetRoutine());
        bulletFireRoutine = StartCoroutine(FireBulletRoutine());
    }

    private void Health_ServerOnHealthDead(Health obj)
    {
        ServerOnTowerDied?.Invoke(this);

        GameObject deathEffectInstance = Instantiate(towerDeadEffect, transform.position + Vector3.up, Quaternion.identity);

        NetworkServer.Spawn(deathEffectInstance);

        StartCoroutine(DestroyAfter(disolver.GetDisolveDuration() + dealthAnimDuration));
    }

    IEnumerator DestroyAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        NetworkServer.Destroy(gameObject);
    }

    IEnumerator FireBulletRoutine()
    {
        while (true)
        {
            if (currentTarget != null)
            {
                if((Vector3.Distance(currentTarget.transform.position, transform.position) > attackRadius) || currentTarget.GetHealth().IsDead())
                {
                    currentTarget = null;
                }
                else
                {
                    SpawnProjectile(currentTarget);
                }
            }
            yield return new WaitForSeconds(fireInterval);
        }
    }


    IEnumerator GetTargetRoutine()
    {
        while (true)
        {
            if (currentTarget == null)
            {
                currentTarget = GetTarget();
            }
            yield return new WaitForSeconds(checkInterval);
        }
    }

    private void SpawnProjectile(CombatTarget target)
    {
        GameObject bulletInstance = Instantiate(projectilePrefab, projectileSpawnPos.position, Quaternion.identity);
        Projectile projectile = bulletInstance.GetComponent<Projectile>();
        
        NetworkServer.Spawn(bulletInstance);
        projectile.SetTarget(target, towerDamage, projectileSpawnPos.position);
        projectile.SetOwner(netIdentity);
    }

    private CombatTarget GetTarget()
    {
        int championCount = Physics.OverlapSphereNonAlloc(transform.position, attackRadius, colliderBuffer, championLayer);

        int minionCount = Physics.OverlapSphereNonAlloc(transform.position, attackRadius, colliderBuffer, minionLayer);
        
        if(championCount <= 0 && minionCount <= 0) { return null; }

        for (int i = 0; i < championCount; i++)
        {
            CombatTarget combatTarget = colliderBuffer[i].GetComponent<CombatTarget>();
            if (combatTarget != null && !TeamChecker.IsSameTeam(gameObject, combatTarget.gameObject))
            {
                return combatTarget;
            }
        }

        for (int i = 0; i < minionCount; i++)
        {
            CombatTarget combatTarget = colliderBuffer[i].GetComponent<CombatTarget>();
            if (combatTarget != null && !TeamChecker.IsSameTeam(gameObject, combatTarget.gameObject))
            {
                return combatTarget;
            }
        }
        
        return null;
    }

    #endregion
}
