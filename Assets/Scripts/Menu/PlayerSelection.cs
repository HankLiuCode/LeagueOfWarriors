using Dota.Networking;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelection : NetworkBehaviour
{
    DotaNewRoomPlayer localPlayer = null;

    public override void OnStartClient()
    {
        //DotaNewRoomPlayer[] players = FindObjectsOfType<DotaNewRoomPlayer>();

        //foreach (DotaNewRoomPlayer player in players)
        //{
        //    if (player.isLocalPlayer)
        //    {
        //        localPlayer = player;
        //        break;
        //    }
        //}

        DotaNewRoomPlayer.OnPlayerConnect += DotaNewRoomPlayer_OnPlayerConnect;
    }

    public override void OnStopClient()
    {
        DotaNewRoomPlayer.OnPlayerConnect -= DotaNewRoomPlayer_OnPlayerConnect;
    }

    private void DotaNewRoomPlayer_OnPlayerConnect(DotaNewRoomPlayer player)
    {
        if (player.isLocalPlayer)
        {
            localPlayer = player;
        }
    }

    public void ToggleReady()
    {
        bool isReady = localPlayer.GetIsReady();
        localPlayer.CmdSetReady(!isReady);
    }

    public void SwitchTeam()
    {
        Team team = localPlayer.GetTeam();
        if(team == Team.Red)
        {
            localPlayer.CmdSetTeam(Team.Blue);
        }
        else
        {
            localPlayer.CmdSetTeam(Team.Red);
        }
    }

    private void SetLocalPlayerChampionId(int championId)
    {
        localPlayer.CmdSetChampionId(championId);
    }

    private void SetLocalPlayerReady(bool ready)
    {
        localPlayer.CmdSetReady(ready);
    }

    private void SetLocalPlayerTeam(Team team)
    {
        localPlayer.CmdSetTeam(team);
    }

    private void SetLocalPlayerTeam(int teamId)
    {
        SetLocalPlayerTeam((Team)teamId);
    }
}
