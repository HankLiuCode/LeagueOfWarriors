using Dota.Attributes;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SpawnGroup : NetworkBehaviour
{
    private int aliveCount;
    [SerializeField] List<SpawnData> spawnCandidates;
    [SerializeField] float spawnInterval = 10f;

    public static event System.Action<Monster> OnMonsterAdded;
    public static event System.Action<Monster> OnMonsterRemoved;
    public static event System.Action<SpawnGroup> OnAllDead;


    public float GetSpawnInterval()
    {
        return spawnInterval;
    }

    [Server]
    public void Spawn()
    {
        aliveCount = spawnCandidates.Count;

        foreach (SpawnData spawnData in spawnCandidates)
        {
            GameObject instance = Instantiate(spawnData.prefab, spawnData.spawnPoint.position, spawnData.spawnPoint.rotation);

            NetworkServer.Spawn(instance);

            Health health = instance.GetComponent<Health>();

            health.OnHealthDead += Health_OnHealthDead;

            Monster monster = instance.GetComponent<Monster>();

            OnMonsterAdded?.Invoke(monster);
        }
    }

    [Server]
    private void Health_OnHealthDead(Health health)
    {
        health.OnHealthDead -= Health_OnHealthDead;

        aliveCount -= 1;

        Monster monster = health.GetComponent<Monster>();

        OnMonsterRemoved?.Invoke(monster);

        if (aliveCount == 0)
        {
            OnAllDead?.Invoke(this);
        }
    }
}


[System.Serializable]
public class SpawnData
{
    public GameObject prefab;
    public Transform spawnPoint;
}
