using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : NetworkBehaviour, ITeamMember, IIconOwner
{
    [SerializeField] 
    [SyncVar]
    Team team;

    [SerializeField] Sprite icon = null;
    
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

    public Sprite GetIcon()
    {
        return icon;
    }
}
