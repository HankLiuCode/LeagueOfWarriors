using Dota.Core;
using Dota.Networking;
using Dota.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoDisplay : MonoBehaviour
{
    [SerializeField] HealthDisplay healthDisplay = null;
    [SerializeField] ManaDisplay manaDisplay = null;
    [SerializeField] StatsDisplay statsDisplay = null;

    public void SetInfo(Stats stats)
    {
        Health health = stats.GetComponent<Health>();
        Mana mana = stats.GetComponent<Mana>();

        healthDisplay.SetHealth(health);
        manaDisplay.SetMana(mana);
        statsDisplay.SetStats(stats);
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(CameraController.GetMouseRay(), out RaycastHit hit, Mathf.Infinity))
            {
                GameObject go = hit.collider.gameObject;
                Stats stats = go.GetComponent<Stats>();
                Debug.Log(stats);

                if (stats != null)
                {
                    SetInfo(stats);
                }
            }
        }
    }
}
