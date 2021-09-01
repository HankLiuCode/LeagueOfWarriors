using Dota.Networking;
using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class FOVEntity : NetworkBehaviour
{
    [SerializeField] SkinnedMeshRenderer skinnedMeshRenderer = null;

    [SerializeField] float viewRadius = 5f;

    [SerializeField] bool isVisible = true;

    // Cache
    List<DotaGamePlayer> dotaGamePlayers;
    List<FOVEntity> fovEntities;

    private void PopulateFOVEntities()
    {
        dotaGamePlayers = ((DotaNetworkRoomManager)NetworkRoomManager.singleton).DotaGamePlayers;
        fovEntities = new List<FOVEntity>();
        foreach (DotaGamePlayer dp in dotaGamePlayers)
        {
            fovEntities.Add(dp.GetDotaPlayerController().GetComponent<FOVEntity>());
        }
    }

    private void Update()
    {
        if (!hasAuthority) { return; }
        
        // this is a hack need to get from roomnetworkManager
        if(fovEntities == null)
        {
            PopulateFOVEntities();
        }

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
            if(fovEntity == this) { continue; }

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
