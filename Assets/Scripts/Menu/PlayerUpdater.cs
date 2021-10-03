using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Dota.Networking;
using System;

public class PlayerUpdater : NetworkBehaviour
{
    [SerializeField] List<DotaRoomPlayer> players = new List<DotaRoomPlayer>();
    [SerializeField] List<PlayerCard> cards = new List<PlayerCard>();

    private void Start()
    {
        if (isClient)
        {
            players.AddRange(FindObjectsOfType<DotaRoomPlayer>());
            UpdateCards();

            DotaRoomPlayer.ClientOnPlayerConnected += DotaRoomPlayer_OnPlayerConnect;
            DotaRoomPlayer.ClientOnPlayerDisconnected += DotaRoomPlayer_OnPlayerDisconnect;
            DotaRoomPlayer.ClientOnPlayerChampionModified += DotaRoomPlayer_ClientOnPlayerChampionModified;
            DotaRoomPlayer.ClientOnPlayerConnectionModified += DotaRoomPlayer_ClientOnPlayerConnectionModified;
            DotaRoomPlayer.ClientOnPlayerTeamModified += DotaRoomPlayer_ClientOnPlayerTeamModified;
        }
    }

    private void OnDestroy()
    {
        if (isClient)
        {
            DotaRoomPlayer.ClientOnPlayerConnected -= DotaRoomPlayer_OnPlayerConnect;
            DotaRoomPlayer.ClientOnPlayerDisconnected -= DotaRoomPlayer_OnPlayerDisconnect;
            DotaRoomPlayer.ClientOnPlayerChampionModified -= DotaRoomPlayer_ClientOnPlayerChampionModified;
            DotaRoomPlayer.ClientOnPlayerConnectionModified -= DotaRoomPlayer_ClientOnPlayerConnectionModified;
            DotaRoomPlayer.ClientOnPlayerTeamModified -= DotaRoomPlayer_ClientOnPlayerTeamModified;
        }
    }


    private void UpdateCards()
    {
        if(players.Count <= 0) { return; }

        DotaRoomPlayer localPlayer = NetworkClient.localPlayer.GetComponent<DotaRoomPlayer>();
        DotaRoomPlayer firstPlayer = players[0];

        int localPlayerIndex = players.IndexOf(localPlayer);
        players[0] = localPlayer;
        players[localPlayerIndex] = firstPlayer;


        for(int i=0; i<cards.Count; i++)
        {
            if(cards[i] == null) { continue; }
            if (i < players.Count)
            {
                cards[i].SetCard(players[i].GetTeam(), players[i].GetChampionId(), players[i].GetConnectionState());
            }
            else
            {
                cards[i].HideCard();
            }
        }
    }

    private void DotaRoomPlayer_ClientOnPlayerTeamModified(DotaRoomPlayer player)
    {
        UpdateCards();
    }

    private void DotaRoomPlayer_ClientOnPlayerConnectionModified(DotaRoomPlayer player)
    {
        UpdateCards();
    }

    private void DotaRoomPlayer_ClientOnPlayerChampionModified(DotaRoomPlayer player)
    {
        UpdateCards();
    }

    private void DotaRoomPlayer_OnPlayerConnect(DotaRoomPlayer player)
    {
        players.Add(player);
        UpdateCards();
    }

    private void DotaRoomPlayer_OnPlayerDisconnect(DotaRoomPlayer player)
    {
        players.Remove(player);
        UpdateCards();
    }
}
