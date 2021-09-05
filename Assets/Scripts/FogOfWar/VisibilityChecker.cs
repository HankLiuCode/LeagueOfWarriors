using Dota.Networking;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityChecker : MonoBehaviour
{
    [SerializeField] Team localPlayerTeam;
    [SerializeField] float checkRadius = 10f;
    [SerializeField] List<GameObject> allies = new List<GameObject>();
    [SerializeField] List<GameObject> enemies = new List<GameObject>();

    [SerializeField] List<GameObject> enemiesInSight = new List<GameObject>();
    [SerializeField] LayerMask obstacleLayer = new LayerMask();
    [SerializeField] LayerMask visionCheckLayer = new LayerMask();
    
    private void Start()
    {
        ((DotaNetworkRoomManager)NetworkRoomManager.singleton).OnAllPlayersAdded += VisibilityChecker_OnAllPlayersAdded;
        DotaGamePlayer.OnDotaGamePlayerStop += DotaGamePlayer_OnDotaGamePlayerStop;
    }

    private void DotaGamePlayer_OnDotaGamePlayerStop()
    {
        
    }

    public List<GameObject> GetVisibleEnemies()
    {
        return enemiesInSight;
    }

    public List<GameObject> GetAllies()
    {
        return allies;
    }

    private void VisibilityChecker_OnAllPlayersAdded()
    {
        localPlayerTeam = ((DotaNetworkRoomManager)NetworkRoomManager.singleton).GetLocalGamePlayer().GetTeam();

        List<DotaGamePlayer> sameTeamGamePlayers;
        List<DotaGamePlayer> otherTeamGamePlayers;

        switch (localPlayerTeam)
        {
            case Team.Red:
                sameTeamGamePlayers = ((DotaNetworkRoomManager) NetworkRoomManager.singleton).ClientGetRedTeamGamePlayers();
                foreach (DotaGamePlayer sameTeamGamePlayer in sameTeamGamePlayers)
                {
                    allies.Add(sameTeamGamePlayer.gameObject);
                }

                otherTeamGamePlayers = ((DotaNetworkRoomManager) NetworkRoomManager.singleton).ClientGetBlueTeamGamePlayers();
                foreach (DotaGamePlayer otherTeamGamePlayer in otherTeamGamePlayers)
                {
                    enemies.Add(otherTeamGamePlayer.gameObject);
                }
                break;

            case Team.Blue:
                sameTeamGamePlayers = ((DotaNetworkRoomManager)NetworkRoomManager.singleton).ClientGetBlueTeamGamePlayers();
                foreach (DotaGamePlayer gamePlayer in sameTeamGamePlayers)
                {
                    allies.Add(gamePlayer.gameObject);
                }

                otherTeamGamePlayers = ((DotaNetworkRoomManager)NetworkRoomManager.singleton).ClientGetRedTeamGamePlayers();
                foreach (DotaGamePlayer otherTeamGamePlayer in otherTeamGamePlayers)
                {
                    enemies.Add(otherTeamGamePlayer.gameObject);
                }
                break;
        }
    }

    private void Update()
    {
        UpdateVisibleEnemyList();
        AdjustEnemyVisibility();
    }

    private void AdjustEnemyVisibility()
    {
        foreach(GameObject enemy in enemies)
        {
            SkinnedMeshRenderer renderer = enemy.GetComponentInChildren<SkinnedMeshRenderer>();
            renderer.enabled = false;
        }

        foreach(GameObject visibleEnemy in enemiesInSight)
        {
            SkinnedMeshRenderer renderer = visibleEnemy.GetComponentInChildren<SkinnedMeshRenderer>();
            renderer.enabled = true;
        }
    }

    private void UpdateVisibleEnemyList()
    {
        enemiesInSight.Clear();
        foreach(GameObject ally in allies)
        {
            Collider[] colliders = Physics.OverlapSphere(ally.transform.position, checkRadius, visionCheckLayer);
            foreach(Collider c in colliders)
            {
                Vector3 direction = c.transform.position - ally.transform.position;
                bool hasObstacle = Physics.Raycast(ally.transform.position, direction, direction.magnitude, obstacleLayer);

                if(c.tag != localPlayerTeam.ToString() && !hasObstacle)
                {
                    enemiesInSight.Add(c.gameObject);
                }
            }
        }
    }
}
