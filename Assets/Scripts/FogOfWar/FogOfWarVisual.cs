using Dota.Networking;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarVisual : NetworkBehaviour
{
    [SerializeField] Team localPlayerTeam;
    [SerializeField] GameObject FOVGraphicsPrefab = null;
    [SerializeField] MinionSpawner minionManager = null;
    [SerializeField] List<FOVGraphics> fovGraphicsList = new List<FOVGraphics>();

    [SerializeField] float minionUpdateMeshInterval = 0.1f;
    [SerializeField] int minionDegreePercast = 30;

    [SerializeField] float championUpdateMeshInterval = 0.05f;
    [SerializeField] int championDegreePercast = 10;

    private void Awake()
    {
        Team team = NetworkClient.localPlayer.GetComponent<DotaRoomPlayer>().GetTeam();
        localPlayerTeam = team;

        Champion.OnChampionSpawned += Champion_OnChampionSpawned;
        Champion.ClientOnChampionDead += Champion_ClientOnChampionDead;

        Tower.OnTowerSpawned += Tower_OnTowerSpawned;
        Tower.OnTowerDestroyed += Tower_OnTowerDestroyed;

        Base.OnBaseSpawned += Base_OnBaseSpawned;

        Minion.OnMinionSpawned += Minion_OnMinionSpawned;
        Minion.OnMinionDestroyed += Minion_OnMinionDestroyed;
    }

    private void Minion_OnMinionSpawned(Minion minion)
    {
        VisionEntity visionEntity = minion.GetComponent<VisionEntity>();
        if (minion.GetTeam() == localPlayerTeam)
        {
            AttachViewMesh(minion.gameObject, visionEntity.GetViewRadius(), minionDegreePercast, minionUpdateMeshInterval);
        }
    }

    private void Minion_OnMinionDestroyed(Minion minion)
    {
        if (minion.GetTeam() == localPlayerTeam)
        {
            RemoveViewMesh(minion.gameObject);
        }
    }

    private void Base_OnBaseSpawned(Base teamBase)
    {
        VisionEntity visionEntity = teamBase.GetComponent<VisionEntity>();
        if (teamBase.GetTeam() == localPlayerTeam)
        {
            AttachViewMesh(teamBase.gameObject, visionEntity.GetViewRadius(), 20, 10);
        }
    }

    private void Tower_OnTowerDestroyed(Tower tower)
    {
        if (tower.GetTeam() == localPlayerTeam)
        {
            RemoveViewMesh(tower.gameObject);
        }
    }

    private void Tower_OnTowerSpawned(Tower tower)
    {
        VisionEntity visionEntity = tower.GetComponent<VisionEntity>();
        if (tower.GetTeam() == localPlayerTeam)
        {
            AttachViewMesh(tower.gameObject, visionEntity.GetViewRadius(), 20, 10);
        }
    }

    private void Champion_OnChampionSpawned(Champion champion)
    {
        VisionEntity visionEntity = champion.GetComponent<VisionEntity>();
        if (champion.GetTeam() == localPlayerTeam)
        {
            AttachViewMesh(champion.gameObject, visionEntity.GetViewRadius(), 20, 10);
        }
    }

    private void Champion_ClientOnChampionDead(Champion champion)
    {
        Debug.Log("Champion Dead: " + champion.name);

        if (champion.GetTeam() == localPlayerTeam)
        {
            RemoveViewMesh(champion.gameObject);
        }
    }

    public void AttachViewMesh(GameObject go, float viewRadius, int degreePerCast, float updateInterval)
    {
        if(go.GetComponentInChildren<FOVGraphics>() != null) 
        {
            Debug.Log(go.name + "Already has ViewMesh Attached");
            return;
        }

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
