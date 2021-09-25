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

        if(cardPlayer != null)
        {
            DotaRoomPlayer.OnPlayerChampionModified -= DotaRoomPlayer_OnPlayerChampionModified;
            DotaRoomPlayer.OnPlayerConnectionModified -= DotaRoomPlayer_OnPlayerConnectionModified;
            DotaRoomPlayer.OnPlayerTeamModified -= DotaRoomPlayer_OnPlayerTeamModified;
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
        DotaRoomPlayer.OnPlayerTeamModified += DotaRoomPlayer_OnPlayerTeamModified;
    }

    private void UpdateCardInfo(DotaRoomPlayer player)
    {
        playerCardInstance.gameObject.SetActive(true);
        playerCardInstance.SetCard(player.GetPlayerName(), player.GetTeam(), player.GetChampionId(), player.GetConnectionState());
        playerCardInstance.transform.localPosition = Vector3.zero;
    }

    private void DotaRoomPlayer_OnPlayerChampionModified(DotaRoomPlayer player)
    {
        if (cardPlayer == null || cardPlayer != player) { return; }
        UpdateCardInfo(player);
    }

    private void DotaRoomPlayer_OnPlayerConnectionModified(DotaRoomPlayer player)
    {
        if (cardPlayer == null || cardPlayer != player) { return; }
        UpdateCardInfo(player);
    }

    private void DotaRoomPlayer_OnPlayerTeamModified(DotaRoomPlayer player)
    {
        if (cardPlayer == null || cardPlayer != player) { return; }
        UpdateCardInfo(player);
    }

    private void OnDestroy()
    {
        if(cardPlayer != null)
        {
            DotaRoomPlayer.OnPlayerChampionModified -= DotaRoomPlayer_OnPlayerChampionModified;
            DotaRoomPlayer.OnPlayerConnectionModified -= DotaRoomPlayer_OnPlayerConnectionModified;
            DotaRoomPlayer.OnPlayerTeamModified -= DotaRoomPlayer_OnPlayerTeamModified;
        }
    }
}
