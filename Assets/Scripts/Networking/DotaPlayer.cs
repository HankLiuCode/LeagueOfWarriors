using Dota.Controls;
using Dota.Core;
using Dota.Networking;
using Dota.UI;
using Mirror;
using UnityEngine;

namespace Dota.Networking
{
    public class DotaPlayer : NetworkBehaviour
    {
        [SerializeField] GameObject characterPrefab = null;
        [SerializeField] PlayerUISetup playerUISetupPrefab = null;

        // hide when finish debugging
        [SerializeField]  DotaPlayerController dotaPlayerController = null;

        // this is only not null on localPlayer
        PlayerUISetup playerUIInstance = null;

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

            if (dotaPlayerController.hasAuthority)
            {
                playerUIInstance = Instantiate(playerUISetupPrefab.gameObject).GetComponent<PlayerUISetup>();
                playerUIInstance.SetUpSelfUI(dotaPlayerController);
            }
            

            // Add player to DotaPlayers List on Client, Does not run on host, since it will cause duplicate DotaPlayers
            if (!NetworkServer.active)
            {
                ((DotaNetworkManager)NetworkManager.singleton).DotaPlayers.Add(this);
            }
        }

        public override void OnStopClient()
        {
            // Removes DotaPlayer On the NetworkManager on the client side
            ((DotaNetworkManager) NetworkManager.singleton).DotaPlayers.Remove(this);

            if (dotaPlayerController.hasAuthority)
            {
                playerUIInstance.DestroySelfUI();
            }
        }

        public DotaPlayerController GetPlayerControllerByConnection(NetworkConnection conn)
        {
            DotaPlayerController[] controllers = GameObject.FindObjectsOfType<DotaPlayerController>();
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