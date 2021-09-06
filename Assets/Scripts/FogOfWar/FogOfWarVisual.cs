using Dota.Networking;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarVisual : MonoBehaviour
{
    [SerializeField] GameObject FOVGraphicsPrefab = null;
    [SerializeField] MinionManager minionManager = null;
    [SerializeField] Team localPlayerTeam;
    [SerializeField] List<FOVGraphics> fovGraphicsList = new List<FOVGraphics>();

    private void Start()
    {
        ((DotaNetworkRoomManager)NetworkRoomManager.singleton).OnAllGamePlayersAdded += FogOfWarVisual_OnAllGamePlayersAdded;
    }

    private void OnSelfMinionAdded(NetworkIdentity obj)
    {
        AttachViewMesh(obj.gameObject);
    }

    private void OnSelfMinionRemoved(NetworkIdentity obj)
    {
        RemoveViewMesh(obj.gameObject);
    }

    private void FogOfWarVisual_OnAllGamePlayersAdded()
    {
        Team localPlayerTeam = ((DotaNetworkRoomManager)NetworkRoomManager.singleton).GetLocalGamePlayer().GetTeam();

        this.localPlayerTeam = localPlayerTeam;

        List<DotaGamePlayer> dotaGamePlayers = ((DotaNetworkRoomManager)NetworkRoomManager.singleton).ClientGetDotaGamePlayers();

        foreach (DotaGamePlayer dotaGamePlayer in dotaGamePlayers)
        {
            if (dotaGamePlayer.GetTeam() == this.localPlayerTeam)
            {
                AttachViewMesh(dotaGamePlayer.gameObject);
            }
        }

        switch (localPlayerTeam)
        {
            case Team.Red:
                minionManager.OnRedMinionAdded += OnSelfMinionAdded;
                minionManager.OnRedMinionRemoved += OnSelfMinionRemoved;
                break;

            case Team.Blue:
                minionManager.OnBlueMinionAdded += OnSelfMinionAdded;
                minionManager.OnBlueMinionRemoved += OnSelfMinionRemoved;
                break;
        }
    }

    public void AttachViewMesh(GameObject go)
    {
        GameObject fovGraphicsGameObject = Instantiate(FOVGraphicsPrefab);
        fovGraphicsGameObject.transform.parent = go.transform;
        fovGraphicsGameObject.transform.localPosition = Vector3.zero;

        FOVGraphics fovGraphicsInstance = fovGraphicsGameObject.GetComponent<FOVGraphics>();

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
