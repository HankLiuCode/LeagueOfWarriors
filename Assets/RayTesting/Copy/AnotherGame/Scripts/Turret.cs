using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public float range = 5;  //�w�]�����d��5
    public string enemyTag = "Enemy";
    public Transform target;  //�����ؼ�
    public Transform partRotate; //���௥�x
    public Transform bulletPoint;  //�l�u�ͦ���m
    public GameObject bulletPrefab;  //�l�u�w�s��
    public float bulletRate = 2f; //�o�g�l�u�t�v
    private float countDown = 0;
    void Start()
    {
        InvokeRepeating("UpdateTarget", 0, 0.5f);  //�C��0.5�����I�sUpdateTarget��s�ؼ�
        countDown = 1 / bulletRate;
    }

    void Update()
    {
        if (target == null) return;
        LockTarget();
        //�˭p�ɵo�g�l�u
        countDown -= Time.deltaTime;
        if(countDown<= 0)
        {
            //�o�g�l�u
            Debug.Log("�����l�u");
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
        Gizmos.color = Color.red;  //�]�w�����d���C��
        Gizmos.DrawWireSphere(transform.position, range);  //�e�X�@�Ӳy�������d��
    }
    private void UpdateTarget() 
    {
      GameObject[] enemies =  GameObject.FindGameObjectsWithTag(enemyTag);  //���Ҧ��ĤH
        float minDistance = Mathf.Infinity;
        Transform nearestEnemy = null;
        foreach(var enemy in enemies)
        {
            float distance = Vector3.Distance(enemy.transform.position, transform.position);
            if(distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy.transform;  //���̪񪺼ĤH
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
