using Dota.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Monster : NetworkBehaviour, IIconOwner, ITeamMember, IMinimapEntity
{
    CombatTarget target;          //������a���

    [SerializeField] Animator animator = null;          //�ۨ��ʵe
    [SerializeField] NetworkAnimator netAnimator = null;
    [SerializeField] AnimationEventHandler animationEventHandler = null;
    [SerializeField] Health health = null;
    [SerializeField] StatStore statStore = null;
    [SerializeField] MinimapDefaultIcon minimapIconPrefab = null;
    [SerializeField] Sprite icon = null;

    private Vector3 initialPosition;            //��l��m
    private Quaternion initalRotation;

    [SerializeField] float defendRadius = 3f;          //�۽åb�|�A���a�i�J��Ǫ��|�l�����a�A��Z��<�����Z���h�|�o�ʧ���
    [SerializeField] float chaseRadius = 4f;            //�l���b�|�A��Ǫ��W�X�l���b�|��|���l���A��^�l���_�l��m

    [SerializeField] float attackRange = 2f;            //�����Z��
    [SerializeField] float runSpeed = 5f;          //�]�ʳt��
    [SerializeField] float turnSpeed = 0.1f;         //�ਭ�t�סA��ĳ0.1

    private enum MonsterState
    {
        IDLE,        //��a�ݾ�
        RELAX,       //��a�[��
        CHASE,       //�l�����A
        RETURN,      //�W�X�l���d����^
        ATTACK,      //�������A
        DEAD,        //���`���A
    }
    private MonsterState currentState = MonsterState.IDLE;          //�q�{���A����a�ݾ�

    [SerializeField] float[] actionWeight = { 5000, 5000 };         //�]�m�ݾ��ɦU�ذʧ@���v���A���Ǩ̦����ݾ��B�[��
    [SerializeField] float actRestTme = 1f;            //�󴫫ݾ����O�����j�ɶ�
    [SerializeField] float lastActTime;          //�̪�@�����O�ɶ�

    [SerializeField] private float distanceToPlayer = Mathf.Infinity;    //�Ǫ��P���a���Z��
    private float distanceToInitial;         //�Ǫ��P��l��m���Z��
    private Quaternion targetRotation;         //�Ǫ����ؼд¦V

    private bool is_Running = false;


    public override void OnStartServer()
    {
        //playerUnit = GameObject.FindGameObjectWithTag("Player");
        //thisAnimator = GetComponent<Animator>();
        animationEventHandler.OnAttackPoint += AnimationEventHandler_OnAttackPoint;
        animationEventHandler.OnDeathEnd += AnimationEventHandler_OnDeathEnd;

        //�O�s��l��m�H��
        initialPosition = transform.position;
        initalRotation = transform.rotation;

        //�����Z�����j��۽åb�|�A�_�h�N�L�kĲ�o�l�����A�A�����}�l�԰��F
        attackRange = Mathf.Min(defendRadius, attackRange);

        //�H���@�ӫݾ��ʧ@
        RandomAction();
    }

    /// <summary>
    /// �ھ��v���H���ݾ����O
    /// </summary>
    void RandomAction()
    {
        //��s��ʮɶ�
        lastActTime = Time.time;
        //�ھ��v���H��
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
            //�ݾ����A�A����actRestTme�᭫�s�H�����O
            case MonsterState.IDLE:
                transform.rotation = Quaternion.Slerp(transform.rotation, initalRotation, turnSpeed);

                if (Time.time - lastActTime > actRestTme)
                {
                    RandomAction();         //�H���������O
                }
                //�Ӫ��A�U���˴����O
                EnemyDistanceCheck();
                break;

            //�ݾ����A�A�ѩ��[��ʵe�ɶ������A�çƱ�ʵe���㼽��A�G���ݮɶ��O�ھڤ@�ӧ���ʵe��������סA�Ӥ��O���O���j�ɶ�
            case MonsterState.RELAX:
                transform.rotation = Quaternion.Slerp(transform.rotation, initalRotation, turnSpeed);

                if (Time.time - lastActTime > animator.GetCurrentAnimatorStateInfo(0).length)
                {
                    RandomAction();         //�H���������O
                }
                //�Ӫ��A�U���˴����O
                EnemyDistanceCheck();
                break;


            //�l�����A�A�µ۪��a�]�h
            case MonsterState.CHASE:
                Debug.Log("Chase");

                if (!is_Running)
                {
                    //thisAnimator.SetTrigger("Run");
                    animator.SetBool("running", true);
                    is_Running = true;
                }
                transform.Translate(Vector3.forward * Time.deltaTime * runSpeed);
                //�¦V���a��m
                targetRotation = Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed);
                //�Ӫ��A�U���˴����O
                ChaseRadiusCheck();
                break;

            //��^���A�A�W�X�l���d����^�X�ͦ�m
            case MonsterState.RETURN:
                Debug.Log("Return");

                //�¦V��l��m����
                animator.SetBool("running", true);
                targetRotation = Quaternion.LookRotation(initialPosition - transform.position, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed);
                transform.Translate(Vector3.forward * Time.deltaTime * runSpeed);
                //�Ӫ��A�U���˴����O
                ReturnCheck();
                break;

            //�������A
            case MonsterState.ATTACK:

                Debug.Log("Attack");

                //���V�ؼ�
                targetRotation = Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed);

                //�T�w�����ʧ@�������~��i�J�O�����A�A�����������ɥؼв��ʦӷƨB
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                {
                    // Check enemy damage.
                    //animator.playbackTime
                    return;
                }

                //���ʵe����
                if (animator.IsInTransition(0))
                {
                    return;
                }
                //�Ӫ��A�U���˴����O
                AttackCheck();
                break;
        }
    }

    /// <summary>
    /// ��a�ݾ��B�[��A���˴�
    /// </summary>
    [Server]
    void EnemyDistanceCheck()
    {
        if (target == null) { return; }

        distanceToPlayer = Vector3.Distance(target.transform.position, transform.position);

        if (distanceToPlayer < attackRange)         //�Ǫ��P���a���Z�� �p�� �����Z��
        {
            Debug.Log("EnemyDistanceCheck to Attack");
            currentState = MonsterState.ATTACK;          //�i�J�������A
        }
        else if (distanceToPlayer < defendRadius)       //�Ǫ��P���a���Z�� �p�� �۽åb�|
        {
            currentState = MonsterState.CHASE;          //�i�J�l�����A
        }
        //if�Q����(HP���)
        //�i�J�l�����A...
    }


    /// <summary>
    /// �l�����A�˴��A�˴��ĤH�O�_�i�J�����d��
    /// </summary>
    [Server]
    void ChaseRadiusCheck()
    {
        if (target == null) { return; }

        distanceToPlayer = Vector3.Distance(target.transform.position, transform.position);

        distanceToInitial = Vector3.Distance(transform.position, initialPosition);

        if (distanceToPlayer < attackRange)         //�Ǫ��P���a���Z�� �p�� �����Z��
        {
            Debug.Log("ChaseRadiusCheck to Attack");
            currentState = MonsterState.ATTACK;          //�i�J�������A
        }
        //�p�G�W�X�l���d��N��^
        if (distanceToInitial > chaseRadius)        //�Ǫ��P��l��m���Z�� �j�� �l���b�|
        {
            currentState = MonsterState.RETURN;         //��^�X�ͦ�m
        }

        //�p�G�ؼЦ��`(�ؼ�HP��0)�N��^
        //......
    }

    /// <summary>
    /// �����l���b�|�A��^���A���˴��A���A�˴��ĤH�Z��
    /// </summary>
    [Server]
    void ReturnCheck()
    {
        distanceToInitial = Vector3.Distance(transform.position, initialPosition);
        //�p�G�w�g�����l��m�A�h�H���@�Ӫ�����A
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
    /// �������A�˴�
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
        //������,��������ʵe
        if (distanceToPlayer < attackRange)         //�Ǫ��P���a���Z�� �p�� �����Z��
        {
            animator.SetBool("running", false);
            netAnimator.SetTrigger("attack");
        }
        //�p�G�ؼжW�X�����d��
        if (distanceToPlayer > attackRange)         //�Ǫ��P���a���Z�� �j�� �����Z��
        {
            animator.SetBool("running", true);
            currentState = MonsterState.CHASE;          //�i�J�l�����A
        }
        //�p�G�W�X�l���d��N��^
        if (distanceToInitial > chaseRadius)        //�Ǫ��P��l��m���Z�� �j�� �l���b�|
        {
            currentState = MonsterState.RETURN;         //��^�X�ͦ�m
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
