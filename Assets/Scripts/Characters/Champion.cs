using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Champion : NetworkBehaviour, ITeamMember
{
    [SyncVar]
    [SerializeField] 
    Team team;

    [SerializeField] Sprite icon;

    public Sprite GetIcon()
    {
        return icon;
    }

    public Team GetTeam()
    {
        return team;
    }

    public void SetTeam(Team team)
    {
        this.team = team;
        gameObject.tag = team.ToString();
    }
}
