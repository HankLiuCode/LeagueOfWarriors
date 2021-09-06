using Dota.Controls;
using Mirror;
using System;
using System.Collections;
using UnityEngine;

namespace Dota.Networking
{
    public class DotaGamePlayer : NetworkBehaviour
    {
        [SerializeField] 
        Team team;

        [SerializeField] 
        string playerName;

        [SerializeField]
        Sprite playerSprite;

        public static event Action OnDotaGamePlayerStart;
        public static event Action OnDotaGamePlayerStop;

        #region Server

        [Server]
        public void ServerSetPlayerName(string playerName)
        {
            this.playerName = playerName;
            RpcSetPlayerName(playerName);
        }

        [Server]
        public void ServerSetTeam(Team team)
        {
            this.team = team;
            RpcSetTeam(team);
        }

        [ClientRpc]
        public void RpcSetPlayerName(string playerName)
        {
            this.playerName = playerName;
        }

        [ClientRpc]
        public void RpcSetTeam(Team team)
        {
            this.team = team;
            gameObject.tag = team.ToString();
        }

        #endregion

        public Team GetTeam()
        {
            return team;
        }

        public Sprite GetPlayerSprite()
        {
            return playerSprite;
        }

        #region Client

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