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
        GameToRoom = 5
    }

    public class DotaRoomPlayer : NetworkBehaviour, ITeamMember
    {
        public const int MAX_CHAMPIONS = 2;

        [SerializeField]
        [SyncVar(hook = nameof(OnTeamChanged))]
        Team team;

        [SerializeField]
        [SyncVar(hook = nameof(OnChampionIdChanged))]
        int championId;

        [SerializeField]
        [SyncVar(hook = nameof(OnPlayerConnectionChanged))]
        PlayerConnectionState playerConnState;

        public static event System.Action<DotaRoomPlayer> ClientOnPlayerConnected;
        public static event System.Action<DotaRoomPlayer> ClientOnPlayerDisconnected;

        public static event System.Action<DotaRoomPlayer> ClientOnPlayerTeamModified;
        public static event System.Action<DotaRoomPlayer> ClientOnPlayerChampionModified;
        public static event System.Action<DotaRoomPlayer> ClientOnPlayerConnectionModified;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
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

        #region Client

        public override void OnStartClient()
        {
            if (hasAuthority)
            {
                CmdSetConnectionState(PlayerConnectionState.RoomNotReady);
            }
            ClientOnPlayerConnected?.Invoke(this);
        }

        public override void OnStopClient()
        {
            ClientOnPlayerDisconnected?.Invoke(this);
        }

        public void OnChampionIdChanged(int oldValue, int newValue)
        {
            ClientOnPlayerChampionModified?.Invoke(this);
        }

        public void OnTeamChanged(Team oldTeam, Team newTeam)
        {
            ClientOnPlayerTeamModified?.Invoke(this);
        }

        public void OnPlayerConnectionChanged(PlayerConnectionState oldValue, PlayerConnectionState newValue)
        {
            ClientOnPlayerConnectionModified?.Invoke(this);
        }

        public void ClientSetTeam(Team team)
        {
            CmdSetTeam(team);
        }

        public void ClientSetChampionId(int championId)
        {
            CmdSetChampionId(championId);
        }

        public void ClientSetConnectionState(PlayerConnectionState playerConnState)
        {
            CmdSetConnectionState(playerConnState);
        }

        #endregion


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
            this.championId = (championId + MAX_CHAMPIONS) % MAX_CHAMPIONS;
        }

        [Server]
        public void ServerSetConnectionState(PlayerConnectionState playerConnState)
        {
            this.playerConnState = playerConnState;
        }

        [Server]
        public void ServerSetTeam(Team team)
        {
            this.team = team;
        }
        #endregion
    }
}