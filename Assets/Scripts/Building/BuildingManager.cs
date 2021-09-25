using Dota.Attributes;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : NetworkBehaviour
{
    [Header("Blue")]
    [SerializeField] Base blueBase;
    [SerializeField] Tower[] topBlueTowers;
    [SerializeField] Tower[] middleBlueTowers;
    [SerializeField] Tower[] bottomBlueTowers;

    [Header("Red")]
    [SerializeField] Base redBase;
    [SerializeField] Tower[] topRedTowers;
    [SerializeField] Tower[] middleRedTowers;
    [SerializeField] Tower[] bottomRedTowers;

    public event System.Action<Tower> OnTowerAdded;
    public event System.Action<Tower> OnTowerRemoved;

    public event System.Action<Base> OnBaseAdded;
    public event System.Action<Base> OnBaseRemoved;

    public Tower[] GetTowers(Team team, Lane lane)
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

    public Base GetBase(Team team)
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

    private void NotifyBaseAdded(Base teamBase, Team team)
    {
        Health health = teamBase.GetComponent<Health>();
        health.OnHealthDead += OnBaseDead;
        teamBase.ServerSetTeam(team);
        OnBaseAdded?.Invoke(teamBase);
    }

    private void NotifyTowersAdded(Tower[] towers, Team team)
    {
        foreach (Tower tower in towers)
        {
            Health health = tower.GetComponent<Health>();
            health.OnHealthDead += OnTowerDead;
            tower.ServerSetTeam(team);
            OnTowerAdded?.Invoke(tower);
        }
    }

    private void OnBaseDead(Health health)
    {
        Base b = health.GetComponent<Base>();
        Debug.Log("Game Over!");
        OnBaseRemoved?.Invoke(b);
    }

    private void OnTowerDead(Health health)
    {
        Tower t = health.GetComponent<Tower>();
        OnTowerRemoved?.Invoke(t);
    }
}
