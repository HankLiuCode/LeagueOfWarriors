using Dota.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dota.Attributes;

public class StatsDisplay : MonoBehaviour
{
    [SerializeField] StatStore statStore = null;

    [SerializeField] StatsRow speed = null;
    [SerializeField] StatsRow damage = null;
    [SerializeField] StatsRow maxMana = null;
    [SerializeField] StatsRow maxHealth = null;

    public void SetStats(StatStore statStore)
    {
        if(this.statStore != null)
        {
            this.statStore.OnStatsModified -= StatStore_OnStatsModified;
        }
        this.statStore = statStore;
        this.statStore.OnStatsModified += StatStore_OnStatsModified;

        UpdateStats(this.statStore.GetStats());
    }

    private void StatStore_OnStatsModified(Stats stats)
    {
        UpdateStats(stats);
    }

    private void UpdateStats(Stats stats)
    {
        Debug.Log("UpdateStats");
        speed.SetRow("speed", stats.moveSpeed);
        damage.SetRow("damage", stats.attackDamage);
        maxHealth.SetRow("maxHealth", stats.maxHealth);
        maxMana.SetRow("maxMana", stats.maxMana); 
    }
}
