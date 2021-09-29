using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Dota.Networking;

public class PlayerCard : MonoBehaviour
{
    [SerializeField] GameObject card = null;
    [SerializeField] Image playerIcon = null;
    [SerializeField] Image teamBackground = null;
    [SerializeField] TextMeshProUGUI teamTextMesh = null;
    [SerializeField] TextMeshProUGUI readyTextMesh = null;
    [SerializeField] ChampionIdMapping mapping = null;


    public void HideCard()
    {
        card.gameObject.SetActive(false);
    }

    public void SetCard(Team team, int championId, PlayerConnectionState connectionState)
    {
        card.gameObject.SetActive(true);

        playerIcon.sprite = mapping.GetIcon(championId);
        teamBackground.color = (team == Team.Red) ? Color.red : Color.white;
        teamTextMesh.text = team.ToString();

        string showText;
        switch (connectionState)
        {
            case PlayerConnectionState.RoomToGame:
                showText = "entering";
                break;

            case PlayerConnectionState.RoomReady:
                showText = "ready";
                teamTextMesh.gameObject.SetActive(false);
                break;

            case PlayerConnectionState.RoomNotReady:
                showText = "not ready";
                break;

            default:
                showText = connectionState.ToString();
                break;
        }

        if(readyTextMesh != null)
        {
            readyTextMesh.text = showText;
        }
    }
}
