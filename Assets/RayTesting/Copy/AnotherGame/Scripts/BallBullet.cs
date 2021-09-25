using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBullet : MonoBehaviour
{
    private Transform m_Target;
    public float speed = 80;  //�l�u�t��
    public void SetTarget(Transform target)
    {
        m_Target = target;
    }
    void Start()
    {
        
    }

   
    void Update()
    {
        if (m_Target == null) return;  //�P�_�ؼЬO�_����
        Vector3 dir = m_Target.position - transform.position;
        if (Vector3.Distance(m_Target.position, transform.position) < speed * Time.deltaTime)
        {
            //�����ؼ�
            return;
        }
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);
    }
    private void HitTarget()
    {
        //�ĤH���� �P���ۤv
        Destroy(gameObject);
    }
}
