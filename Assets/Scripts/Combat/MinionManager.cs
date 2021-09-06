using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionManager : NetworkBehaviour
{
    [SerializeField] GameObject minionPrefab;
    [SerializeField] float spawnInterval = 10f;

    [SerializeField]
    SyncList<NetworkIdentity> minionInstances = new SyncList<NetworkIdentity>();

    public event Action OnMinionListUpdated;

    float spawnTimer;

    private void Start()
    {
        minionInstances.Callback += OnMinionInstancesUpdated;
    }

    #region Server
    private void Update()
    {
        if (isServer)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0)
            {
                SpawnMinion();
                spawnTimer = spawnInterval;
            }
        }

        if (isClient)
        {
            Debug.Log(minionInstances.Count);
        }
    }

    public void SpawnMinion()
    {
        GameObject minionInstance = Instantiate(minionPrefab, Vector3.zero, Quaternion.identity);
        minionInstances.Add(minionInstance.GetComponent<NetworkIdentity>());
        NetworkServer.Spawn(minionInstance);
    }


    private void OnMinionInstancesUpdated(SyncList<NetworkIdentity>.Operation op, int itemIndex, NetworkIdentity oldItem, NetworkIdentity newItem)
    {
        OnMinionListUpdated?.Invoke();
    }

    #endregion
}
