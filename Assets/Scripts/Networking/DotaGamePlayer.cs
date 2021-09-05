using Dota.Controls;
using Mirror;
using System;
using System.Collections;
using UnityEngine;

namespace Dota.Networking
{
    public class DotaGamePlayer : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnTeamValueChanged))]
        [SerializeField] 
        Team team;

        [SyncVar]
        [SerializeField] 
        string playerName;

        [SyncVar]
        [SerializeField] 
        int championId;

        public static event Action OnDotaGamePlayerStart;
        public static event Action OnDotaGamePlayerStop;

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

        public Team GetTeam()
        {
            return team;
        }

        #region Client

        private void OnTeamValueChanged(Team oldTeam, Team newTeam)
        {
            gameObject.tag = newTeam.ToString();
        }

        public override void OnStartClient()
        {
            ((DotaNetworkRoomManager)NetworkRoomManager.singleton).ClientAddDotaGamePlayer(this);
            OnDotaGamePlayerStart?.Invoke();
        }

        public override void OnStopClient()
        {
            ((DotaNetworkRoomManager)NetworkRoomManager.singleton).ClientRemoveDotaGamePlayer(this);
            OnDotaGamePlayerStop?.Invoke();
        }
        #endregion
    }
}