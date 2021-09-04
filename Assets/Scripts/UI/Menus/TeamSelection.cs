using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class TeamSelection : MonoBehaviour
{
    public void RedSelected(bool selected)
    {
        if (selected)
        {
            DotaRoomPlayer dRoomPlayer = GetLocalRoomPlayer();
            dRoomPlayer.CmdSetTeam(Team.Red);
        }
    }

    public void BlueSelected(bool selected)
    {
        if (selected)
        {
            DotaRoomPlayer dRoomPlayer = GetLocalRoomPlayer();
            dRoomPlayer.CmdSetTeam(Team.Blue);
        }
    }

    public DotaRoomPlayer GetLocalRoomPlayer()
    {
        List<DotaRoomPlayer> roomPlayers = ((DotaNetworkRoomManager) NetworkRoomManager.singleton).GetDotaRoomPlayers();

        foreach(DotaRoomPlayer drp in roomPlayers)
        {
            if (drp.isLocalPlayer)
            {
                return drp;
            }
        }
        return null;
    }
}
