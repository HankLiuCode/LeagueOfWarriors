using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static int EnemyAlive;
    public Wave[] waveEnemy;
    public Transform spawnPoint;  //生成位置
    public float spawnInterval;  //敵人生成間隔時間
    private float countDown = 1f;  //倒計時(1秒生成一次)
    private int waveIndex;
    void Start()
    {
        countDown = spawnInterval;  //賦值
    }

    void Update()
    {
        if(EnemyAlive >0)
        {
            return;
        }
        if(waveIndex == waveEnemy.Length)
        {
            Debug.Log("win!!");
        }
        countDown -= Time.deltaTime;
        if(countDown <= 0)  //倒計時結束
        {
            //再次賦值
            countDown = spawnInterval;  //倒計時結束,生成敵人

            SpawnEnemy();  //倒計時結束後調用這個函式
        }
    }
    private void SpawnEnemy()  //生成敵人方法
    {
        StartCoroutine(WaveEnemy());  //啟動協程

    }
    IEnumerator WaveEnemy()
    {
        if(waveIndex >= waveEnemy.Length)
        {
            yield break;
        }

        //取出當前一波的數據
        Wave wave = waveEnemy[waveIndex];
        //記錄存活的數量
        EnemyAlive = wave.count;


        for (int i = 0; i < wave.count; i++)
        {
            Instantiate(wave.enemyPrefab, spawnPoint.position, spawnPoint.rotation);  //實例化敵人參數
            yield return new WaitForSeconds(1/wave.rate);  
        }
        waveIndex++;  //波數
    }
}
