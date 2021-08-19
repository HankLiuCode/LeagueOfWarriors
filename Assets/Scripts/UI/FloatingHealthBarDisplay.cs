using Dota.Controls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingHealthBarDisplay : MonoBehaviour
{
    [SerializeField] private GameObject healthBarPrefab = null;

    List<DotaPlayerController> allPlayers = null;

    private void Update()
    {
        if(allPlayers == null)
        {
            DotaPlayerController[] controllers = GameObject.FindObjectsOfType<DotaPlayerController>();
            allPlayers = new List<DotaPlayerController>(controllers);
        }
    }
}
