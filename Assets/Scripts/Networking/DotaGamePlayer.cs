using Dota.Controls;
using Mirror;
using System;
using System.Collections;
using UnityEngine;

namespace Dota.Networking
{
    public class DotaGamePlayer : NetworkBehaviour
    {

        // Change to RPC Later
        [SerializeField] 
        [SyncVar]
        Team team;

        [SerializeField] 
        string playerName;

        [SerializeField]
        Sprite playerSprite;

        public static event Action<DotaGamePlayer> OnDotaGamePlayerStart;
        public static event Action<DotaGamePlayer> OnDotaGamePlayerStop;

        #region Server

        [Server]
        public void ServerSetPlayerName(string playerName)
        {
            this.playerName = playerName;
        }

        [Server]
        public void ServerSetTeam(Team team)
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
            OnDotaGamePlayerStart?.Invoke(this);
        }

        public override void OnStopClient()
        {
            ((DotaNetworkRoomManager)NetworkRoomManager.singleton).ClientRemoveDotaGamePlayer(this);
            OnDotaGamePlayerStop?.Invoke(this);
        }
        #endregion
    }
}