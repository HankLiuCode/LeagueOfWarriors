using Dota.Attributes;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum Lane
{
    Top,
    Middle,
    Bottom
}

public class MinionManager : NetworkBehaviour
{
    [SerializeField] GameObject blueMinionPrefab;
    [SerializeField] GameObject redMinionPrefab;


    [SerializeField] bool spawnMinion;
    [SerializeField] float firstWaveSpawnAfter = 5f;
    [SerializeField] int minionPerWave = 3;
    [SerializeField] float spawnInterval = 10f;

    [Header("Blue")]
    [SerializeField] Transform blueBase;
    [SerializeField] Transform[] blueStartPositions;
    [SerializeField] Transform[] topBlueTowers;
    [SerializeField] Transform[] middleBlueTowers;
    [SerializeField] Transform[] bottomBlueTowers;

    [Header("Red")]
    [SerializeField] Transform redBase;
    [SerializeField] Transform[] redStartPositions;
    [SerializeField] Transform[] topRedTowers;
    [SerializeField] Transform[] middleRedTowers;
    [SerializeField] Transform[] bottomRedTowers;

    SyncList<Minion> minions = new SyncList<Minion>();
    IEnumerator redSpawnRoutine;
    IEnumerator blueSpawnRoutine;


    public event System.Action<Minion> OnMinionAdded;
    public event System.Action<Minion> OnMinionRemoved;


    private void Start()
    {
        minions.Callback += OnMinionsUpdated;

        if (isServer)
        {
            redSpawnRoutine = SpawnWaveRoutine(Team.Red, firstWaveSpawnAfter, 1, spawnInterval);
            blueSpawnRoutine = SpawnWaveRoutine(Team.Blue, firstWaveSpawnAfter, 1, spawnInterval);
            StartCoroutine(redSpawnRoutine);
            StartCoroutine(blueSpawnRoutine);
        }
    }

    public SyncList<Minion> GetMinions()
    {
        return minions;
    }

    #region Server

    public void StartSpawnWave()
    {
        if(redSpawnRoutine != null) { StopCoroutine(redSpawnRoutine); }
        if (blueSpawnRoutine != null) { StopCoroutine(blueSpawnRoutine); }
        redSpawnRoutine = SpawnWaveRoutine(Team.Red, firstWaveSpawnAfter, 1, spawnInterval);
        blueSpawnRoutine = SpawnWaveRoutine(Team.Blue, firstWaveSpawnAfter, 1, spawnInterval);
        StartCoroutine(redSpawnRoutine);
        StartCoroutine(blueSpawnRoutine);
    }

    public void StopSpawnWave()
    {
        if (redSpawnRoutine != null) { StopCoroutine(redSpawnRoutine); }
        if (blueSpawnRoutine != null) { StopCoroutine(blueSpawnRoutine); }
    }

    //��{�ͦ��@�i�@�i�p�L
    /// <summary>
    /// </summary>
    /// <param name="time">�C���}�l��X��}�l�ͦ��h�L</param>
    /// <param name="delyTime">�P�@�i����Ӥp�L�ͦ������j</param>
    /// <param spwanTime="">�U�@�i�p�L�ͦ����ɶ����j</param>
    /// <returns></returns>
    IEnumerator SpawnWaveRoutine(Team team, float time, float delayTime, float spawnTime)
    {
        yield return new WaitForSeconds(time);

        while (spawnMinion)
        {
            //�@��for�`���N��@�i�p�L
            for (int i = 0; i < minionPerWave; i++)
            {
                switch (team)
                {
                    case Team.Red:
                        SpawnMinion(Team.Red, redStartPositions[1].position,middleBlueTowers, blueBase, Lane.Middle); // Mid
                        SpawnMinion(Team.Red, redStartPositions[0].position, topBlueTowers, blueBase, Lane.Top);    // Top
                        SpawnMinion(Team.Red, redStartPositions[2].position, bottomBlueTowers, blueBase, Lane.Bottom); // Bottom
                        break;

                    case Team.Blue:
                        SpawnMinion(Team.Blue, blueStartPositions[1].position, middleRedTowers, redBase, Lane.Middle); // Mid
                        SpawnMinion(Team.Blue, blueStartPositions[0].position, topRedTowers, redBase, Lane.Top);    // Top
                        SpawnMinion(Team.Blue, blueStartPositions[2].position, bottomRedTowers, redBase, Lane.Bottom); // Bottom
                        break;
                }

                yield return new WaitForSeconds(delayTime);
            }
            //���ݤU�@�i�p�L�ͦ����ɶ�
            yield return new WaitForSeconds(spawnTime);
        }
    }

    [Server]
    public void SpawnMinion(Team team, Vector3 spawnPosition, Transform[] towers, Transform targetBase, Lane lane)
    {
        GameObject minionPrefab = team == Team.Blue ? blueMinionPrefab : redMinionPrefab;

        GameObject minionInstance = Instantiate(minionPrefab, spawnPosition, GetMinionOrientation(team, lane));

        Minion minion = minionInstance.GetComponent<Minion>();

        Health health = minionInstance.GetComponent<Health>();

        health.OnHealthDead += Health_OnHealthDead;

        minion.SetTeam(team);

        minion.SetTowers(towers, targetBase);

        minion.SetRoad(GetMask(lane));

        NetworkServer.Spawn(minionInstance);

        minions.Add(minionInstance.GetComponent<Minion>());
    }

    private Quaternion GetMinionOrientation(Team team, Lane lane)
    {
        Quaternion orien = Quaternion.identity;

        if(team == Team.Red)
        {
            switch (lane)
            {
                case Lane.Top:
                    orien = Quaternion.Euler(0, -90, 0);
                    break;
                case Lane.Middle:
                    orien = Quaternion.Euler(0, -135, 0);
                    break;
                case Lane.Bottom:
                    orien = Quaternion.Euler(0, -180, 0);
                    break;
            }
        }
        else if(team == Team.Blue)
        {
            switch (lane)
            {
                case Lane.Top:
                    orien = Quaternion.Euler(0, 0, 0);
                    break;
                case Lane.Middle:
                    orien = Quaternion.Euler(0, 45, 0);
                    break;
                case Lane.Bottom:
                    orien = Quaternion.Euler(0, 90, 0);
                    break;
            }
        }

        return orien;
    }

    private int GetMask(Lane lane)
    {
        int mask = -1;

        switch (lane)
        {
            case Lane.Top:
                mask = 1 << 4;
                break;
            case Lane.Middle:
                mask = 1 << 3;
                break;
            case Lane.Bottom:
                mask = 1 << 5;
                break;
        }
        return mask;
    }

    private void Health_OnHealthDead(Health health)
    {
        Minion minion = health.GetComponent<Minion>();
        OnMinionRemoved?.Invoke(minion);
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
