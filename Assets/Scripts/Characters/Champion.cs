using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Champion : NetworkBehaviour, ITeamMember, IIconOwner, IMinimapEntity
{
    [SyncVar]
    [SerializeField] 
    Team team;

    [SerializeField] Sprite icon;
    [SerializeField] GameObject minimapIconPrefab;

    public Sprite GetIcon()
    {
        return icon;
    }

    public string GetLayerName()
    {
        return "Champion";
    }

    public Sprite GetMinimapIcon()
    {
        return icon;
    }

    public MinimapIcon GetMinimapIconInstance()
    {
        MinimapPlayerIcon minimapIconInstance = Instantiate(minimapIconPrefab).GetComponent<MinimapPlayerIcon>();
        minimapIconInstance.SetTeam(team);
        minimapIconInstance.SetPlayerIcon(icon);
        return minimapIconInstance;
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
