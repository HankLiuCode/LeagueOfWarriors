﻿using Dota.Controls;
using Dota.Networking;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotaPlayer : NetworkBehaviour
{
    [SerializeField] GameObject characterPrefab = null;
    [SerializeField] CameraController cameraController = null; // client
    DotaPlayerController dotaPlayerController = null; // server

    // 1. Need to know if player is in lobby, is in game, is disconnected
    // 2. Need to know what champion the player chose

    #region Server
    public override void OnStartServer()
    {
        GameObject characterInstance = Instantiate(characterPrefab, Vector3.zero, Quaternion.identity);
        dotaPlayerController = characterInstance.GetComponent<DotaPlayerController>();
        NetworkServer.Spawn(characterInstance, connectionToClient);
    }
    #endregion


    #region Client
    public override void OnStartAuthority()
    {
        cameraController.gameObject.SetActive(true);
        cameraController.SetFollowTarget(FindLocalPlayerController().transform);
    }

    public override void OnStopClient()
    {
        if (!hasAuthority) { return; }
        cameraController.gameObject.SetActive(false);
    }

    public DotaPlayerController FindLocalPlayerController()
    {
        DotaPlayerController[] controllers = GameObject.FindObjectsOfType<DotaPlayerController>();
        foreach (DotaPlayerController dpc in controllers)
        {
            if (dpc.hasAuthority)
            {
                return dpc;
            }
        }
        return null;
    }
    #endregion
}
