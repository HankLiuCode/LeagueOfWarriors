using Dota.Networking;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerConnectMessage : NetworkMessage
{
    public string playerName;
}

public struct PlayerUpdateMessage : NetworkMessage
{
    public bool switchTeam;
    public bool toggleReady;
    public bool switchChampion;

    public int championId;
}


public struct ClientSceneLoadedMessage : NetworkMessage { }
public struct PlayerLeaveMessage : NetworkMessage { }