using Dota.Controls;
using Mirror;
using System;
using System.Collections;
using UnityEngine;

namespace Dota.Networking
{
    public class DotaGamePlayer : NetworkBehaviour, ITeamMember
    {
        [SerializeField] 
        [SyncVar]
        Team team;

        [SerializeField] 
        [SyncVar]
        string playerName;

        [SerializeField]
        Sprite playerSprite;

        #region Server

        [Server]
        public void SetPlayerName(string playerName)
        {
            this.playerName = playerName;
        }

        [Server]
        public void SetTeam(Team team)
        {
            this.team = team;
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

        public override void OnStopClient()
        {
            
        }
        #endregion
    }
}