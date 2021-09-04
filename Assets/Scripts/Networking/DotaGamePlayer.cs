using Dota.Controls;
using Mirror;
using System.Collections;
using UnityEngine;

namespace Dota.Networking
{
    public class DotaGamePlayer : NetworkBehaviour
    {
        [SyncVar]
        [SerializeField] 
        Team team;

        [SyncVar]
        [SerializeField] 
        string playerName;

        [SyncVar]
        [SerializeField] 
        int championId;

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

        public override void OnStartClient()
        {
            ((DotaNetworkRoomManager)NetworkRoomManager.singleton).ClientAddDotaGamePlayer(this);
        }

        public override void OnStopClient()
        {
            ((DotaNetworkRoomManager)NetworkRoomManager.singleton).ClientRemoveDotaGamePlayer(this);
        }
        #endregion
    }
}