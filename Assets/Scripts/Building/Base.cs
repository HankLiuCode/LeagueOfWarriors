using Dota.Attributes;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : NetworkBehaviour, ITeamMember, IIconOwner, IMinimapEntity
{
    [SerializeField] Team team;
    [SerializeField] Sprite baseIcon = null;

    [SerializeField] GameObject minimapIconPrefab = null;
    [SerializeField] Health health = null;
    [SerializeField] Disolver disolver = null;
    [SerializeField] GameObject destroyEffect = null;

    public static event System.Action<Base> OnBaseSpawned;
    public static event System.Action<Base> OnBaseDestroyed;

    public static event System.Action<Base> ServerOnBaseDead;
    public static event System.Action<Base> ClientOnBaseDead;

    #region Client

    public override void OnStartClient()
    {
        OnBaseSpawned?.Invoke(this);
        health.ClientOnHealthDead += Health_ClientOnHealthDead;
    }

    private void Health_ClientOnHealthDead(Health health)
    {
        disolver.StartDisolve();
        ClientOnBaseDead?.Invoke(this);
    }

    public override void OnStopClient()
    {
        OnBaseDestroyed?.Invoke(this);
    }

    #endregion

    #region Server
    public override void OnStartServer()
    {
        health.ServerOnHealthDead += Health_ServerOnHealthDead;
    }

    private void Health_ServerOnHealthDead(Health obj)
    {
        GameObject deathEffectInstance = Instantiate(destroyEffect, transform);
        deathEffectInstance.transform.localPosition = Vector3.up;
        NetworkServer.Spawn(deathEffectInstance);

        ServerOnBaseDead?.Invoke(this);
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
