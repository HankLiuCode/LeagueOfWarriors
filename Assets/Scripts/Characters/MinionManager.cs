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

    [SerializeField] Transform[] blueStartPositions;
    [SerializeField] Transform[] redStartPositions;
    [SerializeField] BuildingManager buildingManager = null;

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

    //協程生成一波一波小兵
    /// <summary>
    /// </summary>
    /// <param name="time">遊戲開始後幾秒開始生成士兵</param>
    /// <param name="delyTime">同一波內兩個小兵生成的間隔</param>
    /// <param spwanTime="">下一波小兵生成的時間間隔</param>
    /// <returns></returns>
    IEnumerator SpawnWaveRoutine(Team team, float time, float delayTime, float spawnTime)
    {
        yield return new WaitForSeconds(time);

        while (spawnMinion)
        {
            for (int i = 0; i < minionPerWave; i++)
            {
                switch (team)
                {
                    case Team.Red:
                        SpawnMinion(Team.Red, redStartPositions[1].position, buildingManager.GetTowers(Team.Blue, Lane.Middle), buildingManager.GetBase(Team.Blue), Lane.Middle); // Mid
                        //SpawnMinion(Team.Red, redStartPositions[0].position, buildingManager.GetTowers(Team.Blue, Lane.Top), buildingManager.GetBase(Team.Blue), Lane.Top);    // Top
                        //SpawnMinion(Team.Red, redStartPositions[2].position, buildingManager.GetTowers(Team.Blue, Lane.Bottom), buildingManager.GetBase(Team.Blue), Lane.Bottom); // Bottom
                        break;

                    case Team.Blue:
                        SpawnMinion(Team.Blue, blueStartPositions[1].position, buildingManager.GetTowers(Team.Red, Lane.Middle), buildingManager.GetBase(Team.Red), Lane.Middle); // Mid
                        //SpawnMinion(Team.Blue, blueStartPositions[0].position, buildingManager.GetTowers(Team.Red, Lane.Top), buildingManager.GetBase(Team.Red), Lane.Top);    // Top
                        //SpawnMinion(Team.Blue, blueStartPositions[2].position, buildingManager.GetTowers(Team.Red, Lane.Bottom), buildingManager.GetBase(Team.Red), Lane.Bottom); // Bottom
                        break;
                }

                yield return new WaitForSeconds(delayTime);
            }
            //等待下一波小兵生成的時間
            yield return new WaitForSeconds(spawnTime);
        }
    }

    [Server]
    public void SpawnMinion(Team team, Vector3 spawnPosition, Tower[] towers, Base targetBase, Lane lane)
    {
        GameObject minionPrefab = team == Team.Blue ? blueMinionPrefab : redMinionPrefab;

        GameObject minionInstance = Instantiate(minionPrefab, spawnPosition, GetMinionOrientation(team, lane));

        Minion minion = minionInstance.GetComponent<Minion>();

        Health health = minionInstance.GetComponent<Health>();

        health.OnHealthDead += Health_OnHealthDead;

        minion.ServerSetTeam(team);

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
        RpcNotifyMinionRemoved(minion);
    }

    #endregion

    #region Client

    [ClientRpc]
    public void RpcNotifyMinionRemoved(Minion minion)
    {
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
