using Dota.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Monster : NetworkBehaviour, IIconOwner, ITeamMember, IMinimapEntity
{
    CombatTarget target;          //獲取玩家單位

    [SerializeField] Animator animator = null;          //自身動畫
    [SerializeField] NetworkAnimator netAnimator = null;
    [SerializeField] AnimationEventHandler animationEventHandler = null;
    [SerializeField] Health health = null;
    [SerializeField] StatStore statStore = null;
    [SerializeField] MinimapDefaultIcon minimapIconPrefab = null;
    [SerializeField] Sprite icon = null;

    private Vector3 initialPosition;            //初始位置
    private Quaternion initalRotation;

    [SerializeField] float defendRadius = 3f;          //自衛半徑，玩家進入後怪物會追擊玩家，當距離<攻擊距離則會發動攻擊
    [SerializeField] float chaseRadius = 4f;            //追擊半徑，當怪物超出追擊半徑後會放棄追擊，返回追擊起始位置

    [SerializeField] float attackRange = 2f;            //攻擊距離
    [SerializeField] float runSpeed = 5f;          //跑動速度
    [SerializeField] float turnSpeed = 0.1f;         //轉身速度，建議0.1

    private enum MonsterState
    {
        IDLE,        //原地待機
        RELAX,       //原地觀察
        CHASE,       //追擊狀態
        RETURN,      //超出追擊範圍後返回
        ATTACK,      //攻擊狀態
        DEAD,        //死亡狀態
    }
    private MonsterState currentState = MonsterState.IDLE;          //默認狀態為原地待機

    [SerializeField] float[] actionWeight = { 5000, 5000 };         //設置待機時各種動作的權重，順序依次為待機、觀察
    [SerializeField] float actRestTme = 1f;            //更換待機指令的間隔時間
    [SerializeField] float lastActTime;          //最近一次指令時間

    [SerializeField] private float distanceToPlayer = Mathf.Infinity;    //怪物與玩家的距離
    private float distanceToInitial;         //怪物與初始位置的距離
    private Quaternion targetRotation;         //怪物的目標朝向

    private bool is_Running = false;


    public override void OnStartServer()
    {
        //playerUnit = GameObject.FindGameObjectWithTag("Player");
        //thisAnimator = GetComponent<Animator>();
        animationEventHandler.OnAttackPoint += AnimationEventHandler_OnAttackPoint;
        animationEventHandler.OnDeathEnd += AnimationEventHandler_OnDeathEnd;

        //保存初始位置信息
        initialPosition = transform.position;
        initalRotation = transform.rotation;

        //攻擊距離不大於自衛半徑，否則就無法觸發追擊狀態，直接開始戰鬥了
        attackRange = Mathf.Min(defendRadius, attackRange);

        //隨機一個待機動作
        RandomAction();
    }

    /// <summary>
    /// 根據權重隨機待機指令
    /// </summary>
    void RandomAction()
    {
        //更新行動時間
        lastActTime = Time.time;
        //根據權重隨機
        float number = Random.Range(0, actionWeight[0] + actionWeight[1]);
        if (number <= actionWeight[0])
        {
            currentState = MonsterState.IDLE;
            animator.SetBool("relax", false);
            //thisAnimator.SetTrigger("IDLE");
        }
        else if (actionWeight[0] < number && number <= actionWeight[0] + actionWeight[1])
        {
            currentState = MonsterState.RELAX;
            animator.SetBool("relax", true);
            //thisAnimator.SetTrigger("Check");
        }

    }

    [ServerCallback]
    private void OnTriggerStay(Collider other)
    {
        if(target != null) { return; }

        CombatTarget combatTarget = other.GetComponent<CombatTarget>();
        if(health != null)
        {
            target = combatTarget;
        }
    }

    [ServerCallback]
    private void OnTriggerExit(Collider other)
    {
        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            if(target != null && other.gameObject == target.gameObject)
            {
                target = null;
            }
        }
    }

    [ServerCallback]
    void Update()
    {
        if (health.IsDead()) { return; }

        switch (currentState)
        {
            //待機狀態，等待actRestTme後重新隨機指令
            case MonsterState.IDLE:
                transform.rotation = Quaternion.Slerp(transform.rotation, initalRotation, turnSpeed);

                if (Time.time - lastActTime > actRestTme)
                {
                    RandomAction();         //隨機切換指令
                }
                //該狀態下的檢測指令
                EnemyDistanceCheck();
                break;

            //待機狀態，由於觀察動畫時間較長，並希望動畫完整播放，故等待時間是根據一個完整動畫的播放長度，而不是指令間隔時間
            case MonsterState.RELAX:
                transform.rotation = Quaternion.Slerp(transform.rotation, initalRotation, turnSpeed);

                if (Time.time - lastActTime > animator.GetCurrentAnimatorStateInfo(0).length)
                {
                    RandomAction();         //隨機切換指令
                }
                //該狀態下的檢測指令
                EnemyDistanceCheck();
                break;


            //追擊狀態，朝著玩家跑去
            case MonsterState.CHASE:
                Debug.Log("Chase");

                if (!is_Running)
                {
                    //thisAnimator.SetTrigger("Run");
                    animator.SetBool("running", true);
                    is_Running = true;
                }
                transform.Translate(Vector3.forward * Time.deltaTime * runSpeed);
                //朝向玩家位置
                targetRotation = Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed);
                //該狀態下的檢測指令
                ChaseRadiusCheck();
                break;

            //返回狀態，超出追擊範圍後返回出生位置
            case MonsterState.RETURN:
                Debug.Log("Return");

                //朝向初始位置移動
                animator.SetBool("running", true);
                targetRotation = Quaternion.LookRotation(initialPosition - transform.position, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed);
                transform.Translate(Vector3.forward * Time.deltaTime * runSpeed);
                //該狀態下的檢測指令
                ReturnCheck();
                break;

            //攻擊狀態
            case MonsterState.ATTACK:

                Debug.Log("Attack");

                //面向目標
                targetRotation = Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed);

                //確定攻擊動作都播完才能進入別的狀態，防止攻擊播放時目標移動而滑步
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                {
                    // Check enemy damage.
                    //animator.playbackTime
                    return;
                }

                //讓動畫播完
                if (animator.IsInTransition(0))
                {
                    return;
                }
                //該狀態下的檢測指令
                AttackCheck();
                break;
        }
    }

    /// <summary>
    /// 原地待機、觀察狀態的檢測
    /// </summary>
    [Server]
    void EnemyDistanceCheck()
    {
        if (target == null) { return; }

        distanceToPlayer = Vector3.Distance(target.transform.position, transform.position);

        if (distanceToPlayer < attackRange)         //怪物與玩家的距離 小於 攻擊距離
        {
            Debug.Log("EnemyDistanceCheck to Attack");
            currentState = MonsterState.ATTACK;          //進入攻擊狀態
        }
        else if (distanceToPlayer < defendRadius)       //怪物與玩家的距離 小於 自衛半徑
        {
            currentState = MonsterState.CHASE;          //進入追擊狀態
        }
        //if被攻擊(HP減少)
        //進入追擊狀態...
    }


    /// <summary>
    /// 追擊狀態檢測，檢測敵人是否進入攻擊範圍
    /// </summary>
    [Server]
    void ChaseRadiusCheck()
    {
        if (target == null) { return; }

        distanceToPlayer = Vector3.Distance(target.transform.position, transform.position);

        distanceToInitial = Vector3.Distance(transform.position, initialPosition);

        if (distanceToPlayer < attackRange)         //怪物與玩家的距離 小於 攻擊距離
        {
            Debug.Log("ChaseRadiusCheck to Attack");
            currentState = MonsterState.ATTACK;          //進入攻擊狀態
        }
        //如果超出追擊範圍就返回
        if (distanceToInitial > chaseRadius)        //怪物與初始位置的距離 大於 追擊半徑
        {
            currentState = MonsterState.RETURN;         //返回出生位置
        }

        //如果目標死亡(目標HP至0)就返回
        //......
    }

    /// <summary>
    /// 遠離追擊半徑，返回狀態的檢測，不再檢測敵人距離
    /// </summary>
    [Server]
    void ReturnCheck()
    {
        distanceToInitial = Vector3.Distance(transform.position, initialPosition);
        //如果已經接近初始位置，則隨機一個近似狀態
        if (distanceToInitial < 0.5f)
        {
            animator.SetBool("running", false);
            is_Running = false;
            RandomAction();
        }
    }


    // Animation Event
    [Server]
    private void AnimationEventHandler_OnAttackPoint()
    {
        target.GetHealth().ServerTakeDamage(statStore.GetStats().attackDamage);
    }

    [Server]
    private void AnimationEventHandler_OnDeathEnd()
    {
        Destroy(gameObject);
    }


    /// <summary>
    /// 攻擊狀態檢測
    /// </summary>
    [Server]
    void AttackCheck()
    {
        if (target == null) { return; }

        if (target.GetHealth().IsDead())
        {
            currentState = MonsterState.RETURN;
            target = null;
            return;
        }

        distanceToPlayer = Vector3.Distance(target.transform.position, transform.position);
        distanceToInitial = Vector3.Distance(transform.position, initialPosition);
        //攻擊中,播放攻擊動畫
        if (distanceToPlayer < attackRange)         //怪物與玩家的距離 小於 攻擊距離
        {
            animator.SetBool("running", false);
            netAnimator.SetTrigger("attack");
        }
        //如果目標超出攻擊範圍
        if (distanceToPlayer > attackRange)         //怪物與玩家的距離 大於 攻擊距離
        {
            animator.SetBool("running", true);
            currentState = MonsterState.CHASE;          //進入追擊狀態
        }
        //如果超出追擊範圍就返回
        if (distanceToInitial > chaseRadius)        //怪物與初始位置的距離 大於 追擊半徑
        {
            currentState = MonsterState.RETURN;         //返回出生位置
        }
    }

    public Sprite GetIcon()
    {
        return null;
    }

    public Team GetTeam()
    {
        return Team.None;
    }

    public void ServerSetTeam(Team team)
    {
        
    }

    public string GetLayerName()
    {
        return "Monster";
    }

    public MinimapIcon GetMinimapIconInstance()
    {
        MinimapDefaultIcon minimapIconInstance = Instantiate(minimapIconPrefab).GetComponent<MinimapDefaultIcon>();
        minimapIconInstance.SetTeam(Team.None);
        return minimapIconInstance;
    }
}
