using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterAI : MonoBehaviour
{

    public GameObject playerUnit;          //������a���
    [SerializeField] Animator animator = null;          //�ۨ��ʵe
    private Vector3 initialPosition;            //��l��m
    public GameObject RebirthPrefab;   //���᭫���I

    public float defendRadius;          //�۽åb�|�A���a�i�J��Ǫ��|�l�����a�A��Z��<�����Z���h�|�o�ʧ���
    public float chaseRadius;            //�l���b�|�A��Ǫ��W�X�l���b�|��|���l���A��^�l���_�l��m

    public float attackRange;            //�����Z��
    public float runSpeed;          //�]�ʳt��
    public float turnSpeed;         //�ਭ�t�סA��ĳ0.1

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

    public float[] actionWeight = { 5000, 5000 };         //�]�m�ݾ��ɦU�ذʧ@���v���A���Ǩ̦����ݾ��B�[��
    public float actRestTme;            //�󴫫ݾ����O�����j�ɶ�
    private float lastActTime;          //�̪�@�����O�ɶ�

    private float diatanceToPlayer;         //�Ǫ��P���a���Z��
    private float diatanceToInitial;         //�Ǫ��P��l��m���Z��
    private Quaternion targetRotation;         //�Ǫ����ؼд¦V

    private bool is_Running = false;

    void Start()
    {
        //playerUnit = GameObject.FindGameObjectWithTag("Player");
        //thisAnimator = GetComponent<Animator>();

        //�O�s��l��m�H��
        initialPosition = gameObject.GetComponent<Transform>().position;

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
        float number = Random.Range(0, actionWeight[0] + actionWeight[1] );
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

    void Update()
    {
        switch (currentState)
        {
            //�ݾ����A�A����actRestTme�᭫�s�H�����O
            case MonsterState.IDLE:
                if (Time.time - lastActTime > actRestTme)
                {
                    RandomAction();         //�H���������O
                }
                //�Ӫ��A�U���˴����O
                EnemyDistanceCheck();
                break;

            //�ݾ����A�A�ѩ��[��ʵe�ɶ������A�çƱ�ʵe���㼽��A�G���ݮɶ��O�ھڤ@�ӧ���ʵe��������סA�Ӥ��O���O���j�ɶ�
            case MonsterState.RELAX:
                if (Time.time - lastActTime > animator.GetCurrentAnimatorStateInfo(0).length)
                {
                    RandomAction();         //�H���������O
                }
                //�Ӫ��A�U���˴����O
                EnemyDistanceCheck();
                break;



            //�l�����A�A�µ۪��a�]�h
            case MonsterState.CHASE:
                if (!is_Running)
                {
                    //thisAnimator.SetTrigger("Run");
                    animator.SetBool("running", true);
                    is_Running = true;
                }
                transform.Translate(Vector3.forward * Time.deltaTime * runSpeed);
                //�¦V���a��m
                targetRotation = Quaternion.LookRotation(playerUnit.transform.position - transform.position, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed);
                //�Ӫ��A�U���˴����O
                ChaseRadiusCheck();
                break;

            //��^���A�A�W�X�l���d����^�X�ͦ�m
            case MonsterState.RETURN:
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
                //���V�ؼ�
                Vector3 look = playerUnit.transform.position - this.transform.position;
                look.y = 0.0f;
                this.transform.forward = look;
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

            //���`���A
            case MonsterState.DEAD:
                animator.SetTrigger("die");
                //�Ӫ��A�U���˴����O
                DEADCheck();
                break;
        }
    }

    /// <summary>
    /// ��a�ݾ��B�[��A���˴�
    /// </summary>
    void EnemyDistanceCheck()
    {
        diatanceToPlayer = Vector3.Distance(playerUnit.transform.position, transform.position);
        if (diatanceToPlayer < attackRange)         //�Ǫ��P���a���Z�� �p�� �����Z��
        {
            currentState = MonsterState.ATTACK;          //�i�J�������A
        }
        else if (diatanceToPlayer < defendRadius)       //�Ǫ��P���a���Z�� �p�� �۽åb�|
        {
            currentState = MonsterState.CHASE;          //�i�J�l�����A
        }

        //if�Q����(HP���)
        //�i�J�l�����A...
    }


    /// <summary>
    /// �l�����A�˴��A�˴��ĤH�O�_�i�J�����d��
    /// </summary>
    void ChaseRadiusCheck()
    {
        diatanceToPlayer = Vector3.Distance(playerUnit.transform.position, transform.position);
        diatanceToInitial = Vector3.Distance(transform.position, initialPosition);

        if (diatanceToPlayer < attackRange)         //�Ǫ��P���a���Z�� �p�� �����Z��
        {
            currentState = MonsterState.ATTACK;          //�i�J�������A
        }
        //�p�G�W�X�l���d��N��^
        if (diatanceToInitial > chaseRadius)        //�Ǫ��P��l��m���Z�� �j�� �l���b�|
        {
            currentState = MonsterState.RETURN;         //��^�X�ͦ�m
        }

        //�p�G�ؼЦ��`(�ؼ�HP��0)�N��^
        //......
    }

    /// <summary>
    /// �����l���b�|�A��^���A���˴��A���A�˴��ĤH�Z��
    /// </summary>
    void ReturnCheck()
    {
        diatanceToInitial = Vector3.Distance(transform.position, initialPosition);
        //�p�G�w�g�����l��m�A�h�H���@�Ӫ�����A
        if (diatanceToInitial < 0.5f)
        {
            animator.SetBool("running", false);
            is_Running = false;
            RandomAction();
        }
    }

    /// <summary>
    /// �������A�˴�
    /// </summary>
    void AttackCheck() 
    {
        diatanceToPlayer = Vector3.Distance(playerUnit.transform.position, transform.position);
        diatanceToInitial = Vector3.Distance(transform.position, initialPosition);
        //������,��������ʵe
        if (diatanceToPlayer < attackRange)         //�Ǫ��P���a���Z�� �p�� �����Z��
        {
            animator.SetBool("running", false);
            animator.SetTrigger("attack");
        }
        //�p�G�ؼжW�X�����d��
        if (diatanceToPlayer > attackRange)         //�Ǫ��P���a���Z�� �j�� �����Z��
        {
            animator.SetBool("running", true);
            currentState = MonsterState.CHASE;          //�i�J�l�����A
        }
        //�p�G�W�X�l���d��N��^
        if (diatanceToInitial > chaseRadius)        //�Ǫ��P��l��m���Z�� �j�� �l���b�|
        {
            currentState = MonsterState.RETURN;         //��^�X�ͦ�m
        }
        
        //�p�G�ؼЦ��`(�ؼ�HP��0)�N��^
        //......
    }


    /// <summary>
    /// ���`���A�˴�
    /// </summary>
    void DEADCheck() 
    {
        //Ĳ�o�Ǫ�����
        RebirthPrefab.GetComponent<CreatMonster>().CreateMonsters();

    }

}

