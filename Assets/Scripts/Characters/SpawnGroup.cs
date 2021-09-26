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

    public static event System.Action<SpawnGroup> OnGroupDead;

    public List<Monster> alive = new List<Monster>();
    
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
            Monster monster = Instantiate(spawnData.monsterPrefab, spawnData.spawnPoint.position, spawnData.spawnPoint.rotation).GetComponent<Monster>();

            NetworkServer.Spawn(monster.gameObject);

            Health health = monster.GetComponent<Health>();

            alive.Add(monster);

            health.ServerOnHealthDead += Health_ServerOnHealthDead;
        }
    }

    [Server]
    private void Health_ServerOnHealthDead(Health health)
    {
        Monster monster = health.GetComponent<Monster>();

        alive.Remove(monster);

        if (alive.Count == 0)
        {
            OnGroupDead?.Invoke(this);
        }
    }
}


[System.Serializable]
public class SpawnData
{
    public GameObject monsterPrefab;
    public Transform spawnPoint;
}
