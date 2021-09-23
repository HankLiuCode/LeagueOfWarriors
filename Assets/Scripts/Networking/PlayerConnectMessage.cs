using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerConnectMessage : NetworkMessage
{
    public string playerName;
    public Team team;
    public int championId;
}

public struct NewPlayerEnterMessage : NetworkMessage { }

public struct PlayerLeaveMessage : NetworkMessage { }