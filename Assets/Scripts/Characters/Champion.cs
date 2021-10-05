using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Dota.Attributes;
using Dota.Networking;

public class Champion : NetworkBehaviour, ITeamMember, IIconOwner, IMinimapEntity, IRewardReceiver
{
    public const float REVIVE_TIME = 15f;


    [SyncVar]
    [SerializeField] 
    Team team;

    [SerializeField] Sprite icon = null;
    [SerializeField] GameObject minimapIconPrefab = null;
    [SerializeField] Health health = null;
    [SerializeField] Disolver disolver = null;
    [SerializeField] float dealthAnimDuration = 3f;
    [SerializeField] Level level = null;
    [SerializeField] StatStore statStore = null;

    [SyncVar]
    DotaRoomPlayer owner;

    public static event System.Action<Champion> OnChampionSpawned;
    public static event System.Action<Champion> OnChampionDestroyed;

    public static event System.Action<Champion> ServerOnChampionDead;
    public static event System.Action<Champion> ClientOnChampionDead;

    #region Client

    public override void OnStartClient()
    {
        OnChampionSpawned?.Invoke(this);
        health.ClientOnHealthDead += Health_ClientOnHealthDead;
        health.ClientOnHealthDeadEnd += Health_ClientOnHealthDeadEnd;
    }

    private void Health_ClientOnHealthDeadEnd()
    {
        disolver.StartDisolve();
    }

    private void Health_ClientOnHealthDead(Health health)
    {
        ClientOnChampionDead?.Invoke(this);
    }

    public override void OnStopClient()
    {
        OnChampionDestroyed?.Invoke(this);
    }

    #endregion

    #region Server

    [ServerCallback]
    private void Update()
    {
        health.ServerHeal(statStore.GetStats().healthRegen * Time.deltaTime);
    }

    public override void OnStartServer()
    {
        health.ServerOnHealthDead += Health_ServerOnHealthDead;
    }

    private void Health_ServerOnHealthDead(Health health)
    {
        StartCoroutine(DestroyAfter(disolver.GetDisolveDuration() + dealthAnimDuration));
        ServerOnChampionDead?.Invoke(this);
    }

    IEnumerator DestroyAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        NetworkServer.Destroy(gameObject);
    }

    public void ServerSetOwner(DotaRoomPlayer player)
    {
        owner = player;
    }

    public void ServerSetTeam(Team team)
    {
        this.team = team;
        gameObject.tag = team.ToString();
    }
    #endregion

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

    public DotaRoomPlayer GetOwner()
    {
        return owner;
    }

    public void SendExp(float exp)
    {
        level.AddExperience(exp);
    }
}
