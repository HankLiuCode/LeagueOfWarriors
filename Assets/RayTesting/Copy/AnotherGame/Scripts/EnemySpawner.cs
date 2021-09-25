using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static int EnemyAlive;
    public Wave[] waveEnemy;
    public Transform spawnPoint;  //�ͦ���m
    public float spawnInterval;  //�ĤH�ͦ����j�ɶ�
    private float countDown = 1f;  //�˭p��(1��ͦ��@��)
    private int waveIndex;
    void Start()
    {
        countDown = spawnInterval;  //���
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
        if(countDown <= 0)  //�˭p�ɵ���
        {
            //�A�����
            countDown = spawnInterval;  //�˭p�ɵ���,�ͦ��ĤH

            SpawnEnemy();  //�˭p�ɵ�����եγo�Ө禡
        }
    }
    private void SpawnEnemy()  //�ͦ��ĤH��k
    {
        StartCoroutine(WaveEnemy());  //�Ұʨ�{

    }
    IEnumerator WaveEnemy()
    {
        if(waveIndex >= waveEnemy.Length)
        {
            yield break;
        }

        //���X��e�@�i���ƾ�
        Wave wave = waveEnemy[waveIndex];
        //�O���s�����ƶq
        EnemyAlive = wave.count;


        for (int i = 0; i < wave.count; i++)
        {
            Instantiate(wave.enemyPrefab, spawnPoint.position, spawnPoint.rotation);  //��ҤƼĤH�Ѽ�
            yield return new WaitForSeconds(1/wave.rate);  
        }
        waveIndex++;  //�i��
    }
}
