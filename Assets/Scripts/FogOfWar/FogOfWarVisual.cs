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
    [SerializeField] BuildingManager buildingManager = null;
    [SerializeField] Team localPlayerTeam;
    [SerializeField] List<FOVGraphics> fovGraphicsList = new List<FOVGraphics>();

    [SerializeField] float towerVisualRadius = 20f;

    [SerializeField] float minionUpdateMeshInterval = 0.1f;
    [SerializeField] float minionVisualRadius = 5f;
    [SerializeField] int minionDegreePercast = 30;

    [SerializeField] float championUpdateMeshInterval = 0.05f;
    [SerializeField] float championVisualRadius = 15f;
    [SerializeField] int championDegreePercast = 10;

    private void Awake()
    {
        playerManager.OnLocalChampionReady += PlayerManager_OnLocalPlayerConnectionReady;
        minionManager.OnMinionAdded += MinionManager_OnMinionAdded;
        minionManager.OnMinionRemoved += MinionManager_OnMinionRemoved;
        buildingManager.OnTowerAdded += BuildingManager_OnTowerAdded;
        buildingManager.OnTowerRemoved += BuildingManager_OnTowerRemoved;
    }

    private void BuildingManager_OnTowerRemoved(Tower tower)
    {
        if (tower.GetTeam() == localPlayerTeam)
        {
            RemoveViewMesh(tower.gameObject);
        }
    }

    private void BuildingManager_OnTowerAdded(Tower tower)
    {
        if (tower.GetTeam() == localPlayerTeam)
        {
            AttachViewMesh(tower.gameObject, towerVisualRadius, 20, 10);
        }
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
                AttachViewMesh(player.gameObject, championVisualRadius, championDegreePercast, championUpdateMeshInterval);
            }
        }
    }

    private void MinionManager_OnMinionRemoved(Minion minion)
    {
        if(minion.GetTeam() == localPlayerTeam)
        {
            RemoveViewMesh(minion.gameObject);
        }
    }

    private void MinionManager_OnMinionAdded(Minion minion)
    {
        if (minion.GetTeam() == localPlayerTeam)
        {
            AttachViewMesh(minion.gameObject, minionVisualRadius, minionDegreePercast, minionUpdateMeshInterval);
        }
    }

    public void AttachViewMesh(GameObject go, float viewRadius, int degreePerCast, float updateInterval)
    {
        GameObject fovGraphicsGameObject = Instantiate(FOVGraphicsPrefab);
        fovGraphicsGameObject.transform.parent = go.transform;
        fovGraphicsGameObject.transform.localPosition = Vector3.zero;

        FOVGraphics fovGraphicsInstance = fovGraphicsGameObject.GetComponent<FOVGraphics>();
        fovGraphicsInstance.GenerateMesh(viewRadius, degreePerCast);
        fovGraphicsInstance.StartUpdateMesh(updateInterval);

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
