using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

namespace Dota.Networking
{
    public class DotaNetworkManager : NetworkManager
    {
        [SerializeField] GameObject testingDummyPrefab = null;
        
        public List<DotaGamePlayer> DotaPlayers = new List<DotaGamePlayer>();

        public event Action OnPlayerChanged;

        #region Server
        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);

            DotaGamePlayer player = conn.identity.GetComponent<DotaGamePlayer>();

            DotaPlayers.Add(player);

            OnPlayerChanged?.Invoke();

            Debug.Log("Server Add Player");
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            // Removes DotaPlayer On the NetworkManager on the server side
            DotaGamePlayer player = conn.identity.GetComponent<DotaGamePlayer>();

            DotaPlayers.Remove(player);

            OnPlayerChanged?.Invoke();

            base.OnServerDisconnect(conn);
        }
        #endregion
    }
}
