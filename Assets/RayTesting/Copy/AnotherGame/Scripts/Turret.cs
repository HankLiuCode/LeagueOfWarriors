using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public float range = 5;  //預設攻擊範圍5
    public string enemyTag = "Enemy";
    public Transform target;  //攻擊目標
    public Transform partRotate; //旋轉砲台
    public Transform bulletPoint;  //子彈生成位置
    public GameObject bulletPrefab;  //子彈預製體
    public float bulletRate = 2f; //發射子彈速率
    private float countDown = 0;
    void Start()
    {
        InvokeRepeating("UpdateTarget", 0, 0.5f);  //每格0.5秒鐘呼叫UpdateTarget更新目標
        countDown = 1 / bulletRate;
    }

    void Update()
    {
        if (target == null) return;
        LockTarget();
        //倒計時發射子彈
        countDown -= Time.deltaTime;
        if(countDown<= 0)
        {
            //發射子彈
            Debug.Log("炸裂子彈");
            GameObject bulletgo =  Instantiate(bulletPrefab, bulletPoint.position, bulletPoint.rotation);
            BallBullet bullet = bulletgo.GetComponent<BallBullet>();
            if(bullet == null)
            {
                bullet = bulletgo.AddComponent<BallBullet>();
            }
            bullet.SetTarget(target);
            countDown = 1 / bulletRate;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;  //設定攻擊範圍顏色
        Gizmos.DrawWireSphere(transform.position, range);  //畫出一個球型攻擊範圍
    }
    private void UpdateTarget() 
    {
      GameObject[] enemies =  GameObject.FindGameObjectsWithTag(enemyTag);  //找到所有敵人
        float minDistance = Mathf.Infinity;
        Transform nearestEnemy = null;
        foreach(var enemy in enemies)
        {
            float distance = Vector3.Distance(enemy.transform.position, transform.position);
            if(distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy.transform;  //找到最近的敵人
            }
        }
        if(minDistance < range)
        {
            target = nearestEnemy;
        }
        else
        {
            target = null;
        }
    }
    private void LockTarget()
    {
        Vector3 dir = target.position - transform.position;
        partRotate.rotation =  Quaternion.LookRotation(dir);
    }
}
