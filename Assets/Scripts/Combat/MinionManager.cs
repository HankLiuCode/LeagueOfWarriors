using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionManager : NetworkBehaviour
{
    [SerializeField] GameObject minionPrefab;
    [SerializeField] float spawnInterval = 3f;

    [SerializeField] List<Transform> spawnPoints = new List<Transform>();

    SyncList<NetworkIdentity> redMinionInstances = new SyncList<NetworkIdentity>();
    SyncList<NetworkIdentity> blueMinionInstances = new SyncList<NetworkIdentity>();

    public event System.Action<NetworkIdentity> OnRedMinionAdded;
    public event System.Action<NetworkIdentity> OnRedMinionRemoved;

    public event System.Action<NetworkIdentity> OnBlueMinionAdded;
    public event System.Action<NetworkIdentity> OnBlueMinionRemoved;

    float spawnTimer;


    private void Start()
    {
        redMinionInstances.Callback += OnRedMinionInstancesUpdated;
        blueMinionInstances.Callback += OnBlueMinionInstancesUpdated;
    }

    public override void OnStartServer()
    {
        spawnTimer = spawnInterval;
    }

    public SyncList<NetworkIdentity> GetRedMinions()
    {
        return redMinionInstances;
    }

    public SyncList<NetworkIdentity> GetBlueMinions()
    {
        return blueMinionInstances;
    }

    #region Server
    private void Update()
    {
        if (isServer)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0)
            {
                //SpawnBlueMinion();
                //SpawnRedMinion();
                spawnTimer = spawnInterval;
            }
        }
    }

    [Server]
    public void SpawnBlueMinion()
    {
        GameObject minionInstance = Instantiate(minionPrefab, GetRandomSpawnPoint().position, Quaternion.identity);

        Minion minion = minionInstance.GetComponent<Minion>();

        minion.SetTeam(Team.Blue);

        NetworkServer.Spawn(minionInstance);

        blueMinionInstances.Add(minionInstance.GetComponent<NetworkIdentity>());
    }

    [Server]
    public void SpawnRedMinion()
    {
        GameObject minionInstance = Instantiate(minionPrefab, GetRandomSpawnPoint().position, Quaternion.identity);

        Minion minion = minionInstance.GetComponent<Minion>();

        minion.SetTeam(Team.Red);

        NetworkServer.Spawn(minionInstance);

        redMinionInstances.Add(minionInstance.GetComponent<NetworkIdentity>());
    }

    [Server]
    public Transform GetRandomSpawnPoint()
    {
        int index = Random.Range(0, spawnPoints.Count);
        return spawnPoints[index];
    }

    #endregion

    private void OnBlueMinionInstancesUpdated(SyncList<NetworkIdentity>.Operation op, int itemIndex, NetworkIdentity oldItem, NetworkIdentity newItem)
    {
        switch (op)
        {
            case SyncList<NetworkIdentity>.Operation.OP_ADD:
                // index is where it got added in the list
                // newItem is the new item
                OnBlueMinionAdded?.Invoke(newItem);
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
                OnBlueMinionRemoved?.Invoke(oldItem);
                break;
            case SyncList<NetworkIdentity>.Operation.OP_SET:
                // index is the index of the item that was updated
                // oldItem is the previous value for the item at the index
                // newItem is the new value for the item at the index
                break;
        }
    }

    private void OnRedMinionInstancesUpdated(SyncList<NetworkIdentity>.Operation op, int itemIndex, NetworkIdentity oldItem, NetworkIdentity newItem)
    {
        switch (op)
        {
            case SyncList<NetworkIdentity>.Operation.OP_ADD:
                // index is where it got added in the list
                // newItem is the new item
                OnRedMinionAdded?.Invoke(newItem);
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
                OnRedMinionRemoved?.Invoke(oldItem);
                break;
            case SyncList<NetworkIdentity>.Operation.OP_SET:
                // index is the index of the item that was updated
                // oldItem is the previous value for the item at the index
                // newItem is the new value for the item at the index
                break;
        }
    }
}
