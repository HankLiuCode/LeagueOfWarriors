using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : NetworkBehaviour
{
    [SerializeField] 
    [SyncVar]
    Team team;

    #region Server
    public void SetTeam(Team team)
    {
        this.team = team;
        gameObject.tag = team.ToString();
    }
    #endregion

    public Team GetTeam()
    {
        return team;
    }
}
