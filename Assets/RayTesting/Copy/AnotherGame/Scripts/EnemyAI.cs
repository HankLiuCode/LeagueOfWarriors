using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 10;  //避免移動太快,給一個固定的移動速度
    Transform target;  //需找到所有路徑及下個目標點
    private int pointIndex = 0;
    void Start()  
    {
        target = PathPoints.pathPoints[pointIndex];  //給target賦值
    }

    
    void Update()  //更新敵人位置(這裡是一個向量:方向和長度)
    {
        Vector3 dir = target.position - transform.position;  //目標點減掉當前位置=要移動的方向和長度
        transform.Translate(dir.normalized * moveSpeed * Time.deltaTime, Space.World);  //只要方向不要長度所以用normalized作為方向,有返回值
        if (Vector3.Distance(target.position, transform.position) < 0.2f)   //判斷當前位置和目標位置如距離小於某個值,就認為到達目標點
        {
            pointIndex++;  //到達目標,切換下個目標
            //到達終點
            if (pointIndex >= PathPoints.pathPoints.Length)
            {
                PathEnd();
                return;
            }
            target = PathPoints.pathPoints[pointIndex];  //target為目標點
        }
    }
    private void PathEnd()
    {
        EnemySpawner.EnemyAlive--;
        Destroy(gameObject);  //銷毀Enemy
    }
}
