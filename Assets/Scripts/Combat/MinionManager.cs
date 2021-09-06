using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionManager : NetworkBehaviour
{
    [SerializeField] GameObject minionPrefab;
    [SerializeField] float spawnInterval = 3f;

    [SerializeField]
    SyncList<NetworkIdentity> minionInstances = new SyncList<NetworkIdentity>();

    public event Action<NetworkIdentity> OnMinionAdded;
    public event Action<NetworkIdentity> OnMinionRemoved;

    float spawnTimer;

    public override void OnStartServer()
    {
        spawnTimer = spawnInterval;
        minionInstances.Callback += OnMinionInstancesUpdated;
        SpawnMinion(); 
    }

    public SyncList<NetworkIdentity> GetMinions()
    {
        return minionInstances;
    }

    #region Server
    private void Update()
    {
        //if (isServer)
        //{
        //    spawnTimer -= Time.deltaTime;
        //    if (spawnTimer <= 0)
        //    {
        //        SpawnMinion();
        //        spawnTimer = spawnInterval;
        //    }
        //}

        if (isClient)
        {
            //Debug.Log(minionInstances.Count);
        }
    }

    [Server]
    public void SpawnMinion()
    {
        GameObject minionInstance = Instantiate(minionPrefab);

        minionInstance.GetComponent<Minion>().SetTeam(Team.Blue);

        NetworkServer.Spawn(minionInstance);


        ////minionInstances.Add(minionInstance.GetComponent<NetworkIdentity>());
    }
    #endregion

    #region Client
    private void OnMinionInstancesUpdated(SyncList<NetworkIdentity>.Operation op, int itemIndex, NetworkIdentity oldItem, NetworkIdentity newItem)
    {
        switch (op)
        {
            case SyncList<NetworkIdentity>.Operation.OP_ADD:
                // index is where it got added in the list
                // newItem is the new item
                OnMinionAdded?.Invoke(newItem);
                break;
            case SyncList<NetworkIdentity>.Operation.OP_CLEAR:
                // list got cleared
                break;
            case SyncList<NetworkIdentity>.Operation.OP_INSERT:
                // index is where it got added in the list
                // newItem is the new item
                break;
            case SyncList<NetworkIdentity>.Operation.OP_REMOVEAT:
                // index is where it got removed in the list
                // oldItem is the item that was removed
                OnMinionRemoved?.Invoke(oldItem);
                break;
            case SyncList<NetworkIdentity>.Operation.OP_SET:
                // index is the index of the item that was updated
                // oldItem is the previous value for the item at the index
                // newItem is the new value for the item at the index
                break;
        }
    }
    #endregion
}
