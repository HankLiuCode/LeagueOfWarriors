using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class SmartSolider : MonoBehaviour
{

    public Transform target;
    //private Animation ani;
    [SerializeField] Animator animator = null;
    private NavMeshAgent nav;
    public Transform[] towers;
    public int type = 0;

    private float distance;//�p�L�P�b�𪺶Z��
    //�ĤH�i�J�p�L�����d��A�N�[���C��̭�
    List<Transform> enemylist = new List<Transform>();
    // Use this for initialization
    void Start()
    {

        nav = GetComponent<NavMeshAgent>();
        //ani = GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        SoliderMove();

    }

    void SoliderMove()
    {

        if (target == null)
        {
            //�ؼ��I���šA�ݭn�M��@�U�ؼ��I
            target = GetTarget();
            return;
        }
        //ani.CrossFade("Run");
        animator.SetBool("running", true);
        nav.SetDestination(target.position);


        //�q�L�Z���P�_�p�L�O�_�������b��
        distance = Vector3.Distance(transform.position, target.position);
        if (distance > 2)
        {
            nav.speed = 3.5f;//�O����t�׫e�i
        }
        else
        {
            nav.speed = 0;//�p�L����e�i
            Vector3 tarpos = target.position;
            Vector3 lookpos = new Vector3(tarpos.x, transform.position.y, tarpos.z);
            transform.LookAt(lookpos);//�p�L�ݦV����A�b�������A�ҥH�ݦV���׬O�p�L������
            //ani.CrossFade("Attack1");
            animator.SetBool("running", false);
            animator.SetBool("attack", true);
        }


    }

/// <summary>
/// �ĤH�i�J�p�L�����d��,�T�w�p�L�����ؼ�
/// </summary>
/// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        //�P�_�ĤH�O�^���٬O�p�L
        if (other.gameObject.tag == "Player" && this.type == 1)//�w���ާ@�̪��a�Ө��A�Ĥ�p�L�|�𥴪��a�A���a�۹��Ĥ�p�L�O�ĤH
        {
            this.enemylist.Add(other.transform);
            Transform temp = enemylist[0];//���X�C���Ĥ@�ӧ@�������ؼ�
            if (target == null || temp != target)
            {
                target = temp;

            }

        }
        else
        {
            SmartSolider solider = other.GetComponent<SmartSolider>();
            if (solider != null
                && solider.type != this.type //�T�w�O�Ĥ�
                && !enemylist.Contains(other.transform))//�ˬd�ĤH�O�_�w�g�]�t�b�ĤH�C��̡A����ƥ[�J
            {
                enemylist.Add(other.transform);
                Transform temp = enemylist[0];
                if (target == null || temp != target)
                {
                    target = temp;
                }
            }

        }

    }

/// <summary>
/// �ĤH���}�p�L�����d��A�q�C������
/// </summary>
/// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (enemylist.Contains(other.transform))
        {
            target = GetTarget();
        }


    }


    /// <summary>
    /// �������ɡA��q�C���M��
    /// </summary>
    /// <param name="destroyTower"></param>
    void DestroyTowerInList(Transform detroyTower)
    {
        for (int i = 0; i < towers.Length; i++)
        {
            if (towers[i] == detroyTower)
            {
                towers[i] = null;
                return;
            }
        }
    }


    //���p�p�L�]�V���ؼ��I�O��
    Transform GetTarget()
    {
        this.enemylist.RemoveAll(t => t == null);//�������C��̩Ҧ����Ū�����
        if (enemylist.Count > 0) //�P�_�Ʋո̬O�_����
        {
            //�������Ĥ@��
            return enemylist[0];
        }
        for (int i = 0; i < towers.Length; i++)
        {

            if (towers[i] != null)
            {
                return towers[i];
            }

        }
        return null;
    }
    
    //�]�m�p�L�樫����
    public void SetRoad(int road)
    {
        nav = GetComponent<NavMeshAgent>();
        nav.areaMask = road;
    }
}
