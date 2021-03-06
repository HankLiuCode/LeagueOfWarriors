using Dota.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class Monster : NetworkBehaviour, IIconOwner, ITeamMember, IMinimapEntity
{
    CombatTarget target;

    [SerializeField] Animator animator = null;
    [SerializeField] NetworkAnimator netAnimator = null;
    [SerializeField] AnimationEventHandler animationEventHandler = null;
    [SerializeField] Health health = null;
    [SerializeField] StatStore statStore = null;
    [SerializeField] NavMeshAgent agent = null;
    [SerializeField] MinimapDefaultIcon minimapIconPrefab = null;
    [SerializeField] Sprite icon = null;

    private Vector3 initialPosition;            
    private Quaternion initalRotation;

    [SerializeField] float defendRadius = 3f;          
    [SerializeField] float chaseRadius = 4f;            

    [SerializeField] float attackRange = 2f;            
    [SerializeField] float runSpeed = 5f;          
    [SerializeField] float turnSpeed = 0.1f;
    [SerializeField] float destroyTime = 3f;

    private enum MonsterState
    {
        IDLE,
        RELAX,
        CHASE,      
        RETURN,      
        ATTACK,      
        DEAD,       
    }
    private MonsterState currentState = MonsterState.IDLE;          

    [SerializeField] float[] actionWeight = { 5000, 5000 };         
    [SerializeField] float actRestTme = 1f;         
    [SerializeField] float lastActTime;        

    [SerializeField] private float distanceToPlayer = Mathf.Infinity; 
    private float distanceToInitial;        
    private Quaternion targetRotation;       
    private bool is_Running = false;

    public static event System.Action<Monster> ServerOnMonsterDead;
    public static event System.Action<Monster> ClientOnMonsterDead;
    public static event System.Action<Monster> OnMonsterSpawned;
    public static event System.Action<Monster> OnMonsterDestroyed;


    #region Server
    public override void OnStartServer()
    {
        health.ServerOnHealthDead += Health_ServerOnHealthDead;
        animationEventHandler.OnAttackPoint += AnimationEventHandler_OnAttackPoint;

        initialPosition = transform.position;
        initalRotation = transform.rotation;

        //?????Z?????j???????b?|?A?_?h?N?L?k???o?l?????A?A?????}?l?????F
        attackRange = Mathf.Min(defendRadius, attackRange);

        //?H???@?????????@
        RandomAction();
    }

    private void Health_ServerOnHealthDead(Health obj)
    {
        agent.enabled = false;
        StartCoroutine(DestroyAfter(destroyTime));
        ServerOnMonsterDead?.Invoke(this);
    }

    IEnumerator DestroyAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        NetworkServer.Destroy(gameObject);
    }

    /// <summary>
    /// ?????v???H?????????O
    /// </summary>
    void RandomAction()
    {
        //???s????????
        lastActTime = Time.time;
        //?????v???H??
        float number = Random.Range(0, actionWeight[0] + actionWeight[1]);
        if (number <= actionWeight[0])
        {
            currentState = MonsterState.IDLE;
            animator.SetBool("relax", false);
        }
        else if (actionWeight[0] < number && number <= actionWeight[0] + actionWeight[1])
        {
            currentState = MonsterState.RELAX;
            animator.SetBool("relax", true);
        }
    }

    [ServerCallback]
    private void OnTriggerStay(Collider other)
    {
        if (target != null) { return; }

        CombatTarget combatTarget = other.GetComponent<CombatTarget>();
        if (health != null)
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
            if (target != null && other.gameObject == target.gameObject)
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
            //???????A?A????actRestTme?????s?H?????O
            case MonsterState.IDLE:
                transform.rotation = Quaternion.Slerp(transform.rotation, initalRotation, turnSpeed);

                if (Time.time - lastActTime > actRestTme)
                {
                    RandomAction();         //?H?????????O
                }
                //?????A?U?????????O
                EnemyDistanceCheck();
                break;

            //???????A?A?????[?????e?????????A?????????e?????????A?G?????????O?????@?????????e???????????A?????O???O???j????
            case MonsterState.RELAX:
                transform.rotation = Quaternion.Slerp(transform.rotation, initalRotation, turnSpeed);

                if (Time.time - lastActTime > animator.GetCurrentAnimatorStateInfo(0).length)
                {
                    RandomAction();         //?H?????????O
                }
                //?????A?U?????????O
                EnemyDistanceCheck();
                break;


            //?l?????A?A???????a?]?h
            case MonsterState.CHASE:
                Debug.Log("Chase");

                if (!is_Running)
                {
                    //thisAnimator.SetTrigger("Run");
                    animator.SetBool("running", true);
                    is_Running = true;
                }
                if(target != null)
                {
                    Vector3 targetVec = target.transform.position - transform.position;
                    agent.Move(targetVec.normalized * Time.deltaTime * runSpeed);
                }

                ChaseRadiusCheck();
                break;

            //???^???A?A?W?X?l???d???????^?X?????m
            case MonsterState.RETURN:
                Debug.Log("Return");

                //???V???l???m????
                animator.SetBool("running", true);
                agent.SetDestination(initialPosition);
                //targetRotation = Quaternion.LookRotation(initialPosition - transform.position, Vector3.up);
                //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed);
                //transform.Translate(Vector3.forward * Time.deltaTime * runSpeed);
                //?????A?U?????????O
                ReturnCheck();
                break;

            //???????A
            case MonsterState.ATTACK:

                Debug.Log("Attack");

                //???V????
                targetRotation = Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed);

                //?T?w???????@???????~???i?J?O?????A?A???????????????????????????B
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                {
                    // Check enemy damage.
                    //animator.playbackTime
                    return;
                }

                //?????e????
                if (animator.IsInTransition(0))
                {
                    return;
                }
                //?????A?U?????????O
                AttackCheck();
                break;
        }
    }

    /// <summary>
    /// ???a?????B?[?????A??????
    /// </summary>
    [Server]
    void EnemyDistanceCheck()
    {
        if (target == null) { return; }

        distanceToPlayer = Vector3.Distance(target.transform.position, transform.position);

        if (distanceToPlayer < attackRange)         //?????P???a???Z?? ?p?? ?????Z??
        {
            Debug.Log("EnemyDistanceCheck to Attack");
            currentState = MonsterState.ATTACK;          //?i?J???????A
        }
        else if (distanceToPlayer < defendRadius)       //?????P???a???Z?? ?p?? ?????b?|
        {
            currentState = MonsterState.CHASE;          //?i?J?l?????A
        }

        //if?Q????(HP????)
        //?i?J?l?????A...
    }


    /// <summary>
    /// ?l?????A?????A???????H?O?_?i?J?????d??
    /// </summary>
    [Server]
    void ChaseRadiusCheck()
    {
        if (target == null) { return; }

        distanceToPlayer = Vector3.Distance(target.transform.position, transform.position);

        distanceToInitial = Vector3.Distance(transform.position, initialPosition);

        if (distanceToPlayer < attackRange)         //?????P???a???Z?? ?p?? ?????Z??
        {
            Debug.Log("ChaseRadiusCheck to Attack");
            currentState = MonsterState.ATTACK;          //?i?J???????A
        }
        //?p?G?W?X?l???d???N???^
        if (distanceToInitial > chaseRadius)        //?????P???l???m???Z?? ?j?? ?l???b?|
        {
            currentState = MonsterState.RETURN;         //???^?X?????m
        }

        //?p?G???????`(????HP??0)?N???^
        //......
    }

    /// <summary>
    /// ?????l???b?|?A???^???A???????A???A???????H?Z??
    /// </summary>
    [Server]
    void ReturnCheck()
    {
        distanceToInitial = Vector3.Distance(transform.position, initialPosition);
        //?p?G?w?g???????l???m?A?h?H???@?????????A
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
        target.GetHealth().ServerTakeDamage(statStore.GetStats().attackDamage, netIdentity);
    }

    /// <summary>
    /// ???????A????
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
        //??????,???????????e
        if (distanceToPlayer < attackRange)         //?????P???a???Z?? ?p?? ?????Z??
        {
            animator.SetBool("running", false);
            netAnimator.SetTrigger("attack");
        }
        //?p?G?????W?X?????d??
        if (distanceToPlayer > attackRange)         //?????P???a???Z?? ?j?? ?????Z??
        {
            animator.SetBool("running", true);
            currentState = MonsterState.CHASE;          //?i?J?l?????A
        }
        //?p?G?W?X?l???d???N???^
        if (distanceToInitial > chaseRadius)        //?????P???l???m???Z?? ?j?? ?l???b?|
        {
            currentState = MonsterState.RETURN;         //???^?X?????m
        }
    }

    #endregion


    #region Client

    public override void OnStartClient()
    {
        OnMonsterSpawned?.Invoke(this);
        health.ClientOnHealthDead += Health_ClientOnHealthDead;
    }

    private void Health_ClientOnHealthDead(Health obj)
    {
        agent.enabled = false;
        ClientOnMonsterDead?.Invoke(this);
    }

    public override void OnStopClient()
    {
        OnMonsterDestroyed?.Invoke(this);
    }

    #endregion

    public Sprite GetIcon()
    {
        return icon;
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
