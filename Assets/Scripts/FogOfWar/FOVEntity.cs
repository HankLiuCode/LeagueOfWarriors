using Dota.Networking;
using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class FOVEntity : NetworkBehaviour
{
    [SerializeField] SkinnedMeshRenderer skinnedMeshRenderer = null;

    [SerializeField] float viewRadius = 5f;

    [SerializeField] bool isVisible = true;

    List<FOVEntity> fovEntities = new List<FOVEntity>();

    private void Start()
    {
        ((DotaNetworkRoomManager) NetworkRoomManager.singleton).OnAllPlayersAdded += FOVEntity_OnAllPlayersAdded;

        List<DotaGamePlayer> dotaGamePlayers = ((DotaNetworkRoomManager)NetworkRoomManager.singleton).ClientGetDotaGamePlayers();
        foreach (DotaGamePlayer dp in dotaGamePlayers)
        {
            if (!dp.isLocalPlayer)
            {
                fovEntities.Add(dp.GetComponent<FOVEntity>());
            }
        }
    }

    private void FOVEntity_OnAllPlayersAdded()
    {
        List<DotaGamePlayer> dotaGamePlayers = ((DotaNetworkRoomManager)NetworkRoomManager.singleton).ClientGetDotaGamePlayers();
        foreach (DotaGamePlayer dp in dotaGamePlayers)
        {
            if (!dp.isLocalPlayer)
            {
                fovEntities.Add(dp.GetComponent<FOVEntity>());
            }
        }
    }

    private void Update()
    {
        if (!hasAuthority) { return; }
        CheckVisibility();
    }

    public void SetVisible(bool visible)
    {
        skinnedMeshRenderer.enabled = visible;
        isVisible = visible;
    }

    public void CheckVisibility()
    {
        foreach(FOVEntity fovEntity in fovEntities)
        {
            if(Vector3.Distance(fovEntity.transform.position, transform.position) > viewRadius)
            {
                fovEntity.SetVisible(false);
            }
            else
            {
                fovEntity.SetVisible(true);
            }
        }
    }
}
