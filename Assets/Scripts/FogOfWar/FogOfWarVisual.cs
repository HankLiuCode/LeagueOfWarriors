using Dota.Networking;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarVisual : MonoBehaviour
{
    [SerializeField] GameObject FOVGraphicsPrefab = null;
    [SerializeField] MinionManager minionManager = null;
    [SerializeField] PlayerManager playerManager = null;
    [SerializeField] Team localPlayerTeam;
    [SerializeField] List<FOVGraphics> fovGraphicsList = new List<FOVGraphics>();

    private void Awake()
    {
        playerManager.OnLocalChampionReady += PlayerManager_OnLocalPlayerConnectionReady;
    }

    private void PlayerManager_OnLocalPlayerConnectionReady()
    {
        Team localPlayerTeam = playerManager.GetLocalChampion().GetTeam();

        this.localPlayerTeam = localPlayerTeam;

        SyncList<Champion> players = playerManager.GetChampions();

        foreach (Champion player in players)
        {
            if (player.GetTeam() == this.localPlayerTeam)
            {
                AttachViewMesh(player.gameObject);
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

    private void OnSelfMinionAdded(NetworkIdentity obj)
    {
        AttachViewMesh(obj.gameObject);
    }

    private void OnSelfMinionRemoved(NetworkIdentity obj)
    {
        RemoveViewMesh(obj.gameObject);
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
