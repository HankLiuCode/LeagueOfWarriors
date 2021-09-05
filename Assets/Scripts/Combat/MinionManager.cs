using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionManager : NetworkBehaviour
{
    [SerializeField] GameObject minionPrefab;

    public override void OnStartServer()
    {
        GameObject minionInstance = Instantiate(minionPrefab, Vector3.zero, Quaternion.identity);
        NetworkServer.Spawn(minionInstance);
    }
}
