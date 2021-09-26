using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Dota.UI;
using Dota.Core;
using Dota.Networking;
using Dota.Controls;

public class CameraSetup : MonoBehaviour
{
    [SerializeField] CameraController cameraControllerInstance = null;

    private void Start()
    {
        Champion.OnChampionSpawned += Champion_OnChampionSpawned;
    }

    private void OnDestroy()
    {
        Champion.OnChampionSpawned -= Champion_OnChampionSpawned;
    }

    private void Champion_OnChampionSpawned(Champion champion)
    {
        if (champion.hasAuthority)
        {
            cameraControllerInstance.SetFollowTarget(champion.transform);
        }
    }

}
