using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Dota.UI;
using Dota.Core;

namespace Dota.Networking
{
    public class DotaNetworkManager : NetworkManager
    {
        [SerializeField] GameObject testingDummyPrefab = null;
        
        public List<DotaPlayer> DotaPlayers = new List<DotaPlayer>();

        #region Server
        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);

            DotaPlayer player = conn.identity.GetComponent<DotaPlayer>();

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
            DotaPlayers.Remove(player);

            base.OnServerDisconnect(conn);
        }
        #endregion
    }
}
