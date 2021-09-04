using Dota.Networking;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarVisual : MonoBehaviour
{
    [SerializeField] GameObject FOVGraphicsPrefab = null;
    [SerializeField] Team localPlayerTeam;
    [SerializeField] List<FOVGraphics> fovGraphicsList = new List<FOVGraphics>();

    private void Start()
    {
        ((DotaNetworkRoomManager)NetworkRoomManager.singleton).OnAllPlayersAdded += VisionManager_OnAllPlayersAdded;
    }

    private void VisionManager_OnAllPlayersAdded()
    {
        Team localPlayerTeam = ((DotaNetworkRoomManager) NetworkRoomManager.singleton).GetLocalGamePlayer().GetTeam();

        this.localPlayerTeam = localPlayerTeam;

        List<DotaGamePlayer> dotaGamePlayers = ((DotaNetworkRoomManager)NetworkRoomManager.singleton).ClientGetDotaGamePlayers();

        Debug.Log("DotaGamePlayersCount" + dotaGamePlayers.Count);

        foreach(DotaGamePlayer dotaGamePlayer in dotaGamePlayers)
        {
            AttachViewMesh(dotaGamePlayer.gameObject);
        }
    }

    public void AttachViewMesh(GameObject go)
    {
        FOVGraphics fovGraphicsInstance = Instantiate(FOVGraphicsPrefab, Vector3.zero, Quaternion.identity, go.transform).GetComponent<FOVGraphics>();
        fovGraphicsList.Add(fovGraphicsInstance);
    }

    public void RemoveViewMesh(GameObject go)
    {
        FOVGraphics fovGraphics = go.GetComponentInChildren<FOVGraphics>();
        if(fovGraphics == null) { return; }

        fovGraphicsList.Remove(fovGraphics);
        Destroy(fovGraphics.gameObject);
    }
}
