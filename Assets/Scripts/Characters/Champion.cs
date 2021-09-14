using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Dota.Attributes;

public class Champion : NetworkBehaviour, ITeamMember, IIconOwner, IMinimapEntity
{
    [SyncVar]
    [SerializeField] 
    Team team;
    
    [SerializeField] Sprite icon = null;
    [SerializeField] GameObject minimapIconPrefab = null;
    [SerializeField] Health health = null;

    public event System.Action<Champion> OnChampionDead;

    #region Server
    [Server]
    public void ServerRevive()
    {
        health.ServerRevive();
    }
    #endregion

    private void Start()
    {
        health.OnHealthDead += Champion_OnHealthDead;
        health.OnHealthDeadEnd += Health_OnHealthDeadEnd;
        health.OnHealthRevive += Health_OnHealthRevive;
    }

    private void Health_OnHealthRevive()
    {
        gameObject.SetActive(true);
    }

    private void Health_OnHealthDeadEnd()
    {
        gameObject.SetActive(false);
    }

    private void Champion_OnHealthDead(Health health)
    {
        OnChampionDead?.Invoke(this);
    }

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
