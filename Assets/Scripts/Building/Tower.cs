using Dota.Attributes;
using Dota.Utils;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : NetworkBehaviour, ITeamMember, IMinimapEntity, IIconOwner, IVisionEntityOwner
{
    [SerializeField] Team team;
    [SerializeField] VisionEntity visionEntity = null;
    [SerializeField] GameObject minimapIconPrefab = null;
    [SerializeField] Health health = null;
    [SerializeField] Sprite towerIcon = null;

    [Header("Tower")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform projectileSpawnPos;
    [SerializeField] LayerMask championLayer;
    [SerializeField] LayerMask minionLayer;


    [SerializeField] float attackRadius = 10f;
    [SerializeField] float checkInterval = 0.2f;
    [SerializeField] float fireInterval = 1f;
    [SerializeField] float towerDamage = 20f;

    
    Collider[] colliderBuffer = new Collider[10];
    Coroutine bulletFireRoutine;
    Coroutine checkEnemyRoutine;
    [SerializeField] CombatTarget currentTarget;

    public List<Minion> enemyMinions = new List<Minion>();
    public List<Champion> enemyChampions = new List<Champion>();

    List<Projectile> projectilePool = new List<Projectile>();


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

    public void SetTeam(Team team)
    {
        this.team = team;
    }

    public Sprite GetIcon()
    {
        return towerIcon;
    }

    #region Server
    private void Start()
    {
        health.OnHealthDead += Health_OnHealthDead;
        if (isServer)
        {
            checkEnemyRoutine = StartCoroutine(GetTargetRoutine());
            bulletFireRoutine = StartCoroutine(FireBulletRoutine());
        }
    }

    private void Health_OnHealthDead(Health health)
    {
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




    //void Start()
    //{
    //    InvokeRepeating("CreateBullet", 0.1f, 1.3f);
    //}
    //�ͦ��l�u
    public void CreateBullet()
    {
        if(enemyChampions.Count <= 0 && enemyMinions.Count <= 0) { return; }

        // Instantiate Bullet



        // If has minion  Set Bullet Target to minion
        // If not set bullet target to champion
    }

    public VisionEntity GetVisionEntity()
    {
        return visionEntity;
    }


    //�p�L�i�J��b��d��A�b��q���X��������ؼ�
    //private void OnTriggerEnter(Collider other)
    //{
    //    //�i�J�b��O�^��
    //    if (other.gameObject.tag == "Player")
    //    {
    //        listHero.Add(other.gameObject);
    //    }
    //    else
    //    {
    //        //�i�J�b��O�p�L
    //        Minion minion = other.GetComponent<Minion>();
    //        //��p�L�����ŮɡA�P�_�p�L�P�b�������O�_�@�P�A���@�P��ܥi�H����
    //        if (minion && minion.GetTeam() != team)
    //        {
    //            listSolider.Add(other.gameObject);//�p�L��b�i�������C��
    //        }

    //    }

    //}
    ////�H���h�X�d��A�����C��
    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.tag == "Player")
    //    {
    //        listHero.Remove(other.gameObject);
    //    }
    //    else
    //    {

    //        listSolider.Remove(other.gameObject);

    //    }
    //}
}
