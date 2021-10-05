using Dota.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dota.Attributes;

public class StatsDisplay : MonoBehaviour
{
    [SerializeField] StatsRow[] statRows = null;

    [SerializeField] StringIconMapping stringIconMapping = null;

    StatStore statStore = null;

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
        statRows[0].SetRow(stringIconMapping.GetIcon("moveSpeed"), stats.moveSpeed);
        statRows[1].SetRow(stringIconMapping.GetIcon("attackSpeed"), stats.attackDamage);
        statRows[2].SetRow(stringIconMapping.GetIcon("attackDamage"), stats.attackDamage);
        statRows[3].SetRow(stringIconMapping.GetIcon("magicDamage"), stats.attackDamage);
        statRows[4].SetRow(stringIconMapping.GetIcon("armor"), stats.armor);
        statRows[5].SetRow(stringIconMapping.GetIcon("magicResist"), stats.armor);
        statRows[6].SetRow(stringIconMapping.GetIcon("maxHealth"), stats.maxHealth);
        statRows[7].SetRow(stringIconMapping.GetIcon("maxMana"), stats.maxMana); 
    }
}
