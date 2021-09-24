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


    public static event System.Action<Champion> OnChampionSpawned;
    public static event System.Action<Champion> OnChampionDestroyed;
    public event System.Action<Champion> OnChampionDead;



    #region Server
    [Server]
    public void ServerRevive()
    {
        health.ServerRevive();
    }

    #endregion
    // Both
    private void Start()
    {
        health.OnHealthDead += Champion_OnHealthDead;
        health.OnHealthDeadEnd += Health_OnHealthDeadEnd;
        health.OnHealthRevive += Health_OnHealthRevive;
    }

    private void Health_OnHealthDeadEnd()
    {
        gameObject.SetActive(false);
    }

    private void Champion_OnHealthDead(Health health)
    {
        OnChampionDead?.Invoke(this);
    }
    
    private void Health_OnHealthRevive()
    {
        gameObject.SetActive(true);
    }

    public Sprite GetIcon()
    {
        return icon;
    }

    public string GetLayerName()
    {
        return "Champion";
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

    public void ServerSetTeam(Team team)
    {
        this.team = team;
        gameObject.tag = team.ToString();
    }
}
