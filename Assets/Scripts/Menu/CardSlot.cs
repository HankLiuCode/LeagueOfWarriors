using Dota.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSlot : MonoBehaviour
{
    public bool HasPlayer { get { return cardPlayer != null; } }
    [SerializeField] DotaRoomPlayer cardPlayer;
    [SerializeField] PlayerCard playerCardInstance;

    public DotaRoomPlayer GetPlayer()
    {
        return cardPlayer;
    }

    public void SetPlayer(DotaRoomPlayer player)
    {
        // if player is null hide playerCardInstance
        // if already has player, remove listeners from dotaRoomPlayer
        // if don't have player instantiate playercard and listen to player events

        if(cardPlayer != null)
        {
            DotaRoomPlayer.OnPlayerChampionModified -= DotaRoomPlayer_OnPlayerChampionModified;
            DotaRoomPlayer.OnPlayerConnectionModified -= DotaRoomPlayer_OnPlayerConnectionModified;
        }

        if (player == null)
        {
            cardPlayer = null;
            playerCardInstance.gameObject.SetActive(false);
            return;
        }

        cardPlayer = player;
        UpdateCardInfo(cardPlayer);
        DotaRoomPlayer.OnPlayerChampionModified += DotaRoomPlayer_OnPlayerChampionModified;
        DotaRoomPlayer.OnPlayerConnectionModified += DotaRoomPlayer_OnPlayerConnectionModified;
    }

    private void UpdateCardInfo(DotaRoomPlayer player)
    {
        Debug.Log("Update Card Info");

        playerCardInstance.gameObject.SetActive(true);
        playerCardInstance.SetCard(player.GetPlayerName(), player.GetTeam(), player.GetChampionId(), player.GetConnectionState());
        playerCardInstance.transform.localPosition = Vector3.zero;
    }

    private void DotaRoomPlayer_OnPlayerChampionModified(DotaRoomPlayer player)
    {
        if (cardPlayer == null || cardPlayer != player) { return; }
        UpdateCardInfo(player);
    }

    private void DotaRoomPlayer_OnPlayerConnectionModified(DotaRoomPlayer player, PlayerConnectionState connState)
    {
        if (cardPlayer == null || cardPlayer != player) { return; }
        UpdateCardInfo(player);
    }
}
