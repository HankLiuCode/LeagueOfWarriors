using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : NetworkBehaviour, ITeamMember, IIconOwner, IMinimapEntity
{
    [SerializeField] 
    [SyncVar]
    Team team;

    [SerializeField] Sprite icon = null;
    [SerializeField] Sprite minimapIcon = null;
    [SerializeField] GameObject minimapIconPrefab = null;
    
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

    public Sprite GetMinimapIcon()
    {
        return minimapIcon;
    }

    public MinimapIcon GetMinimapIconInstance()
    {
        MinimapIcon minimapIconInstance = Instantiate(minimapIconPrefab).GetComponent<MinimapIcon>();
        minimapIconInstance.SetVisible(false);
        minimapIconInstance.SetTeam(team);

        return minimapIconInstance;
    }

    public string GetLayerName()
    {
        return "Minion";
    }
}
