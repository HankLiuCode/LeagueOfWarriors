using Dota.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSlot : MonoBehaviour
{
    [SerializeField] PlayerCard playerCardPrefab;
    [SerializeField] DotaNewRoomPlayer cardPlayer;
    [SerializeField] Team team;
    public bool HasPlayer { get { return cardPlayer != null; } }

    PlayerCard playerCardInstance;

    public DotaNewRoomPlayer GetPlayer()
    {
        return cardPlayer;
    }

    public Team GetTeam()
    {
        return team;
    }

    public bool IsSameTeam(DotaNewRoomPlayer player)
    {
        if (player.GetTeam() == team) return true;
        return false;
    }

    public void UpdatePlayerInfo()
    {
        if(cardPlayer == null) { return; }
        playerCardInstance.SetCard(cardPlayer.GetPlayerName(), cardPlayer.GetTeam(), cardPlayer.GetChampionId(), cardPlayer.GetIsReady());
    }

    public static void Swap(CardSlot slot1, CardSlot slot2)
    {
        if (slot1.HasPlayer && slot2.HasPlayer) 
        {
            DotaNewRoomPlayer slot1Player = slot1.GetPlayer();
            DotaNewRoomPlayer slot2Player = slot2.GetPlayer();

            slot1.RemovePlayer();
            slot2.RemovePlayer();

            slot1.SetPlayer(slot2Player);
            slot2.SetPlayer(slot1Player);
        }
        else if(slot1.HasPlayer && !slot2.HasPlayer)
        {
            DotaNewRoomPlayer slot1Player = slot1.GetPlayer();
            slot1.RemovePlayer();
            slot2.SetPlayer(slot1Player);
        }
        else if (!slot1.HasPlayer && slot2.HasPlayer)
        {
            DotaNewRoomPlayer slot2Player = slot2.GetPlayer();
            slot2.RemovePlayer();
            slot1.SetPlayer(slot2Player);
        }
        else
        {
            return;
        }
    }

    public void SetPlayer(DotaNewRoomPlayer player)
    {
        if(cardPlayer != null) 
        {
            Debug.LogError("Slot already has player");
            return;
        }

        if(player.GetTeam() != team)
        {
            Debug.LogError("player is not the same team as slot");
        }

        cardPlayer = player;
        playerCardInstance = Instantiate(playerCardPrefab, transform);
        playerCardInstance.SetCard(player.GetPlayerName(), player.GetTeam(), player.GetChampionId(), player.GetIsReady());
        playerCardInstance.transform.localPosition = Vector3.zero;
    }

    public void RemovePlayer()
    {
        if(cardPlayer != null)
        {
            cardPlayer = null;
            Destroy(playerCardInstance.gameObject);
        }
    }
}
