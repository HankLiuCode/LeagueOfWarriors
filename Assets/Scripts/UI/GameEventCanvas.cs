using Dota.Networking;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventCanvas : NetworkBehaviour
{
    [SerializeField] Team localPlayerTeam;


    //public override void OnStartClient()
    //{
    //    Team team = NetworkClient.localPlayer.GetComponent<DotaRoomPlayer>().GetTeam();
    //    localPlayerTeam = team;
    //    Champion.ClientOnChampionDeadAttacker += Champion_ClientOnChampionDeadAttacker;
    //}

    //public override void OnStopClient()
    //{
    //    Champion.ClientOnChampionDeadAttacker -= Champion_ClientOnChampionDeadAttacker;
    //}

    private void Champion_ClientOnChampionDeadAttacker(Champion deadChampion, NetworkIdentity slayer)
    {
        // localPlayerDead
        bool isLocalPlayerDead = (deadChampion.GetOwner() == NetworkClient.localPlayer.GetComponent<DotaRoomPlayer>());
        Champion champion = slayer.GetComponent<Champion>();
        bool isLocalPlayerSlainEnemy = (champion.GetOwner() == NetworkClient.localPlayer.GetComponent<DotaRoomPlayer>());
        
        if (isLocalPlayerDead)
        {
            SelfSlain();
            return;
        }
        else if (isLocalPlayerSlainEnemy)
        {
            SelfSlainEnemy();
            return;
        }

        bool isDeadChampionSameTeam = deadChampion.GetTeam() == localPlayerTeam;

        if (isDeadChampionSameTeam)
        {
            AllySlain();
            return;
        }
        else
        {
            EnemySlain();
            return;
        }
    }

    private void SelfSlain()
    {
        Debug.Log("Self Slain");
    }

    private void SelfSlainEnemy()
    {
        Debug.Log("Self Slain Enemy");
    }

    private void AllySlain()
    {
        Debug.Log("Ally Slain");
    }

    private void EnemySlain()
    {
        Debug.Log("Enemy Slain");
    }
}
