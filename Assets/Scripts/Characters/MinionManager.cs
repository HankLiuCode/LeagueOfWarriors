using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionManager : NetworkBehaviour
{
    [SerializeField] GameObject minionPrefab;
    [SerializeField] float spawnInterval = 3f;

    [SerializeField] List<Transform> spawnPoints = new List<Transform>();

    SyncList<Minion> minions = new SyncList<Minion>();

    public event System.Action<Minion> OnMinionAdded;
    public event System.Action<Minion> OnMinionRemoved;

    float spawnTimer;


    private void Start()
    {
        minions.Callback += OnMinionsUpdated;
    }

    public override void OnStartServer()
    {
        spawnTimer = spawnInterval;
    }

    public SyncList<Minion> GetMinions()
    {
        return minions;
    }

    #region Server
    private void Update()
    {
        if (isServer)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0)
            {
                SpawnMinion(Team.Blue);
                spawnTimer = spawnInterval;
            }
        }
    }

    [Server]
    public void SpawnMinion(Team team)
    {
        GameObject minionInstance = Instantiate(minionPrefab, GetRandomSpawnPoint().position, Quaternion.identity);

        Minion minion = minionInstance.GetComponent<Minion>();

        minion.SetTeam(team);

        NetworkServer.Spawn(minionInstance);

        minions.Add(minionInstance.GetComponent<Minion>());
    }

    [Server]
    public Transform GetRandomSpawnPoint()
    {
        int index = Random.Range(0, spawnPoints.Count);
        return spawnPoints[index];
    }

    #endregion

    private void OnMinionsUpdated(SyncList<Minion>.Operation op, int itemIndex, Minion oldItem, Minion newItem)
    {
        switch (op)
        {
            case SyncList<Minion>.Operation.OP_ADD:
                // index is where it got added in the list
                // newItem is the new item
                OnMinionAdded?.Invoke(newItem);
                break;
            case SyncList<Minion>.Operation.OP_CLEAR:
                // list got cleared
                break;
            case SyncList<Minion>.Operation.OP_INSERT:
                // index is where it got added in the list
                // newItem is the new item
                break;
            case SyncList<Minion>.Operation.OP_REMOVEAT:
                // index is where it got removed in the list
                // oldItem is the item that was removed
                OnMinionRemoved?.Invoke(oldItem);
                break;
            case SyncList<Minion>.Operation.OP_SET:
                // index is the index of the item that was updated
                // oldItem is the previous value for the item at the index
                // newItem is the new value for the item at the index
                break;
        }
    }
}
