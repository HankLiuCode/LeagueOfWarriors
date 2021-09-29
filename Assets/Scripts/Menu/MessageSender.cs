using Dota.Networking;
using Mirror;
using UnityEngine;

public class MessageSender : NetworkBehaviour
{

    #region Client
    public void LastChampion()
    {
        DotaRoomPlayer localPlayer = NetworkClient.localPlayer.GetComponent<DotaRoomPlayer>();
        int championId = localPlayer.GetChampionId();
        localPlayer.CmdSetChampionId(championId - 1);
    }

    public void NextChampion()
    {
        DotaRoomPlayer localPlayer = NetworkClient.localPlayer.GetComponent<DotaRoomPlayer>();
        int championId = localPlayer.GetChampionId();
        localPlayer.CmdSetChampionId(championId + 1);
    }

    public void ToggleReady()
    {
        DotaRoomPlayer localPlayer = NetworkClient.localPlayer.GetComponent<DotaRoomPlayer>();
        bool isRoomReady = (localPlayer.GetConnectionState() == PlayerConnectionState.RoomReady);
        localPlayer.CmdSetConnectionState(isRoomReady ? PlayerConnectionState.RoomNotReady : PlayerConnectionState.RoomReady);
    }

    public void SwitchTeam()
    {
        DotaRoomPlayer localPlayer = NetworkClient.localPlayer.GetComponent<DotaRoomPlayer>();
        Team team = localPlayer.GetTeam();

        if (team == Team.Red)
        {
            localPlayer.CmdSetTeam(Team.Blue);
        }
        else
        {
            localPlayer.CmdSetTeam(Team.Red);
        }
    }
    #endregion
}
