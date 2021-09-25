using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 10;  //�קK���ʤӧ�,���@�өT�w�����ʳt��
    Transform target;  //�ݧ��Ҧ����|�ΤU�ӥؼ��I
    private int pointIndex = 0;
    void Start()  
    {
        target = PathPoints.pathPoints[pointIndex];  //��target���
    }

    
    void Update()  //��s�ĤH��m(�o�̬O�@�ӦV�q:��V�M����)
    {
        Vector3 dir = target.position - transform.position;  //�ؼ��I���e��m=�n���ʪ���V�M����
        transform.Translate(dir.normalized * moveSpeed * Time.deltaTime, Space.World);  //�u�n��V���n���שҥH��normalized�@����V,����^��
        if (Vector3.Distance(target.position, transform.position) < 0.2f)   //�P�_��e��m�M�ؼЦ�m�p�Z���p��Y�ӭ�,�N�{����F�ؼ��I
        {
            pointIndex++;  //��F�ؼ�,�����U�ӥؼ�
            //��F���I
            if (pointIndex >= PathPoints.pathPoints.Length)
            {
                PathEnd();
                return;
            }
            target = PathPoints.pathPoints[pointIndex];  //target���ؼ��I
        }
    }
    private void PathEnd()
    {
        EnemySpawner.EnemyAlive--;
        Destroy(gameObject);  //�P��Enemy
    }
}
