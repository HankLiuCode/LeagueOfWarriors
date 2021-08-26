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
        [SerializeField] HealthDisplay healthDisplay = null;
        [SerializeField] ManaDisplay manaDisplay = null;
        [SerializeField] CameraController cameraController = null; // client
        DotaPlayerController dotaPlayerController = null; // server

        // 1. Need to know if player is in lobby, is in game, is disconnected
        // 2. Need to know what champion the player chose

        #region Server
        public override void OnStartServer()
        {
            GameObject characterInstance = Instantiate(characterPrefab, Vector3.zero, Quaternion.identity);
            dotaPlayerController = characterInstance.GetComponent<DotaPlayerController>();
            NetworkServer.Spawn(characterInstance, connectionToClient);
        }
        #endregion


        #region Client
        public override void OnStartAuthority()
        {
            DotaPlayerController localPlayerController = FindLocalPlayerController();
            cameraController.gameObject.SetActive(true);
            cameraController.Initialize(localPlayerController.transform);

            healthDisplay.gameObject.SetActive(true);
            healthDisplay.SetHealth(localPlayerController.GetComponent<Health>());

            manaDisplay.gameObject.SetActive(true);
            manaDisplay.SetMana(localPlayerController.GetComponent<Mana>());
        }

        public override void OnStartClient()
        {
            if (NetworkServer.active) { return; } //if is running as server

            ((DotaNetworkManager) NetworkManager.singleton).DotaPlayers.Add(this);
        }

        public override void OnStopClient()
        {
            // Removes DotaPlayer On the NetworkManager on the client side
            ((DotaNetworkManager) NetworkManager.singleton).DotaPlayers.Remove(this);

            if (!hasAuthority) { return; }

            cameraController.gameObject.SetActive(false);
            healthDisplay.gameObject.SetActive(false);
        }

        public DotaPlayerController FindLocalPlayerController()
        {
            DotaPlayerController[] controllers = GameObject.FindObjectsOfType<DotaPlayerController>();
            foreach (DotaPlayerController dpc in controllers)
            {
                if (dpc.hasAuthority)
                {
                    return dpc;
                }
            }
            return null;
        }
        #endregion
    }

}