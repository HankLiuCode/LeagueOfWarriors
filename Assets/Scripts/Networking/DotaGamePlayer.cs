using Dota.Controls;
using Mirror;
using UnityEngine;

namespace Dota.Networking
{
    public class DotaGamePlayer : NetworkBehaviour
    {
        [SerializeField] GameObject characterPrefab = null;
        DotaPlayerController dotaPlayerController = null;

        public DotaPlayerController GetDotaPlayerController()
        {
            return dotaPlayerController;
        }

        #region Server
        public override void OnStartServer()
        {
            GameObject characterInstance = Instantiate(characterPrefab, Vector3.zero, Quaternion.identity);
            dotaPlayerController = characterInstance.GetComponent<DotaPlayerController>();
            NetworkServer.Spawn(characterInstance, connectionToClient);
        }

        #endregion

        #region Client
        public override void OnStartClient()
        {
            dotaPlayerController = GetPlayerControllerByConnection(connectionToServer);
            ((DotaNetworkRoomManager) NetworkRoomManager.singleton).OnAllPlayersAdded += DotaGamePlayer_OnAllPlayersAdded;
            ((DotaNetworkRoomManager)NetworkRoomManager.singleton).AddDotaGamePlayer(this);
        }

        private void DotaGamePlayer_OnAllPlayersAdded()
        {
            
        }

        public override void OnStopClient()
        {
            // Removes DotaPlayer On the NetworkManager on the client side
            ((DotaNetworkRoomManager)NetworkRoomManager.singleton).RemoveDotaGamePlayer(this);
        }

        public DotaPlayerController GetPlayerControllerByConnection(NetworkConnection conn)
        {
            DotaPlayerController[] controllers = FindObjectsOfType<DotaPlayerController>();
            foreach (DotaPlayerController player in controllers)
            {
                NetworkConnection playerConn = player.connectionToServer;
                if (playerConn == conn)
                {
                    return player;
                }
            }
            return null;
        }
        #endregion
    }
}