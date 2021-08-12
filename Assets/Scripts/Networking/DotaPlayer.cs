using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotaPlayer : NetworkBehaviour
{

    [Command]
    public void CmdSpawnEffect(GameObject gameObject)
    {
        NetworkServer.Spawn(gameObject);
    }

    [Command]
    public void CmdDestroyEffect(GameObject gameObject)
    {
        NetworkServer.Destroy(gameObject);
    }
}
