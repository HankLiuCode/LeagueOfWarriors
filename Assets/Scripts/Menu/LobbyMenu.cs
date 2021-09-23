using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Dota.Networking;
using System;

public class LobbyMenu : NetworkBehaviour
{
    [SerializeField] List<CardSlot> slots = new List<CardSlot>();

    public override void OnStartClient()
    {
        //DotaNewRoomPlayer[] players = FindObjectsOfType<DotaNewRoomPlayer>();
        //foreach(DotaNewRoomPlayer player in players)
        //{
        //    foreach (CardSlot slot in slots)
        //    {
        //        if (!slot.HasPlayer && slot.IsSameTeam(player))
        //        {
        //            slot.SetPlayer(player);
        //            break;
        //        }
        //    }
        //}

        DotaNewRoomPlayer.OnPlayerModified += DotaGamePlayer_OnPlayerModified;
        DotaNewRoomPlayer.OnPlayerConnect += DotaGamePlayer_OnPlayerConnect;
        DotaNewRoomPlayer.OnPlayerDisconnect += DotaGamePlayer_OnPlayerLeave;
        DotaNewRoomPlayer.OnPlayerReady += DotaNewRoomPlayer_OnPlayerReady;
    }

    public override void OnStopClient()
    {
        Debug.Log("OnStopClient LobbyMenu");
        DotaNewRoomPlayer.OnPlayerModified -= DotaGamePlayer_OnPlayerModified;
        DotaNewRoomPlayer.OnPlayerConnect -= DotaGamePlayer_OnPlayerConnect;
        DotaNewRoomPlayer.OnPlayerDisconnect -= DotaGamePlayer_OnPlayerLeave;
        DotaNewRoomPlayer.OnPlayerReady -= DotaNewRoomPlayer_OnPlayerReady;
    }

    private void DotaNewRoomPlayer_OnPlayerReady(DotaNewRoomPlayer player)
    {
        foreach (CardSlot slot in slots)
        {
            if (slot.HasPlayer)
            {
                slot.UpdatePlayerInfo();
            }
        }
    }

    private void DotaGamePlayer_OnPlayerLeave(DotaNewRoomPlayer player)
    {
        foreach (CardSlot slot in slots)
        {
            if(slot.HasPlayer && slot.GetPlayer() == player)
            {
                slot.RemovePlayer();
                return;
            }
        }
        Debug.LogError("player Not Found in slots: " + player.name);
    }

    private void DotaGamePlayer_OnPlayerConnect(DotaNewRoomPlayer player)
    {
        foreach(CardSlot slot in slots)
        {
            if (!slot.HasPlayer && slot.IsSameTeam(player))
            {
                slot.SetPlayer(player);
                return;
            }
        }

        Debug.LogError("No Available slot for player: " + player.name);
    }

    private void DotaGamePlayer_OnPlayerModified(DotaNewRoomPlayer player)
    {
        CardSlot modifiedSlot = null;

        foreach (CardSlot slot in slots)
        {
            if (slot.HasPlayer && slot.GetPlayer() == player)
            {
                modifiedSlot = slot;
                break;
            }
        }

        if(modifiedSlot == null)
        {
            Debug.LogError("Modified Slots not found: " + player.name);
        }

        if (modifiedSlot.IsSameTeam(player))
        {
            modifiedSlot.UpdatePlayerInfo();
        }
        else
        {
            foreach (CardSlot slot in slots)
            {
                if (!slot.HasPlayer && slot.IsSameTeam(player))
                {
                    CardSlot.Swap(modifiedSlot, slot);
                }
            }
        }
    }
}
