using Dota.Controls;
using Mirror;
using UnityEngine;

namespace Dota.Networking
{
    public class DotaGamePlayer : NetworkBehaviour
    {
        [SerializeField] GameObject characterPrefab = null;
        [SerializeField] UISetup UISetupPrefab = null;

        DotaPlayerController dotaPlayerController = null;
        UISetup UISetupInstance = null; // this is only not null on localPlayer

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

            if (hasAuthority)
            {
                UISetupInstance = Instantiate(UISetupPrefab.gameObject).GetComponent<UISetup>();
                UISetupInstance.SetUpUI(this);
            }

            // Add player to DotaPlayers List on Client
            // Players are already added to List on server by server side networkManager
            if (!NetworkServer.active)
            {
                ((DotaNetworkRoomManager)NetworkRoomManager.singleton).DotaGamePlayers.Add(this);
            }
        }

        public override void OnStopClient()
        {
            // Removes DotaPlayer On the NetworkManager on the client side
            ((DotaNetworkRoomManager) NetworkRoomManager.singleton).DotaGamePlayers.Remove(this);

            //if (dotaPlayerController.hasAuthority)
            //{
            //    UISetupInstance.DestroyAll();
            //}
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