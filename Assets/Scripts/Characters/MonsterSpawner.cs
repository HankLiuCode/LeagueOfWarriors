using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MonsterSpawner : NetworkBehaviour
{
    [SerializeField] List<SpawnGroup> spawnGroups;
    
    public override void OnStartServer()
    {
        foreach (SpawnGroup spawnGroup in spawnGroups)
        {
            StartCoroutine(StartSpawnRoutine(spawnGroup));
        }

        SpawnGroup.OnGroupDead += SpawnGroup_OnAllDead;
    }

    [Server]
    private void SpawnGroup_OnAllDead(SpawnGroup spawnGroup)
    {
        StartCoroutine(StartSpawnRoutine(spawnGroup));
    }

    [Server]
    IEnumerator StartSpawnRoutine(SpawnGroup spawnGroup)
    {
        yield return new WaitForSeconds(spawnGroup.GetSpawnInterval());
        spawnGroup.Spawn();
    }
}