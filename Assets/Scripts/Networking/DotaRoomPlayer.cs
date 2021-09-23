using Mirror;
using System;
using System.Collections;
using UnityEngine;

namespace Dota.Networking
{
    public enum PlayerConnectionState
    {
        Offline = 0,
        RoomNotReady = 1,
        RoomReady = 2,
        RoomToGame = 3,
        Game = 4,
        GameToGameStats = 5,
        GameStatsToRoom = 6
    }

    public class DotaRoomPlayer : NetworkBehaviour, ITeamMember
    {
        [SerializeField] 
        [SyncVar]
        string playerName;

        [SerializeField]
        [SyncVar(hook = nameof(OnTeamChanged))]
        Team team;

        [SerializeField]
        [SyncVar(hook = nameof(OnChampionIdChanged))]
        int championId;

        [SerializeField]
        PlayerConnectionState playerConnState;

        public static event System.Action<DotaRoomPlayer> OnPlayerConnect;
        public static event System.Action<DotaRoomPlayer> OnPlayerDisconnect;
        public static event System.Action<DotaRoomPlayer> OnPlayerTeamModified;
        public static event System.Action<DotaRoomPlayer> OnPlayerChampionModified;
        public static event System.Action<DotaRoomPlayer, PlayerConnectionState> OnPlayerConnectionModified;


        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public string GetPlayerName()
        {
            return playerName;
        }

        public Team GetTeam()
        {
            return team;
        }

        public int GetChampionId()
        {
            return championId;
        }

        public PlayerConnectionState GetConnectionState()
        {
            return playerConnState;
        }

        public void OnChampionIdChanged(int oldValue, int newValue)
        {
            OnPlayerChampionModified?.Invoke(this);
        }

        public void OnTeamChanged(Team oldTeam, Team newTeam)
        {
            OnPlayerTeamModified?.Invoke(this);
        }

        public void OnPlayerConnectionChanged(PlayerConnectionState oldValue, PlayerConnectionState newValue)
        {
            OnPlayerConnectionModified?.Invoke(this, newValue);
        }

        public override void OnStartClient()
        {
            OnPlayerConnect?.Invoke(this);
        }

        public override void OnStopClient()
        {
            OnPlayerDisconnect?.Invoke(this);
        }

        #region Server

        [Command]
        public void CmdSetChampionId(int championId)
        {
            ServerSetChampionId(championId);
        }

        [Command]
        public void CmdSetTeam(Team team)
        {
            ServerSetTeam(team);
        }

        [Command]
        public void CmdSetConnectionState(PlayerConnectionState playerConnState)
        {
            ServerSetConnectionState(playerConnState);
        }

        [Server]
        public void ServerSetChampionId(int championId)
        {
            this.championId = championId;
        }

        [Server]
        public void ServerSetConnectionState(PlayerConnectionState playerConnState)
        {
            this.playerConnState = playerConnState;
        }

        [Server]
        public void ServerSetPlayerName(string playerName)
        {
            this.playerName = playerName;
        }

        [Server]
        public void ServerSetTeam(Team team)
        {
            this.team = team;
        }
        #endregion
    }
}