using Dota.Networking;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventCanvas : NetworkBehaviour
{
    [SerializeField] Team localPlayerTeam;
    [SerializeField] GameEventNotification notification;


    public override void OnStartClient()
    {
        Team team = NetworkClient.localPlayer.GetComponent<DotaRoomPlayer>().GetTeam();
        localPlayerTeam = team;
        Champion.ClientOnChampionDeadAttacker += Champion_ClientOnChampionDeadAttacker;
    }

    public override void OnStopClient()
    {
        Champion.ClientOnChampionDeadAttacker -= Champion_ClientOnChampionDeadAttacker;
    }

    private void Champion_ClientOnChampionDeadAttacker(Champion deadChampion, NetworkIdentity slayer)
    {
        bool isLocalPlayerDead = (deadChampion.GetOwner() == NetworkClient.localPlayer.GetComponent<DotaRoomPlayer>());
        if (isLocalPlayerDead)
        {
            IIconOwner iconOwner = slayer.GetComponent<IIconOwner>();
            SelfSlain(iconOwner, deadChampion);
            return;
        }

        if(slayer != null)
        {
            Champion localChampion = slayer.GetComponent<Champion>();
            if(localChampion != null)
            {
                SelfSlainEnemy(localChampion, deadChampion);
                return;
            }
        }

        bool isDeadChampionSameTeam = deadChampion.GetTeam() == localPlayerTeam;
        if (isDeadChampionSameTeam)
        {
            IIconOwner iconOwner = slayer.GetComponent<IIconOwner>();
            AllySlain(iconOwner, deadChampion);
            return;
        }
        else
        {
            IIconOwner iconOwner = slayer.GetComponent<IIconOwner>();
            EnemySlain(iconOwner, deadChampion);
            return;
        }
    }

    private void SelfSlain(IIconOwner slayer, IIconOwner victim)
    {
        DisplayKillCanvas(slayer, victim);
        Debug.Log("Self Slain");
    }

    private void SelfSlainEnemy(IIconOwner slayer, IIconOwner victim)
    {
        DisplayKillCanvas(slayer, victim);
        Debug.Log("Self Slain Enemy");
    }

    private void AllySlain(IIconOwner slayer, IIconOwner victim)
    {
        DisplayKillCanvas(slayer, victim);
        Debug.Log("Ally Slain");
    }

    private void EnemySlain(IIconOwner slayer, IIconOwner victim)
    {
        DisplayKillCanvas(slayer, victim);
        Debug.Log("Enemy Slain");
    }

    private void DisplayKillCanvas(IIconOwner slayer, IIconOwner victim)
    {
        notification.SetNotification(slayer.GetIcon(), victim.GetIcon());
        StartCoroutine(ShowNotificationForSeconds(2f));
    }


    IEnumerator ShowNotificationForSeconds(float seconds)
    {
        notification.SetVisible(true);
        yield return new WaitForSeconds(seconds);
        notification.SetVisible(false);
    }
}
