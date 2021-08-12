using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : NetworkBehaviour
{
    [ServerCallback]
    void Update()
    {
        transform.position = Vector3.forward * Mathf.Sin(Time.time / 2) * 4;
    }
}
