using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MonsterManager : NetworkBehaviour
{
    [SerializeField] List<SpawnGroup> spawnGroups;

    public event System.Action<Monster> OnMonsterAdded;
    public event System.Action<Monster> OnMonsterRemoved;
    
    public override void OnStartServer()
    {
        SpawnGroup.OnMonsterAdded += SpawnGroup_OnMonsterAdded;
        SpawnGroup.OnMonsterRemoved += SpawnGroup_OnMonsterRemoved;

        SpawnGroup.OnAllDead += SpawnGroup_OnAllDead;

        foreach (SpawnGroup spawnGroup in spawnGroups)
        {
            StartCoroutine(StartSpawnRoutine(spawnGroup));
        }    
    }

    [Server]
    private void SpawnGroup_OnMonsterRemoved(Monster monster)
    {
        OnMonsterRemoved?.Invoke(monster);
        RpcNotifyMonsterRemoved(monster);
    }

    [Server]
    private void SpawnGroup_OnMonsterAdded(Monster monster)
    {
        OnMonsterAdded?.Invoke(monster);
        RpcNotifyMonsterAdded(monster);
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

    #region Client
    [ClientRpc]
    public void RpcNotifyMonsterAdded(Monster monster)
    {
        OnMonsterAdded?.Invoke(monster);
    }

    [ClientRpc]
    public void RpcNotifyMonsterRemoved(Monster monster)
    {
        OnMonsterRemoved?.Invoke(monster);
    }
    #endregion

}