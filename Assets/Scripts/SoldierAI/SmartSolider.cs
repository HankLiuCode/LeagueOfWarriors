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

    private float distance;//小兵與箭塔的距離
    //敵人進入小兵攻擊範圍，就加載列表裡面
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
            //目標點為空，需要尋找一下目標點
            target = GetTarget();
            return;
        }
        //ani.CrossFade("Run");
        animator.SetBool("running", true);
        nav.SetDestination(target.position);


        //通過距離判斷小兵是否都攻擊箭塔
        distance = Vector3.Distance(transform.position, target.position);
        if (distance > 2)
        {
            nav.speed = 3.5f;//保持原速度前進
        }
        else
        {
            nav.speed = 0;//小兵停止前進
            Vector3 tarpos = target.position;
            Vector3 lookpos = new Vector3(tarpos.x, transform.position.y, tarpos.z);
            transform.LookAt(lookpos);//小兵看向砲塔，箭塔比較高，所以看向高度是小兵的高度
            //ani.CrossFade("Attack1");
            animator.SetBool("running", false);
            animator.SetBool("attack", true);
        }


    }

/// <summary>
/// 敵人進入小兵攻擊範圍,確定小兵攻擊目標
/// </summary>
/// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        //判斷敵人是英雄還是小兵
        if (other.gameObject.tag == "Player" && this.type == 1)//針對於操作者玩家而言，敵方小兵會攻打玩家，玩家相對於敵方小兵是敵人
        {
            this.enemylist.Add(other.transform);
            Transform temp = enemylist[0];//取出列表的第一個作為攻擊目標
            if (target == null || temp != target)
            {
                target = temp;

            }

        }
        else
        {
            SmartSolider solider = other.GetComponent<SmartSolider>();
            if (solider != null
                && solider.type != this.type //確定是敵方
                && !enemylist.Contains(other.transform))//檢查敵人是否已經包含在敵人列表裡，防止重複加入
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
/// 敵人離開小兵攻擊範圍，從列表中移除
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
    /// 當塔消失時，塔從列表中清除
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


    //假如小兵跑向的目標點是塔
    Transform GetTarget()
    {
        this.enemylist.RemoveAll(t => t == null);//移除掉列表裡所有為空的元素
        if (enemylist.Count > 0) //判斷數組裡是否有值
        {
            //直接拿第一個
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
    
    //設置小兵行走的路
    public void SetRoad(int road)
    {
        nav = GetComponent<NavMeshAgent>();
        nav.areaMask = road;
    }
}
