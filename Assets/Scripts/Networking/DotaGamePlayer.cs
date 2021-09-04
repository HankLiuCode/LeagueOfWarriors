using Dota.Controls;
using Mirror;
using UnityEngine;

namespace Dota.Networking
{
    public class DotaGamePlayer : NetworkBehaviour
    {
        [SerializeField] GameObject characterPrefab = null;

        [SyncVar]
        [SerializeField] 
        Team team;

        [SyncVar]
        [SerializeField] 
        string playerName;

        [SyncVar]
        [SerializeField] 
        int championId;

        DotaPlayerController dotaPlayerController = null;

        [Server]
        public void ServerSetPlayerName(string playerName)
        {
            this.playerName = playerName;
        }

        [Server]
        public void ServerSetTeam(Team team)
        {
            this.team = team;
        }

        public Team GetTeam()
        {
            return team;
        }

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
            //if (hasAuthority)
            //{
            //    gameObject.name = "LocalPlayer";
            //    dotaPlayerController.name = "LocalController";
            //}
            dotaPlayerController = GetPlayerControllerByConnection(connectionToServer);
            Debug.Log(connectionToServer);
            dotaPlayerController.SetTeam(team);
            ((DotaNetworkRoomManager)NetworkRoomManager.singleton).ClientAddDotaGamePlayer(this);
            ((DotaNetworkRoomManager)NetworkRoomManager.singleton).OnAllPlayersAdded += DotaGamePlayer_OnAllPlayersAdded;
        }

        private void DotaGamePlayer_OnAllPlayersAdded()
        {
            
        }

        public override void OnStopClient()
        {
            // Removes DotaPlayer On the NetworkManager on the client side
            ((DotaNetworkRoomManager)NetworkRoomManager.singleton).ClientRemoveDotaGamePlayer(this);
        }

        private DotaPlayerController GetPlayerControllerByConnection(NetworkConnection conn)
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