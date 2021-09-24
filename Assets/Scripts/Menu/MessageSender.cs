using Dota.Networking;
using Mirror;
using UnityEngine;

public class MessageSender : NetworkBehaviour
{
    DotaRoomPlayer localPlayer = null;

    public override void OnStartClient()
    {
        DotaRoomPlayer.OnPlayerConnected += DotaRoomPlayer_OnPlayerConnect;
    }

    public override void OnStopClient()
    {
        DotaRoomPlayer.OnPlayerConnected -= DotaRoomPlayer_OnPlayerConnect;
    }

    private void DotaRoomPlayer_OnPlayerConnect(DotaRoomPlayer player)
    {
        if (player.isLocalPlayer)
        {
            localPlayer = player;
        }
    }

    public void ToggleReady()
    {
        bool isRoomReady = (localPlayer.GetConnectionState() == PlayerConnectionState.RoomReady);
        localPlayer.CmdSetConnectionState(isRoomReady ? PlayerConnectionState.RoomNotReady : PlayerConnectionState.RoomReady);
    }

    public void SwitchTeam()
    {
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
}
