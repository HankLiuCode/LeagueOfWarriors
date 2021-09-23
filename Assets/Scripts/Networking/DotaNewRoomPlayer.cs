using Mirror;
using System;
using System.Collections;
using UnityEngine;

namespace Dota.Networking
{
    public class DotaNewRoomPlayer : NetworkBehaviour, ITeamMember
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
        [SyncVar(hook = nameof(OnIsReadyChanged))]
        bool isReady = false;

        public static event System.Action<DotaNewRoomPlayer> OnPlayerConnect;
        public static event System.Action<DotaNewRoomPlayer> OnPlayerDisconnect;
        public static event System.Action<DotaNewRoomPlayer> OnPlayerModified;
        public static event System.Action<DotaNewRoomPlayer> OnPlayerReady;


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

        public bool GetIsReady()
        {
            return isReady;
        }

        public void OnChampionIdChanged(int oldValue, int newValue)
        {
            OnPlayerModified?.Invoke(this);
        }

        public void OnTeamChanged(Team oldTeam, Team newTeam)
        {
            OnPlayerModified?.Invoke(this);
        }

        public void OnIsReadyChanged(bool oldValue, bool newValue)
        {
            OnPlayerReady?.Invoke(this);
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
        public void CmdSetReady(bool ready)
        {
            ServerSetReady(ready);
        }

        [Server]
        public void ServerSetChampionId(int championId)
        {
            this.championId = championId;
        }

        [Server]
        public void ServerSetReady(bool ready)
        {
            this.isReady = ready;
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