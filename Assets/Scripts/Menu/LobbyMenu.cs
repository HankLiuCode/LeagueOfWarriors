using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Dota.Networking;
using System;

public class LobbyMenu : NetworkBehaviour
{
    [SerializeField] List<CardSlot> redSlots = new List<CardSlot>();
    [SerializeField] List<CardSlot> blueSlots = new List<CardSlot>();

    public override void OnStartClient()
    {
        DotaRoomPlayer.OnPlayerTeamModified += DotaRoomPlayer_OnPlayerTeamModified;
        DotaRoomPlayer.OnPlayerConnect += DotaRoomPlayer_OnPlayerConnect;
        DotaRoomPlayer.OnPlayerDisconnect += DotaRoomPlayer_OnPlayerDisconnect;
    }

    public override void OnStopClient()
    {
        DotaRoomPlayer.OnPlayerTeamModified -= DotaRoomPlayer_OnPlayerTeamModified;
        DotaRoomPlayer.OnPlayerConnect -= DotaRoomPlayer_OnPlayerConnect;
        DotaRoomPlayer.OnPlayerDisconnect -= DotaRoomPlayer_OnPlayerDisconnect;
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

    private void DotaRoomPlayer_OnPlayerTeamModified(DotaRoomPlayer player)
    {
        int redIndex = redSlots.FindIndex(slot => slot.GetPlayer() == player);
        int blueIndex = blueSlots.FindIndex(slot => slot.GetPlayer() == player);
        if(redIndex == -1 && blueIndex != -1)
        {
            blueSlots[blueIndex].SetPlayer(null);
            redSlots[redIndex].SetPlayer(player);
        }
        else if(redIndex != -1 && blueIndex == -1)
        {
            blueSlots[blueIndex].SetPlayer(player);
            redSlots[redIndex].SetPlayer(null);
        }
        else
        {
            Debug.Log("Player Not found in both slots");
        }
    }
}
