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
        [SerializeField] GameObject hudPrefab = null;

        public override void OnServerConnect(NetworkConnection conn)
        {
            GameObject testingDummy = Instantiate(testingDummyPrefab, Vector3.zero, Quaternion.identity);
            NetworkServer.Spawn(testingDummy);
        }

    }
}
