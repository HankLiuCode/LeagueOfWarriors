using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBullet : MonoBehaviour
{
    private Transform m_Target;
    public float speed = 80;  //子彈速度
    public void SetTarget(Transform target)
    {
        m_Target = target;
    }
    void Start()
    {
        
    }

   
    void Update()
    {
        if (m_Target == null) return;  //判斷目標是否為空
        Vector3 dir = m_Target.position - transform.position;
        if (Vector3.Distance(m_Target.position, transform.position) < speed * Time.deltaTime)
        {
            //擊中目標
            return;
        }
        transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);
    }
    private void HitTarget()
    {
        //敵人扣血 銷毀自己
        Destroy(gameObject);
    }
}
