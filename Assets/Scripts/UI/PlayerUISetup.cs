using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Dota.UI;
using Dota.Core;
using Dota.Networking;
using Dota.Controls;

public class PlayerUISetup : MonoBehaviour
{
    [SerializeField] GameObject displayPrefab = null;
    [SerializeField] CameraController cameraControllerPrefab = null;

    HealthDisplay healthDisplayInstance = null;
    ManaDisplay manaDisplayInstance = null;
    CameraController cameraControllerInstance = null;


    public void SetUpSelfUI(DotaPlayerController localPlayerController)
    {
        GameObject displayInstance = Instantiate(displayPrefab);
        healthDisplayInstance = displayInstance.GetComponent<HealthDisplay>();
        manaDisplayInstance = displayInstance.GetComponent<ManaDisplay>();
        cameraControllerInstance = Instantiate(cameraControllerPrefab);

        cameraControllerInstance.Initialize(localPlayerController.transform);
        healthDisplayInstance.SetHealth(localPlayerController.GetComponent<Health>());
        manaDisplayInstance.SetMana(localPlayerController.GetComponent<Mana>());
    }

    public void DestroySelfUI()
    {
        Destroy(healthDisplayInstance.gameObject);
        Destroy(cameraControllerInstance.gameObject);
        Destroy(gameObject);
    }
}
