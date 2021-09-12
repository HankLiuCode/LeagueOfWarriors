using UnityEngine;
using System.Collections;


public class Bullet : MonoBehaviour
{
    private GameObject target;
    public float speed = 20f;
    public Tower tower;
    void Start()
    {
        tower = GetComponentInParent<Tower>();
        //生成之後1秒內就要銷毀，因為可能是子彈已經存在攻擊目標，但是目標跑出了射程之外，所以我們規定子彈存在時間為1秒
        Destroy(this.gameObject, 1f);
    }
    /*
    //子彈撞到士兵或者英雄
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "solider")
        {

            //獲取人物身上的血量值，調用減血方法
            Health hp = other.GetComponent<Health>();
            if (hp)
            {
                hp.TakeDamage(0.5f);
                if (hp.hp.Value <= 0)
                {  //子彈碰到小兵，且小兵血量此時為0，從列表中移除小兵
                    tower.listSolider.Remove(other.gameObject);
                    Destroy(other.gameObject);
                }
                Destroy(this.gameObject);
            }

        }
        else if (other.gameObject.tag == "Player")
        {
            //獲取人物身上的血量值，調用減血方法
            Health hp = other.GetComponent<Health>();
            if (hp)
            {
                hp.TakeDamage(0.5f);
                if (hp.hp.Value <= 0)
                {  //子彈碰到小兵，且小兵血量此時為0，從列表中移除
                    tower.listHero.Remove(other.gameObject);
                    Destroy(other.gameObject);
                }
                Destroy(this.gameObject);
            }

        }


    }
    */
    void Update()
    {
        //攻擊目標不為空，子彈移動
        if (target)
        {
            Vector3 dir = target.transform.position - transform.position;
            GetComponent<Rigidbody>().velocity = dir.normalized * speed;
        }
        else
        {
            //子彈在向小兵移動的過程中，小兵可能已經消失了，此時要消失子彈
            Destroy(this.gameObject);
        }

    }
    //得到子彈攻擊的目標
    public void SetTarget(GameObject target)
    {
        this.target = target;
    }


}
