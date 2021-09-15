using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : NetworkBehaviour
{
    [SerializeField] PlayerManager playerManager = null;

    [Header("Blue")]
    [SerializeField] Transform blueBase;
    [SerializeField] Transform[] topBlueTowers;
    [SerializeField] Transform[] middleBlueTowers;
    [SerializeField] Transform[] bottomBlueTowers;

    [Header("Red")]
    [SerializeField] Transform redBase;
    [SerializeField] Transform[] topRedTowers;
    [SerializeField] Transform[] middleRedTowers;
    [SerializeField] Transform[] bottomRedTowers;

    public event System.Action<Tower> OnTowerAdded;
    public event System.Action<Tower> OnTowerRemoved;

    private void Awake()
    {
        playerManager.OnLocalChampionReady += PlayerManager_OnLocalChampionReady;
    }

    private void PlayerManager_OnLocalChampionReady()
    {
        NotifyBuildingsAdded();
    }

    public Transform[] GetTowers(Team team, Lane lane)
    {
        if (team == Team.Red)
        {
            switch (lane)
            {
                case Lane.Top:
                    return topRedTowers;

                case Lane.Middle:
                    return middleRedTowers;

                case Lane.Bottom:
                    return bottomRedTowers;
            }
        }
        else if (team == Team.Blue)
        {
            switch (lane)
            {
                case Lane.Top:
                    return topBlueTowers;

                case Lane.Middle:
                    return middleBlueTowers;

                case Lane.Bottom:
                    return bottomBlueTowers;
            }
        }

        throw new System.Exception("Specified Tower Doesn't Exist");
    }

    public Transform GetBase(Team team)
    {
        switch (team)
        {
            case Team.Red:
                return redBase;
            case Team.Blue:
                return blueBase;
        }

        throw new System.Exception("Base of team: " + team + "Doesn't Exist");
    }


    private void NotifyBuildingsAdded()
    {
        NotifyTowersAdded(topBlueTowers, Team.Blue);
        NotifyTowersAdded(middleBlueTowers, Team.Blue);
        NotifyTowersAdded(bottomBlueTowers, Team.Blue);

        NotifyTowersAdded(topRedTowers, Team.Red);
        NotifyTowersAdded(middleRedTowers, Team.Red);
        NotifyTowersAdded(bottomRedTowers, Team.Red);
    }

    private void NotifyTowersAdded(Transform[] towers, Team team)
    {
        foreach (Transform tower in towers)
        {
            Tower t = tower.GetComponent<Tower>();
            t.SetTeam(team);
            OnTowerAdded?.Invoke(t);
        }
    }
}
