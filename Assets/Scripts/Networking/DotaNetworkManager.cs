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

        // change to private when finish testing
        public List<DotaPlayer> DotaPlayers = new List<DotaPlayer>();

        #region Server
        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);

            // this only adds player to the player list on the server
            // in other words the clients networkmanager won't have the correct list, needs to add manually

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
            DotaPlayer player = conn.identity.GetComponent<DotaPlayer>();
            DotaPlayers.Remove(player);

            base.OnServerDisconnect(conn);
        }
        #endregion
    }
}
