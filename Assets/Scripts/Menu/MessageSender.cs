using Dota.Networking;
using Mirror;
using UnityEngine;

public class MessageSender : NetworkBehaviour
{
    public void ToggleReady()
    {
        NetworkClient.Send(new PlayerUpdateMessage { toggleReady = true });
    }

    public void SwitchTeam()
    {
        NetworkClient.Send(new PlayerUpdateMessage { switchTeam = true });
    }
}
