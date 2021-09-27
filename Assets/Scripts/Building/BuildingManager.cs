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
}
