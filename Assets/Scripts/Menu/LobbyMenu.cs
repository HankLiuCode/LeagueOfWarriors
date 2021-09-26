using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Dota.Networking;
using System;

public class LobbyMenu : NetworkBehaviour
{
    [SerializeField] List<DotaRoomPlayer> players = new List<DotaRoomPlayer>();

    [SerializeField] List<CardSlot> redSlots = new List<CardSlot>();
    [SerializeField] List<CardSlot> blueSlots = new List<CardSlot>();

    public override void OnStartClient()
    {
        DotaRoomPlayer.ClientOnPlayerConnected += DotaRoomPlayer_OnPlayerConnect;
        DotaRoomPlayer.ClientOnPlayerDisconnected += DotaRoomPlayer_OnPlayerDisconnect;
    }

    public override void OnStopClient()
    {
        DotaRoomPlayer.ClientOnPlayerConnected -= DotaRoomPlayer_OnPlayerConnect;
        DotaRoomPlayer.ClientOnPlayerDisconnected -= DotaRoomPlayer_OnPlayerDisconnect;
    }

    private void DotaRoomPlayer_OnPlayerConnect(DotaRoomPlayer player)
    {
        foreach(CardSlot slot in redSlots)
        {
            if (!slot.HasPlayer)
            {
                slot.SetPlayer(player);
                return;
            }
        }

        foreach (CardSlot slot in blueSlots)
        {
            if (!slot.HasPlayer)
            {
                slot.SetPlayer(player);
                return;
            }
        }

        Debug.LogError("Room Is Full");
    }

    private void DotaRoomPlayer_OnPlayerDisconnect(DotaRoomPlayer player)
    {
        foreach (CardSlot slot in redSlots)
        {
            if(slot.GetPlayer() == player)
            {
                slot.SetPlayer(null);
                return;
            }
        }

        foreach (CardSlot slot in blueSlots)
        {
            if (slot.GetPlayer() == player)
            {
                slot.SetPlayer(null);
                return;
            }
        }

        Debug.LogError("Disconnected Player Not found");
    }
}
