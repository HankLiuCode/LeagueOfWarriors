using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : NetworkBehaviour, ITeamMember, IIconOwner, IMinimapEntity
{
    [SerializeField] Team team;
    [SerializeField] Sprite baseIcon = null;

    [SerializeField] GameObject minimapIconPrefab = null;

    public static event System.Action<Base> OnBaseSpawned;
    public static event System.Action<Base> OnBaseDestroyed;

    #region Client

    public override void OnStartClient()
    {
        OnBaseSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        OnBaseDestroyed?.Invoke(this);
    }

    #endregion

    public Sprite GetIcon()
    {
        return baseIcon;
    }

    public string GetLayerName()
    {
        return "Building";
    }

    public MinimapIcon GetMinimapIconInstance()
    {
        MinimapDefaultIcon minimapIconInstance = Instantiate(minimapIconPrefab).GetComponent<MinimapDefaultIcon>();
        minimapIconInstance.SetVisible(false);
        minimapIconInstance.SetTeam(team);

        return minimapIconInstance;
    }

    public Team GetTeam()
    {
        return team;
    }

    public void ServerSetTeam(Team team)
    {
        this.team = team;
    }
}
