using Dota.Networking;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ClientConnectMessage : NetworkMessage
{
    public string playerName;
}

public struct ClientGameToRoomRequestMessage : NetworkMessage {}

public struct ClientSceneLoadedMessage : NetworkMessage 
{
    public string scenePath;
}

public struct AllClientFinishLoadSceneMessage : NetworkMessage
{
    public string scenePath;
}