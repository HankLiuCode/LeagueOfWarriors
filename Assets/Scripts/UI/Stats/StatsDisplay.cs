using Dota.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dota.Attributes;

public class StatsDisplay : MonoBehaviour
{
    [SerializeField] StatStore stats = null;

    [SerializeField] StatsRow speed = null;
    [SerializeField] StatsRow damage = null;
    [SerializeField] StatsRow maxMana = null;
    [SerializeField] StatsRow maxHealth = null;

    public void SetStats(StatStore stats)
    {
        this.stats = stats;

        if(this.stats != null)
        {
            this.stats.OnStatsModified -= Stats_OnStatsModified;
        }
        this.stats.OnStatsModified += Stats_OnStatsModified;

        UpdateStats();
    }

    private void Stats_OnStatsModified()
    {
        UpdateStats();
    }

    private void UpdateStats()
    {
        Debug.Log("UpdateStats");
        speed.SetRow("speed", this.stats.GetMovementSpeed());
        damage.SetRow("damage", this.stats.GetDamage());
        maxHealth.SetRow("maxHealth", this.stats.GetMaxHealth());
        maxMana.SetRow("maxMana", this.stats.GetMaxMana()); 
    }
}
