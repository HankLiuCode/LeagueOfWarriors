using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeathScreen : MonoBehaviour
{
    [SerializeField] Canvas canvas = null;
    [SerializeField] TextMeshProUGUI deathText = null;

    float deathTime = 10f;

    private void Start()
    {
        Champion.OnChampionSpawned += Champion_OnChampionSpawned;
        Champion.ClientOnChampionDead += Champion_ClientOnChampionDead;
    }

    private void Update()
    {
        deathTime = Mathf.Max(deathTime - Time.deltaTime, 0);
        deathText.text = "Revive After: " + Mathf.CeilToInt(deathTime);
    }

    private void OnDestroy()
    {
        Champion.OnChampionSpawned -= Champion_OnChampionSpawned;
    }

    private void Champion_ClientOnChampionDead(Champion champion)
    {
        deathTime = Champion.REVIVE_TIME;
        if (champion.hasAuthority)
        {
            canvas.gameObject.SetActive(true);
        }
    }

    private void Champion_OnChampionSpawned(Champion champion)
    {
        if (champion.hasAuthority)
        {
            canvas.gameObject.SetActive(false);
        }
    }
}
