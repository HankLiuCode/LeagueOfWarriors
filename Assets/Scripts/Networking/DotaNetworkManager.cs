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
        
        public List<DotaPlayer> DotaPlayers = new List<DotaPlayer>();

        public static event Action OnPlayerChanged;

        #region Server
        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);

            DotaPlayer player = conn.identity.GetComponent<DotaPlayer>();

            OnPlayerChanged?.Invoke();

            DotaPlayers.Add(player);
        }

        public override void OnServerConnect(NetworkConnection conn)
        {
            GameObject testingDummy = Instantiate(testingDummyPrefab, Vector3.zero, Quaternion.identity);
            NetworkServer.Spawn(testingDummy);
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            // Removes DotaPlayer On the NetworkManager on the server side
            DotaPlayer player = conn.identity.GetComponent<DotaPlayer>();

            OnPlayerChanged?.Invoke();

            DotaPlayers.Remove(player);

            base.OnServerDisconnect(conn);
        }
        #endregion

        #region Client

        public DotaPlayer GetLocalPlayer()
        {
            foreach (DotaPlayer player in DotaPlayers)
            {
                if (player.isLocalPlayer)
                {
                    return player;
                }
            }
            return null;
        }
        #endregion

    }
}
